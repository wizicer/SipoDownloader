using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PatentFormVer.Entity
{
    public class PatentItemInfo
    {
        public string Id { get; set; }
        public string Preview { get; set; }
        public string QrImage { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // 授权公告号
        public string PublicationNumber { get; set; }
        // 授权公告日
        public DateTime? PublicationDate { get; set; }
        // 申请号
        public string ApplicationNumber { get; set; }
        // 申请日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? ApplicationDate { get; set; }
        // 同一申请的已公布的文献号
        public string LiteratureNumber { get; set; }
        // 申请公布日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? ApplicationPublishDate { get; set; }
        // 专利权人
        public string[] Patentees { get; set; }
        // 发明人
        public string[] Inventors { get; set; }
        // 地址
        public string Address { get; set; }
        // 分类号
        public string[] ClassNumbers { get; set; }
        // 专利代理机构
        public string Agency { get; set; }
        // 代理人
        public string[] Agents { get; set; }
        // 对比文件
        public string[] Files { get; set; }
        // 本国优先权
        public string[] NationalPriorities { get; set; }
        // 优先权
        public string[] Priorities { get; set; }
        // PCT进入国家阶段日
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime? PctEntryDate { get; set; }
        // PCT申请数据
        public string PctApplicationData { get; set; }
        // PCT公布数据
        public string PctPublicationData { get; set; }
        // 分案原申请
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
