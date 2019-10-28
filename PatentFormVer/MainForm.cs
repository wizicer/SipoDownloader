using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using PatentFormVer.Entity;
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
            var items = new List<RawGrantItemInfo>();
            var maxPage = -1;
            while (true)
            {
                AddStatus($"Working on page {pageNum}/{maxPage}");
                var page = await ParsePageAsync(browser, type, keywords, pageNum);
                items.AddRange(page.Items);
                SaveEachItem(page.Items);
                SaveSearch(keywords, type, items);
                AddStatus($"Saved items from page {pageNum}");

                await SetNewCookie();

                if (page.MaxPage == page.CurrentPage) break;
                pageNum = page.NextPage;
                maxPage = page.MaxPage;

                AddStatus($"Sleep {10}s");
                await Task.Delay(10 * 1000);
            }
        }

        private async Task SetNewCookie()
        {
            this.refreshed = false;
            this.webBrowser.Load(pageUrl);

            while (!this.refreshed)
            {
                await Task.Delay(100);
            }
        }

        private void SaveEachItem(IEnumerable<RawGrantItemInfo> items)
        {
            foreach (var item in items)
            {
                {
                    var filepath = Path.Combine(basePath, $@"{item.Id}/raw.json");
                    EnsureDirectoryExist(filepath);
                    var json = JsonConvert.SerializeObject(item);
                    File.WriteAllText(filepath, json);
                }

                {
                    var filepath = Path.Combine(basePath, $@"{item.Id}/info.json");
                    EnsureDirectoryExist(filepath);
                    var json = JsonConvert.SerializeObject(item.ToGrantInfo());
                    File.WriteAllText(filepath, json);
                }
            }
        }

        private void SaveSearch(string keywords, SourceType type, IEnumerable<RawGrantItemInfo> items)
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

        private static async Task<GrantListPageInfo> ParsePageAsync(ScrapingBrowser browser, SourceType type, string keywords, int pageNumber)
        {
            var typeStr = type.ToTypeString();
            var homePage = await browser.NavigateToPageAsync(
                new Uri("http://epub.sipo.gov.cn/patentoutline.action"),
                HttpVerb.Post,
                $"showType=1&strSources={typeStr}&strWhere=%28TI%3D%27{keywords}%27%29" +
                $"&numSortMethod=0&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=0&numDgc=" +
                $"&pageSize=10&pageNow={pageNumber}");
            return homePage.Html.ToGrantListPageInfo();
        }

        private async void btnProcess_ClickAsync(object sender, EventArgs e)
        {
            var burl = "http://epub.sipo.gov.cn/";
            var dirs = Directory.GetDirectories(basePath);
            for (var i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                var rawJsonPath = Path.Combine(dir, "raw.json");
                if (!File.Exists(rawJsonPath)) continue;
                var info = JsonConvert.DeserializeObject<RawGrantItemInfo>(File.ReadAllText(rawJsonPath));
                var ginfo = info.ToGrantInfo();

                {
                    var imagePath = Path.Combine(dir, "image.jpg");
                    if (!File.Exists(imagePath))
                    {
                        AddStatus($"work on {i + 1}/{dirs.Length} image");
                        var url = burl + info.Image;
                        using (var wc = new TimeoutWebClient(10000))
                        {
                            await wc.DownloadFileTaskAsync(url, imagePath);
                        }
                    }
                }

                foreach (var link in ginfo.Links)
                {
                    var filePath = Path.Combine(dir, $"{link.Title}.pdf");
                    if (File.Exists(filePath)) continue;
                    var jsonFilePath = Path.Combine(dir, $"{link.Title}.json");

                    switch (link)
                    {
                        case GrantItemPamLink gipl:
                            AddStatus($"work on {i + 1}/{dirs.Length} {link.Title} {gipl.Id}");

                            await SetNewCookie();
                            var pam = await ParsePamAsync(this.scrapeBrowser, gipl.Type, gipl.Id, gipl.Index);
                            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(pam));

                            using (var wc = new TimeoutWebClient(10000))
                            {
                                await wc.DownloadFileTaskAsync(pam.FileLink, filePath);
                            }

                            AddStatus($"Sleep {20}s");
                            await Task.Delay(20000);
                            break;
                        case GrantItemTxLink gitl:
                        default:
                            break;
                    }
                }

                if (i % 10 == 0)
                {
                    AddStatus($"Sleep {1}s");
                    await Task.Delay(1000);
                }
            }
        }

        private static async Task<PamPageInfo> ParsePamAsync(ScrapingBrowser browser, string type, string id, string number)
        {
            var homePage = await browser.NavigateToPageAsync(
                new Uri("http://epub.sipo.gov.cn/pam.action"),
                HttpVerb.Post,
                $"strSources={type}&strWhere=PN%3D%27{id}%27&recordCursor={number}&strLicenseCode=");
            return homePage.Html.ToPamPageInfo();
        }
    }
}
