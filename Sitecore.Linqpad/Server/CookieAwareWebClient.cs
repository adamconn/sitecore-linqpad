using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Linqpad.Server
{
    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient(CookieContainer cookies = null)
        {
            this.Cookies = cookies ?? new CookieContainer();
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var webRequest = base.GetWebRequest(address);
            var request2 = webRequest as HttpWebRequest;
            if (request2 != null)
            {
                request2.CookieContainer = this.Cookies;
                request2.AllowAutoRedirect = false;
            }
            webRequest.Timeout = 300000;
            return webRequest;
        }

        public CookieContainer Cookies { get; private set; }
    }
}
