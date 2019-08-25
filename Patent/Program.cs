using AngleSharp;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Patent
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Test1();

            var browser = new ScrapingBrowser();

            //set UseDefaultCookiesParser as false if a website returns invalid cookies format
            //browser.UseDefaultCookiesParser = false;

            var homePage = browser.NavigateToPage(new Uri("http://epub.sipo.gov.cn/gjcx.jsp"));

            var form = homePage.FindFormById("pato");
            form["ti"] = "chain";
            var resultsPage = form.Submit();

            var cps = resultsPage.Html.CssSelect("div.cp_box").ToArray();
            foreach (var cp in cps)
            {
                var imgElm = cp.CssSelect("div.cp_img img").FirstOrDefault();
                var img = imgElm.GetAttributeValue("src");
            }

            //var blogPage = resultsPage.FindLinks(By.Text("romcyber blog | Just another WordPress site")).Single().Click();
        }

        private static async Task Test1()
        {
            //var text = await GetText("区块链");
            var text = await GetText("chain");

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(text));

            var cells = document.QuerySelectorAll(".cp_box");
            var titles = cells.Select(m => m.TextContent);
            foreach (var cell in cells)
            {
                var img = cell.QuerySelector(".cp_img img").GetAttribute("src");
            }
        }

        private async static Task<string> GetText(string keywords, int pageNumber = 1)
        {
            var url = "http://epub.sipo.gov.cn/patentoutline.action";
            using (var wc = new TimeoutWebClient(180))
            {
                wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 OPR/58.0.3135.118";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers[HttpRequestHeader.CacheControl] = "max-age=0";
                //wc.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
                wc.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7";
                wc.Headers[HttpRequestHeader.Referer] = "http://epub.sipo.gov.cn/patentoutline.action";
                wc.Headers[HttpRequestHeader.Accept] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                wc.Headers[HttpRequestHeader.Cookie] = "cT6iSq1TseR480S=OER1NSkqjiyW5K5WBbczZzDMWUeJ8avQZh96Y81QPG11ApYjTCQ.Ns_xBTGrr5n6; WEB=20111132; _gscu_884396235=665139550ib43p15; _gscbrs_884396235=1; JSESSIONID=7A1826214BF50A63C6B160C73FADA583; _gscs_884396235=t665310295efbjn76|pv:1; cT6iSq1TseR480T=4JBEFX3ZiqwAK_S1cVVRjAVxOHYXrfOAG8W.NJRzruA1lJb.5IEzudis46SR4crOwXkSKXZ5uHsJFAfKgYvPs1EjaaBMmyDZXu1e8DQUvvVAmKqeyqdynJrw2zxSDf3kjRS9dPcW6XTzGwW01nlKVN7gUYB5vkagg5z7h25VHjT1p4bUS2ZNVLCepHXCc50h_WNioDEI.jxJWSAI8fH.id.yZ_RZ4dHi.uENfxy7PuXfHS9cEPhpkp7mJQ10utCs6Z8OTRUQPoyUWb1dCZ_s5bwliR9oucH0rgNtGRXOhKR.42m2falBlujAREcAybcCr_Q1KAWnjhZ_L1xJe5jjI1DIi";
                wc.Headers["Origin"] = "http://epub.sipo.gov.cn";
                wc.Headers["Upgrade-Insecure-Requests"] = "1";

                /*
Origin: http://epub.sipo.gov.cn
Upgrade-Insecure-Requests: 1

Referer: http://epub.sipo.gov.cn/patentoutline.action
Cache-Control: max-age=0
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 OPR/58.0.3135.118
Accept-Encoding: gzip, deflate
Accept-Language: en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7
Cookie: cT6iSq1TseR480S=OER1NSkqjiyW5K5WBbczZzDMWUeJ8avQZh96Y81QPG11ApYjTCQ.Ns_xBTGrr5n6; WEB=20111132; _gscu_884396235=665139550ib43p15; _gscbrs_884396235=1; JSESSIONID=468DAA8BCFDA0A8D4FD7DC97E7DF628E; _gscs_884396235=t66527797junfvx15|pv:5; cT6iSq1TseR480T=4FtSlyemAUgNEzV.QkSG0oSae3aUICDN5xN16FZVI9T.FFk1GEzVzL.j2GVG2Y9FkybEEyRoz3C0lolBbccMJmzsXQg7Z3T9nUXld1UG.DlT3dhSudds0qdCoPeZeeNa.o9QnpWqF7z2A9XBNFjavhvCvdQiZUasxXWSEcqg._S3gANEv9W3I8qZFy1TnzTJq3VU0uUvcv82nll_9FXIqAOSpeXvEc5Zi.YKjhB3cDLOadPUsFIljoyK4CH4ADacO.nEdAju4JfAR1.UDbpQifq7R8RNGLTZ9.ZzerIj6xmc6yACuC_YXCstp7cqmNThBQb4O0XXUKiRC_arUURAhdPg7

                 */

                var values = new NameValueCollection();
                values.Add("showType", "1");
                values.Add("strSources", "pig");
                //values.Add("strWhere", WebUtility.UrlEncode($"(TI='{keywords}')"));
                values.Add("strWhere", $"(TI='{keywords}')");
                values.Add("numSortMethod", "");
                values.Add("strLicenseCode", "");
                values.Add("numIp", "0");
                values.Add("numIpc", "");
                values.Add("numIg", "0");
                values.Add("numIgc", "");
                values.Add("numIgd", "");
                values.Add("numUg", "0");
                values.Add("numUgc", "");
                values.Add("numUgd", "");
                values.Add("numDg", "0");
                values.Add("numDgc", "");
                values.Add("pageSize", "3");
                values.Add("pageNow", pageNumber.ToString());
                //var ret = await wc.UploadValuesTaskAsync(url, values);
                //return Encoding.UTF8.GetString(ret);

                var str = "showType=1&strSources=&strWhere=%28TI%3D%27chain%27%29&numSortMethod=&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=&numDgc=&pageSize=3&pageNow=1";
                var ret = await wc.UploadStringTaskAsync(url, str);
                return ret;
            }
        }
    }

    internal class TimeoutWebClient : WebClient
    {
        private readonly int timeout;

        public TimeoutWebClient(int timeoutSeconds)
        {
            this.timeout = timeoutSeconds * 1000;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeout;
            return w;
        }
    }

}
