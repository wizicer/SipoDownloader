namespace PatentFormVer
{
    using HtmlAgilityPack;
    using Newtonsoft.Json;
    using PatentFormVer.Entity;
    using ScrapySharp.Extensions;
    using ScrapySharp.Html;
    using ScrapySharp.Network;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static GrantItemInfo ToGrantInfo(this RawGrantItemInfo info)
        {
            var regTitle = new Regex(@"\[(?<type>.*)\]&nbsp;(?<title>.*)");
            var match = regTitle.Match(info.Title);
            var title = match.Groups["title"].Value;
            var type = match.Groups["type"].Value;

            var docDetails = new HtmlDocument();
            docDetails.LoadHtml(info.Details);
            var lis = docDetails.DocumentNode.CssSelect("li");
            var details = new List<GrantDetailInfo>();
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
                    details.Add(new GrantDetailInfo { Name = d[0].Trim(), Content = d[1].Trim() });
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

            var rePam = new Regex(@"javascript\:pam3\('(?<type>[piudg]{3})','(?<id>.+)','(?<index>\d?)'\);");
            var reTx = new Regex(@"javascript\:sw_xx\('(?<number>[0-9]*)'\);");
            var docLinks = new HtmlDocument();
            docLinks.LoadHtml(info.Links);
            var links = docLinks.DocumentNode.CssSelect("span a")
                .Select(link => new { href = link.GetAttributeValue("href"), text = link.InnerText })
                .Select(link =>
                {
                    var pamMatch = rePam.Match(link.href);
                    if (pamMatch.Success)
                    {
                        var pamType = match.Groups["type"].Value;
                        var pamId = match.Groups["id"].Value;
                        var pamIndex = match.Groups["index"].Value;
                        return (GrantItemLinkBase)new GrantItemPamLink
                        {
                            Title = link.text,
                            Type = pamType,
                            Id = pamId,
                            Index = pamIndex,
                        };
                    }

                    var txMatch = reTx.Match(link.href);
                    if (txMatch.Success)
                    {
                        var txNumber = match.Groups["number"].Value;
                        return new GrantItemTxLink
                        {
                            Title = link.text,
                            Number = txNumber,
                        };
                    }

                    throw new NotSupportedException("cannot parse link");
                })
                .ToArray();

            var imageUrl = Regex.Replace(info.Image, "_thumb.jpg$", ".jpg");

            return new GrantItemInfo
            {
                Description = desc,
                LeadingDescription = leadingDesc,
                Id = info.Id,
                Details = details.ToArray(),
                ThumbImage = info.Image,
                Image = imageUrl,
                Links = links,
                QrImage = info.QrImage,
                Title = title,
                Type = type,
            };
        }

        public static string ToTypeString(this SourceType type)
        {
            return type == SourceType.DesignGrant ? "pdg"
                : type == SourceType.InventGrant ? "pig"
                : type == SourceType.InventPublish ? "pip"
                : type == SourceType.UtilityGrant ? "pug"
                : throw new NotSupportedException("unknown type");
        }

        public static PamPageInfo ToPamPageInfo(this HtmlNode root)
        {
            var link = root.CssSelect("div.main dl.gbgw_lfl dd ul li a.right")
                .FirstOrDefault()?.GetAttributeValue("href");
            var box = root.CssSelect("iframe.pam_box")
                .FirstOrDefault()?.GetAttributeValue("src");

            return new PamPageInfo
            {
                FileLink = link,
                PreviewLink = box,
                Raw = root.OuterHtml,
            };
        }

        public static GrantListPageInfo ToGrantListPageInfo(this HtmlNode root)
        {
            var cps = root.CssSelect("div.cp_box").ToArray();
            var list = cps.Select(_ => _.ToRawGrantItemInfo()).ToArray();
            var pages = root.CssSelect("div.next a").ToArray();
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

            return new GrantListPageInfo
            {
                Items = list,
                NextPage = nextNumber,
                MaxPage = maxNumber,
                CurrentPage = curNumber,
                Raw = root.OuterHtml.Trim(),
            };
        }

        public static RawGrantItemInfo ToRawGrantItemInfo(this HtmlNode cp)
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

            return new RawGrantItemInfo
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
}
