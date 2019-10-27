using System;
using System.Net;

namespace PatentFormVer
{
    internal class TimeoutWebClient : WebClient
    {
        private readonly int timeout;

        public TimeoutWebClient(int timeoutSeconds)
        {
            this.timeout = timeoutSeconds * 1000;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeout;
            return w;
        }
    }
}
