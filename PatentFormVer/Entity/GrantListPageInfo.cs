namespace PatentFormVer.Entity
{
    public class GrantListPageInfo
    {
        public RawGrantItemInfo[] Items { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int MaxPage { get; set; }
        public string Raw { get; set; }
    }
}
