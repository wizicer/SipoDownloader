namespace PatentFormVer.Worker
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;
    using PatentFormVer.Entity;
    using ScrapySharp.Network;
    using Serilog;

    public class Lister : CrawlerBase
    {
        public Lister(MainForm.UiInteractor ui)
            : base(ui)
        {
        }

        public async Task Search(string where, SourceType type, int pageNum)
        {
            var items = new List<RawGrantItemInfo>();
            var maxPage = -1;
            while (true)
            {
                if (pageNum == 1 && maxPage == -1)
                    this.Ui.AddStatus($"Working on first page");
                else
                    this.Ui.AddStatus($"Working on page {pageNum}/{maxPage}");

                GrantListPageInfo page;
                var retrySleep = 10;
                while (true)
                {
                    try
                    {
                        page = await ParsePageAsync(type, where, pageNum);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Warning(ex, "error get and parse page");
                        this.Ui.AddStatus($"Failed due to {ex.Message}, sleep {retrySleep}s then retry.");
                        await Task.Delay(retrySleep * 1000);
                        retrySleep += 10;
                    }
                }

                items.AddRange(page.Items);
                SaveEachItem(page.Items);
                page.Items.ToList().ForEach(_ => this.Ui.AddStatus($"Saved item: [{_.Id}] {_.Title}"));
                SaveSearch(where, type, items);
                this.Ui.AddStatus($"Saved items from page {pageNum} for where = {where}");

                if (page.MaxPage == page.CurrentPage) break;
                pageNum = page.NextPage;
                maxPage = page.MaxPage;

                this.Ui.AddStatus($"Sleep {10}s");
                await Task.Delay(10 * 1000);
            }
        }

        public string GetWhereClause(string keywords, string holder)
        {
            //PA='%xxxx科技有限公司%' and (TI='区块链')
            var where = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(holder))
            {
                where.Append($"PA='%{holder}%'");
            }

            if (!string.IsNullOrWhiteSpace(keywords))
            {
                if (where.Length > 0) where.Append(" and ");
                where.Append($"(TI='{keywords}')");
            }

            return where.ToString();
        }

        private static string MakeValidFileName(string name)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string escapedInvalidChars = Regex.Escape(invalidChars);
            string invalidRegex = string.Format(@"([{0}]*\.+$)|([{0}]+)", escapedInvalidChars);

            return Regex.Replace(name, invalidRegex, "_");
        }

        private async Task<GrantListPageInfo> ParsePageAsync(SourceType type, string where, int pageNumber)
        {
            var browser = new ScrapingBrowser();
            await this.SetNewCookie(browser);
            var typeStr = type.ToTypeString();
            var homePage = await browser.NavigateToPageAsync(
                new Uri("http://epub.sipo.gov.cn/patentoutline.action"),
                HttpVerb.Post,
                $"showType=1&strSources={typeStr}&strWhere={HttpUtility.UrlEncode(where)}" +
                $"&numSortMethod=0&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=0&numDgc=" +
                $"&pageSize=10&pageNow={pageNumber}");
            return homePage.Html.ToGrantListPageInfo();
        }

        private void SaveEachItem(IEnumerable<RawGrantItemInfo> items)
        {
            foreach (var item in items)
            {
                var filepath = Path.Combine(basePath, item.Id);
                EnsureDirectoryExist(filepath);
                File.WriteAllText(Path.Combine(filepath, "raw.json"), JsonConvert.SerializeObject(item));
                File.WriteAllText(Path.Combine(filepath, "info.json"), JsonConvert.SerializeObject(item.ToGrantInfo()));
            }
        }

        private void SaveSearch(string where, SourceType type, IEnumerable<RawGrantItemInfo> items)
        {
            // to avoid to large to out of memory
            var groups = items
                .Select((_, i) => new { group = i / 300, item = _ })
                .GroupBy(_ => _.group)
                .ToArray();
            foreach (var group in groups)
            {
                var filepath = Path.Combine(basePath, $@"{MakeValidFileName(where)}-{type}-{group.Key}.json");
                EnsureDirectoryExist(filepath);
                var json = JsonConvert.SerializeObject(group.Select(_ => _.item));
                File.WriteAllText(filepath, json);
            }

        }
    }
}