using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatentFormVer
{
    public partial class MainForm : Form
    {
        private readonly ChromiumWebBrowser webBrowser;
        private readonly string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "works");

        private ScrapingBrowser scrapeBrowser = new ScrapingBrowser();
        private bool refreshed = false;
        private bool searching = false;
        private readonly string pageUrl = "http://epub.sipo.gov.cn/gjcx.jsp";

        public MainForm()
        {
            InitializeComponent();
            webBrowser = new ChromiumWebBrowser(pageUrl);
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.Dock = DockStyle.Fill;
            this.splitContainer1.Panel1.Controls.Add(webBrowser);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.DisableSearch();
        }

        private void DisableSearch()
        {
            EnableSearch(false);
        }

        private void EnableSearch(bool enable = true)
        {
            this.Invoke((Action)(() =>
            {
                this.txtKeywords.Enabled = enable;
                this.btnSearch.Enabled = enable;
            }));
        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
            }
        }

        private async void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            await SetCookie();
        }

        private async Task SetCookie()
        {
            var result = await this.webBrowser.GetCookieManager().VisitUrlCookiesAsync("http://epub.sipo.gov.cn/gjcx.jsp", true);
            if (result.Any(_ => _.Name == "JSESSIONID"))
            {
                scrapeBrowser.Encoding = Encoding.UTF8;

                foreach (var cookie in result)
                {
                    scrapeBrowser.SetCookies(new Uri("http://epub.sipo.gov.cn"), $@"{cookie.Name}={cookie.Value}; expires={cookie.Expires}; path=/");
                }

                this.refreshed = true;
                if (!this.searching)
                {
                    EnableSearch();
                }
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            this.searching = true;
            DisableSearch();
            await Search(scrapeBrowser, txtKeywords.Text, SourceType.InventPublish);
            this.searching = false;
            EnableSearch();
        }

        private async Task Search(ScrapingBrowser browser, string keywords, SourceType type)
        {
            var pageNum = 1;
            var items = new List<CrudeInfo>();
            var maxPage = -1;
            while (true)
            {
                AddStatus($"Working on page {pageNum}/{maxPage}");
                var page = await ParsePageAsync(browser, type, keywords, pageNum);
                items.AddRange(page.Items);
                SaveEachItem(page.Items);
                SaveSearch(keywords, type, items);
                AddStatus($"Saved items from page {pageNum}");

                this.refreshed = false;
                this.webBrowser.Load(pageUrl);

                await Task.Delay(10000);
                while (!this.refreshed)
                {
                    await Task.Delay(100);
                }

                if (page.MaxPage == page.CurrentPage) break;
                pageNum = page.NextPage;
                maxPage = page.MaxPage;
            }
        }

        private void SaveEachItem(IEnumerable<CrudeInfo> items)
        {
            foreach (var item in items)
            {
                var filepath = Path.Combine(basePath, $@"{item.Id}/raw.json");
                EnsureDirectoryExist(filepath);
                var json = JsonConvert.SerializeObject(item);
                File.WriteAllText(filepath, json);
            }
        }

        private void SaveSearch(string keywords, SourceType type, IEnumerable<CrudeInfo> items)
        {
            var filepath = Path.Combine(basePath, $@"{keywords}-{type}.json");
            EnsureDirectoryExist(filepath);
            var json = JsonConvert.SerializeObject(items);
            File.WriteAllText(filepath, json);
        }

        private void EnsureDirectoryExist(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir)) return;
            Directory.CreateDirectory(dir);
        }

        private void AddStatus(string msg)
        {
            this.txtStatus.Invoke((Action)(() =>
            {
                var newmsg = msg + Environment.NewLine + this.txtStatus.Text;
                newmsg = newmsg.Substring(0, newmsg.Length > 5000 ? 5000 : newmsg.Length);
                this.txtStatus.Text = newmsg;
            }));
        }

        private static async Task<PageInfo> ParsePageAsync(ScrapingBrowser browser, SourceType type, string keywords, int pageNumber)
        {
            var typeStr
                = type == SourceType.DesignGrant ? "pdg"
                : type == SourceType.InventGrant ? "pig"
                : type == SourceType.InventPublish ? "pip"
                : type == SourceType.UtilityGrant ? "pug"
                : throw new NotSupportedException("unknown type");
            var homePage = await Task.Run<WebPage>(() => browser.NavigateToPage(
                new Uri("http://epub.sipo.gov.cn/patentoutline.action"),
                HttpVerb.Post,
                $"showType=1&strSources={typeStr}&strWhere=%28TI%3D%27{keywords}%27%29" +
                $"&numSortMethod=0&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=0&numDgc=" +
                $"&pageSize=10&pageNow={pageNumber}"));
            var cps = homePage.Html.CssSelect("div.cp_box").ToArray();
            var list = cps.Select(_ => ParseItem(_)).ToArray();
            var pages = homePage.Html.CssSelect("div.next a").ToArray();
            var regPage = new Regex(@"\((?<g>[0-9]+)\)");
            var maxNumber = 0;
            var nextNumber = 0;
            var curNumber = 0;
            foreach (var page in pages)
            {
                var href = page.GetAttributeValue("href");
                var d = regPage.Match(href).Groups["g"].Value;
                if (int.TryParse(d, out var number))
                {
                    if (number > maxNumber) maxNumber = number;
                    if (page.InnerText == "&gt;")
                    {
                        nextNumber = number;
                    }
                    else if (page.GetClasses().Contains("hover"))
                    {
                        curNumber = number;
                    }
                }
            }

            return new PageInfo
            {
                Items = list,
                NextPage = nextNumber,
                MaxPage = maxNumber,
                CurrentPage = curNumber,
                Raw = homePage.Html.OuterHtml.Trim(),
            };
        }

        private static CrudeInfo ParseItem(HtmlAgilityPack.HtmlNode cp)
        {
            var imgElm = cp.CssSelect("div.cp_img img").FirstOrDefault();
            var img = imgElm?.GetAttributeValue("src");
            var headElm = cp.CssSelect("div.cp_linr h1").FirstOrDefault();
            var head = headElm?.InnerText;
            var detailElm = cp.CssSelect("div.cp_linr ul").FirstOrDefault();
            var details = detailElm?.InnerHtml;
            var descElm = cp.CssSelect("div.cp_linr div.cp_jsh").FirstOrDefault();
            var desc = descElm?.InnerHtml;
            var linksElm = cp.CssSelect("div.cp_linr p.cp_botsm").FirstOrDefault();
            var links = linksElm?.InnerHtml;
            var qrElm = cp.CssSelect("a.qrcode img").FirstOrDefault();
            var qr = qrElm?.GetAttributeValue("src");
            var idElm = cp.CssSelect("a.qrcode").FirstOrDefault();
            var id = idElm?.GetAttributeValue("id");

            return new CrudeInfo
            {
                Id = id.Trim(),
                Image = img.Trim(),
                Title = head.Trim(),
                Details = details.Trim(),
                Description = desc.Trim(),
                Links = links.Trim(),
                QrImage = qr.Trim(),
                Raw = cp.OuterHtml.Trim(),
            };
        }
    }

    public enum SourceType
    {
        // pip: 发明公布
        InventPublish,
        // pig: 发明授权
        InventGrant,
        // pug: 实用新型
        UtilityGrant,
        // pdg: 外观设计
        DesignGrant,
    }
    public class PageInfo
    {
        public CrudeInfo[] Items { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int MaxPage { get; set; }
        public string Raw { get; set; }
    }
    public class CrudeInfo
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }
        public string Links { get; set; }
        public string QrImage { get; set; }
        public string Raw { get; set; }
    }
}
