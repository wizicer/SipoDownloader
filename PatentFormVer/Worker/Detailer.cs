namespace PatentFormVer.Worker
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using PatentFormVer.Entity;
    using ScrapySharp.Network;

    public class Detailer : CrawlerBase
    {
        public Detailer(MainForm.UiInteractor ui)
            : base(ui)
        {
        }

        public async Task Process()
        {
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
                        this.Ui.AddStatus($"work on {i + 1}/{dirs.Length} image");
                        var url = imageBaseUrl + info.Image;
                        using (var wc = new TimeoutWebClient(10000))
                        {
                            await wc.DownloadFileTaskAsync(url, imagePath);
                        }
                    }
                }

                foreach (var link in ginfo.Links)
                {
                    var filePath = Path.Combine(dir, $"{ginfo.Id}-{ginfo.Title}-{link.Title}.pdf");
                    if (File.Exists(filePath)) continue;
                    var jsonFilePath = Path.Combine(dir, $"{link.Title}.json");

                    switch (link)
                    {
                        case GrantItemPamLink gipl:
                            this.Ui.AddStatus($"work on {i + 1}/{dirs.Length} {link.Title} {gipl.Id}");

                            var pam = await ParsePamAsync(gipl.Type, gipl.Id, gipl.Index);
                            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(pam));

                            using (var wc = new TimeoutWebClient(10000))
                            {
                                await wc.DownloadFileTaskAsync(pam.FileLink, filePath);
                            }

                            this.Ui.AddStatus($"Sleep {20}s");
                            await Task.Delay(20000);
                            break;

                        case GrantItemTxLink gitl:
                        default:
                            break;
                    }
                }

                if (i % 10 == 0)
                {
                    this.Ui.AddStatus($"Sleep {1}s");
                    await Task.Delay(1000);
                }
            }
        }

        private async Task<PamPageInfo> ParsePamAsync(string type, string id, string number)
        {
            var browser = new ScrapingBrowser();
            await this.SetNewCookie(browser);
            var homePage = await browser.NavigateToPageAsync(
                new Uri("http://epub.sipo.gov.cn/pam.action"),
                HttpVerb.Post,
                $"strSources={type}&strWhere=PN%3D%27{id}%27&recordCursor={number}&strLicenseCode=");
            return homePage.Html.ToPamPageInfo();
        }
    }
}