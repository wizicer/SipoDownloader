using CefSharp;
using CefSharp.WinForms;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public MainForm()
        {
            InitializeComponent();
            webBrowser = new ChromiumWebBrowser("http://epub.sipo.gov.cn/gjcx.jsp");
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.Dock = DockStyle.Fill;
            this.Controls.Add(webBrowser);
        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
            }
        }

        private async void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            var result = await this.webBrowser.GetCookieManager().VisitUrlCookiesAsync("http://epub.sipo.gov.cn/gjcx.jsp", true);
            if (result.Any(_ => _.Name == "JSESSIONID"))
            {
                var browser = new ScrapingBrowser();
                browser.Encoding = Encoding.UTF8;

                foreach (var cookie in result)
                {
                    browser.SetCookies(new Uri("http://epub.sipo.gov.cn"), $@"{cookie.Name}={cookie.Value}; expires={cookie.Expires}; path=/");
                }

                ParsePage(browser, SourceType.InventGrant, "区块链", 1);
            }
        }

        private static PageInfo ParsePage(ScrapingBrowser browser, SourceType type, string keywords, int pageNumber)
        {
            var typeStr
                = type == SourceType.DesignGrant ? "pdg"
                : type == SourceType.InventGrant ? "pig"
                : type == SourceType.InventPublish ? "pip"
                : type == SourceType.UtilityGrant ? "pug"
                : throw new NotSupportedException("unknown type");
            var homePage = browser.NavigateToPage(
                new Uri("http://epub.sipo.gov.cn/patentoutline.action"),
                HttpVerb.Post,
                $"showType=1&strSources={typeStr}&strWhere=%28TI%3D%27{keywords}%27%29" +
                $"&numSortMethod=0&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=0&numDgc=" +
                $"&pageSize=10&pageNow={pageNumber}");
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
