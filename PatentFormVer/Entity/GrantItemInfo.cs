namespace PatentFormVer.Entity
{
    using System.Collections.Generic;

    public class GrantItemInfo
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string ThumbImage { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public GrantDetailInfo[] Details { get; set; }
        public string LeadingDescription { get; set; }
        public string Description { get; set; }
        public GrantItemLinkBase[] Links { get; set; }
        public string QrImage { get; set; }
    }

    public abstract class GrantItemLinkBase
    {
        public string Title { get; set; }
    }
    public class GrantItemTxLink : GrantItemLinkBase
    {
        public string Number { get; set; }
    }
    public class GrantItemPamLink : GrantItemLinkBase
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Index { get; set; }
    }
}
