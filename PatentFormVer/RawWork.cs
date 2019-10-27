using HtmlAgilityPack;
using Newtonsoft.Json;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatentFormVer
{
    public class RawWork
    {
        public async Task Parse(string basePath)
        {
            foreach (var dir in Directory.GetDirectories(basePath))
            {
                var rawJsonPath = Path.Combine(dir, "raw.json");
                if (!File.Exists(rawJsonPath)) continue;
                var info = JsonConvert.DeserializeObject<CrudeInfo>(File.ReadAllText(rawJsonPath));

                var ginfo = GetGrantInfo(info);
            }
        }

        public static GrantInfo GetGrantInfo(CrudeInfo info)
        {
            var regTitle = new Regex(@"\[(?<type>.*)\]&nbsp;(?<title>.*)");
            var match = regTitle.Match(info.Title);
            var title = match.Groups["title"].Value;
            var type = match.Groups["type"].Value;

            var docDetails = new HtmlDocument();
            docDetails.LoadHtml(info.Details);
            var lis = docDetails.DocumentNode.CssSelect("li");
            var details = new List<DetailInfo>();
            foreach (var li in lis)
            {
                if (string.IsNullOrWhiteSpace(li.InnerText)) continue;
                var text = li.ChildNodes.First().InnerText;
                text = HtmlEntity.DeEntitize(text);
                var d = text.Split(
                    new[] { "：" }, StringSplitOptions.RemoveEmptyEntries);
                if (d.Length == 1)
                {
                    details.Last().Content += d[0].Trim();
                }
                else
                {
                    details.Add(new DetailInfo { Name = d[0].Trim(), Content = d[1].Trim() });
                }
            }

            var docDesc = new HtmlDocument();
            docDesc.LoadHtml(info.Description);
            var desc = docDesc.DocumentNode.InnerText;
            var leadingDesc = "";
            if (desc.EndsWith("全部"))
            {
                desc = desc.Substring(0, desc.Length - 2).Trim();
                desc = HtmlEntity.DeEntitize(desc);
                var d = desc.Split(
                    new[] { "：" }, StringSplitOptions.RemoveEmptyEntries);
                leadingDesc = d[0].Trim();
                desc = d[1].Trim();
            }
            else
            {
            }

            var docLinks = new HtmlDocument();
            docLinks.LoadHtml(info.Links);
            var links = docLinks.DocumentNode.CssSelect("span a")
                .Select(link => new { href = link.GetAttributeValue("href"), text = link.InnerText })
                .ToDictionary(_ => _.text, _ => _.href)
                .ToArray();

            return new GrantInfo
            {
                Description = desc,
                LeadingDescription = leadingDesc,
                Id = info.Id,
                Details = details.ToArray(),
                Image = info.Image,
                Links = links,
                QrImage = info.QrImage,
                Title = title,
                Type = type,
            };
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
    public class GrantInfo
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public DetailInfo[] Details { get; set; }
        public string LeadingDescription { get; set; }
        public string Description { get; set; }
        public KeyValuePair<string, string>[] Links { get; set; }
        public string QrImage { get; set; }
    }

    [System.Diagnostics.DebuggerDisplay("{Name}: {Content}")]
    public class DetailInfo
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

}
