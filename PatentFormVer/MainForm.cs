using CefSharp;
using CefSharp.WinForms;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PatentFormVer
{
    public partial class MainForm : Form
    {
        private readonly ChromiumWebBrowser webBrowser;

        public MainForm()
        {
            InitializeComponent();
            webBrowser = new ChromiumWebBrowser("http://epub.sipo.gov.cn/gjcx.jsp");
            webBrowser.FrameLoadEnd += WebBrowser_FrameLoadEnd;
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;
            webBrowser.Dock = DockStyle.Fill;
            this.Controls.Add(webBrowser);
        }

        private void WebBrowser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
            }
        }

        private async void WebBrowser_FrameLoadEnd(object sender, CefSharp.FrameLoadEndEventArgs e)
        {
            //Cef.GetGlobalCookieManager().
            //var c = new CookieMonster();
            //await this.webBrowser.GetCookieManager().VisitUrlCookiesAsync("http://epub.sipo.gov.cn/gjcx.jsp", true, c);
            var result = await this.webBrowser.GetCookieManager().VisitUrlCookiesAsync("http://epub.sipo.gov.cn/gjcx.jsp", true);
            if (result.Any(_ => _.Name == "JSESSIONID"))
            {
                var browser = new ScrapingBrowser();
                //browser.Encoding = Encoding.GetEncoding(932);
                browser.Encoding = Encoding.UTF8;

                foreach (var cookie in result)
                {
                    browser.SetCookies(new Uri("http://epub.sipo.gov.cn"), $@"{cookie.Name}={cookie.Value}; expires={cookie.Expires}; path=/");
                    //browser.SetCookies(new Uri(cookie.Domain), $"{cookie.Name}={cookie.Value}");
                }

                var d = browser.GetCookie(new Uri("http://epub.sipo.gov.cn"), "JSESSIONID");



                //<div class="cp_img">
                //	<img onerror="javascript:this.src='images/cp_noimg.jpg';" src="pic/wshgg7100/PUBXML/20060906/WGSQ/WGSQ_DZGBD/2005301387738/130001/000003_thumb.jpg">
                //		</div>
                //<div class="cp_linr">
                //	<h1>
                //		[外观设计]&nbsp;小饰物（Pop-It Key Chain-Traditional)</h1>
                //	<ul>
                //		<li class="wl228">授权公告号：CN3558385</li>
                //				<li class="wl228">授权公告日：2006.09.06</li>
                //			<li class="wl228">申请号：2005301387738</li>
                //		<li class="wl228">申请日：2005.10.20</li>
                //		<li class="wl228">专利权人：威恩·苛恩</li>
                //		<li class="wl228">设计人：威恩·苛恩</li>
                //		<li class="clear"></li>
                //		<li>地址：香港上环干诺道西88号粤财大厦29楼</li>
                //		<li>分类号：11-02&nbsp;&nbsp;<a href="javascript:;" class="zhankai" style="color:#c5000f">全部</a>
                //			<div style="display:none;"><ul>
                //				<li>专利代理机构：广州华进联合专利商标代理有限公司</li><li>代理人：曾旻辉</li></ul></div>
                //		</li>
                //		</ul>
                //	<div class="cp_jsh">
                //		<span id="tit">
                //		简要说明：</span>
                //		左视图与右视图对称，省略左视图。</div>
                //	<p class="cp_botsm">
                //		<a href="javascript:zl_xm('2005301387738','pdg');">【全部数据】</a>
                //			<span><a href="javascript:sw_xx('2005301387738');">事务数据</a></span>
                //	</p>
                //</div>
                //<a class="qrcode" id="CN3558385" href="qrcode/CN3558385.png"><img src="qrcode/CN3558385.png" width="74" height="74"></a>




                // strSources
                // pip: 发明公布
                // pig: 发明授权
                // pug: 实用新型
                // pdg: 外观设计
                var type = "pdg";
                //var homePage = browser.NavigateToPage(new Uri("http://epub.sipo.gov.cn/gjcx.jsp"));
                var homePage = browser.NavigateToPage(
                    new Uri("http://epub.sipo.gov.cn/patentoutline.action"),
                    HttpVerb.Post,
                    $"showType=1&strSources={type}&strWhere=%28TI%3D%27区块链%27%29&numSortMethod=0&strLicenseCode=&numIp=&numIpc=&numIg=&numIgc=&numIgd=&numUg=&numUgc=&numUgd=&numDg=0&numDgc=&pageSize=10&pageNow=1");

                //var form = homePage.FindFormById("pato");
                //form["strWhere"] = "(TI='区块链')";
                //form["pageSize"] = "10";
                //form["pageNow"] = "1";
                //var resultsPage = form.Submit();

                //var cps = resultsPage.Html.CssSelect("div.cp_box").ToArray();
                var cps = homePage.Html.CssSelect("div.cp_box").ToArray();
                foreach (var cp in cps)
                {
                    var imgElm = cp.CssSelect("div.cp_img img").FirstOrDefault();
                    var img = imgElm.GetAttributeValue("src");
                }
            }
        }
    }
}
