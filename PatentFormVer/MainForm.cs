namespace PatentFormVer
{
    using CefSharp.WinForms;
    using PatentFormVer.Entity;
    using PatentFormVer.Worker;
    using ScrapySharp.Extensions;
    using Serilog;
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    public partial class MainForm : Form
    {
        private readonly UiInteractor ui;

        public MainForm()
        {
            InitializeComponent();
            this.ui = new UiInteractor(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void DisableSearch()
        {
            EnableSearch(false);
        }

        private void EnableSearch(bool enable = true)
        {
            this.Invoke((Action)(() =>
            {
                this.txtKeywords.Enabled = enable;
                this.btnSearch.Enabled = enable;
            }));
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            var lister = new Lister(this.ui);
            DisableSearch();
            var type = radIp.Checked ? SourceType.InventPublish
                : radUg.Checked ? SourceType.UtilityGrant
                : radIg.Checked ? SourceType.InventGrant
                : radDg.Checked ? SourceType.DesignGrant
                : throw new NotSupportedException("cannot find type");
            var where = lister.GetWhereClause(this.txtKeywords.Text, this.txtHolder.Text);
            await lister.Search(where, type, (int)this.numStartPage.Value);
            EnableSearch();
            this.ui.AddStatus($"Finish search for {where}");
        }

        private async void btnProcess_ClickAsync(object sender, EventArgs e)
        {
            var detailer = new Detailer(this.ui);
            await detailer.Process();
        }

        public class UiInteractor
        {
            private readonly MainForm mainForm;

            public UiInteractor(MainForm mainForm)
            {
                this.mainForm = mainForm;
            }

            public void SetWebBrowser(ChromiumWebBrowser browser)
            {
                foreach (Control ctrl in this.mainForm.splitContainer1.Panel1.Controls)
                {
                    this.mainForm.splitContainer1.Panel1.Controls.Remove(ctrl);
                }

                this.mainForm.splitContainer1.Panel1.Controls.Add(browser);
            }

            public void AddStatus(string msg)
            {
                Log.Logger.Information(msg);
                this.mainForm.txtStatus.Invoke((Action)(() =>
                {
                    var newmsg = msg + Environment.NewLine + this.mainForm.txtStatus.Text;
                    newmsg = newmsg.Substring(0, newmsg.Length > 5000 ? 5000 : newmsg.Length);
                    this.mainForm.txtStatus.Text = newmsg;
                }));
            }
        }
    }
}
