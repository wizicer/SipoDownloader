namespace PatentFormVer.Entity
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;

    public class PatentItemInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("img")]
        public string Preview { get; set; }
        [JsonProperty("qr")]
        public string QrImage { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }

        // 授权公告号
        [JsonProperty("pubno")]
        public string PublicationNumber { get; set; }
        // 授权公告日
        [JsonProperty("pubdate")]
        public DateTime? PublicationDate { get; set; }
        // 申请号
        [JsonProperty("appno")]
        public string ApplicationNumber { get; set; }
        // 申请日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty("appdate")]
        public DateTime? ApplicationDate { get; set; }
        // 同一申请的已公布的文献号
        [JsonProperty("litno")]
        public string LiteratureNumber { get; set; }
        // 申请公布号
        [JsonProperty("apppubno")]
        public string ApplicationPublishNumber { get; set; }
        // 申请公布日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty("apppubdate")]
        public DateTime? ApplicationPublishDate { get; set; }
        // 申请人
        [JsonProperty("applicants")]
        public string[] Applicants { get; set; }
        // 专利权人
        [JsonProperty("patentees")]
        public string[] Patentees { get; set; }
        // 发明人
        [JsonProperty("inventors")]
        public string[] Inventors { get; set; }
        // 地址
        [JsonProperty("address")]
        public string Address { get; set; }
        // 分类号
        [JsonProperty("classes")]
        public string[] ClassNumbers { get; set; }
        // 专利代理机构
        [JsonProperty("agency")]
        public string Agency { get; set; }
        // 代理人
        [JsonProperty("agents")]
        public string[] Agents { get; set; }
        // 对比文件
        [JsonProperty("files")]
        public string[] Files { get; set; }
        // 本国优先权
        [JsonProperty("nprio")]
        public string[] NationalPriorities { get; set; }
        // 优先权
        [JsonProperty("prio")]
        public string[] Priorities { get; set; }
        // PCT进入国家阶段日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        [JsonProperty("pctdate")]
        public DateTime? PctEntryDate { get; set; }
        // PCT申请数据
        [JsonProperty("pctapp")]
        public string PctApplicationData { get; set; }
        // PCT公布数据
        [JsonProperty("pctpub")]
        public string PctPublicationData { get; set; }
        // 分案原申请
        [JsonProperty("oriapp")]
        public string OriginalApplication { get; set; }
    }

    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
