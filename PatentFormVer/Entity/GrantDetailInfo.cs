namespace PatentFormVer.Entity
{
    [System.Diagnostics.DebuggerDisplay("{Name, nq}: {ValuesDisplay, nq}")]
    public class GrantDetailInfo
    {
        public string Name { get; set; }
        public string[] Values { get; set; }

        protected string ValuesDisplay => string.Join(", ", Values);
    }
}
