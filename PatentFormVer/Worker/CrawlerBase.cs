namespace PatentFormVer.Worker
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using CefSharp;
    using CefSharp.WinForms;
    using ScrapySharp.Network;

    public abstract class CrawlerBase
    {
        protected readonly string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "works");

        protected readonly string pageUrl = "http://epub.sipo.gov.cn/gjcx.jsp";

        protected readonly string imageBaseUrl = "http://epub.sipo.gov.cn/";

        protected CrawlerBase(MainForm.UiInteractor ui)
        {
            this.Ui = ui;
        }

        protected MainForm.UiInteractor Ui { get; }

        protected async Task SetNewCookie(ScrapingBrowser scrapeBrowser)
        {
            var refreshed = false;
            var webBrowser = new ChromiumWebBrowser(pageUrl);
            webBrowser.FrameLoadEnd += async (s, e) =>
            {
                var result = await webBrowser.GetCookieManager().VisitUrlCookiesAsync(pageUrl, true);
                if (result.Any(_ => _.Name == "WEB"))
                {
                    scrapeBrowser.Encoding = Encoding.UTF8;

                    foreach (var cookie in result)
                    {
                        scrapeBrowser.SetCookies(new Uri("http://epub.sipo.gov.cn"), $@"{cookie.Name}={cookie.Value}; expires={cookie.Expires}; path=/");
                    }

                    refreshed = true;
                }
            };

            webBrowser.Dock = DockStyle.Fill;
            this.Ui.SetWebBrowser(webBrowser);

            while (!refreshed)
            {
                await Task.Delay(100);
            }
        }

        protected void EnsureDirectoryExist(string path)
        {
            var dir = string.IsNullOrWhiteSpace(Path.GetExtension(path))
                ? path
                : Path.GetDirectoryName(path);
            if (Directory.Exists(dir)) return;
            Directory.CreateDirectory(dir);
        }
    }
}