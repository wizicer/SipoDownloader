namespace PatentFormVer.Worker
{
    using Newtonsoft.Json;
    using PatentFormVer.Entity;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Integrator : CrawlerBase
    {
        public Integrator(MainForm.UiInteractor ui)
            : base(ui)
        {
        }

        public async Task IntegrateAsync()
        {
            var list = new ConcurrentBag<PatentItemInfo>();
            var bd = new DirectoryInfo(basePath);
            var dirs = bd.GetDirectories();
            var tasks = dirs.Select(Process).ToArray();

            Task Process(DirectoryInfo dir)
            {
                return Task.Run(() =>
                {
                    var file = Path.Combine(dir.FullName, "raw.json");
                    var json = File.ReadAllText(file);
                    var ent = JsonConvert.DeserializeObject<RawGrantItemInfo>(json);
                    var info = ent.ToGrantInfo().ToPatentInfo();
                    list.Add(info);
                });
            }

            await Task.WhenAll(tasks);

            var slist = list.OrderBy(_ => _.Id).ToArray();
            var gjson = JsonConvert.SerializeObject(slist, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            File.WriteAllText(Path.Combine(basePath, "patents.js"), "var patents = " + gjson);
            this.Ui.AddStatus("list.js generated.");
        }
    }
}