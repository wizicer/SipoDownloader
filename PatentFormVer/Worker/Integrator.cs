namespace PatentFormVer.Worker
{
    using Newtonsoft.Json;
    using PatentFormVer.Entity;
    using System.IO;
    using System.Threading.Tasks;

    public class Integrator : CrawlerBase
    {
        public Integrator(MainForm.UiInteractor ui)
            : base(ui)
        {
        }

        public async Task IntegrateAsync()
        {
                var bd = new DirectoryInfo(basePath);
            var dirs = bd.GetDirectories();
            foreach (var dir in dirs)
            {
                var file = Path.Combine(dir.FullName, "raw.json");
                var json = File.ReadAllText(file);
                var ent = JsonConvert.DeserializeObject<RawGrantItemInfo>(json);
                var info = ent.ToGrantInfo();
            }
        }
    }
}