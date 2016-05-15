using SAML.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.SessionState;

namespace SAML.Handler
{
    class openam_keepalive : System.Web.UI.Page, IReadOnlySessionState
    {
        protected override void OnLoad(EventArgs e)
        {
            HttpWebRequest request = WebRequest.Create(Config.RefreshSessionURL) as HttpWebRequest;
            request.Method = "POST";

            Uri uri = new Uri(Config.RefreshSessionURL);
            CookieContainer ck = new CookieContainer();
            ck.Add(new Cookie(Config.TokenCookie, Request.Cookies[Config.TokenCookie].Value, string.Empty, uri.Authority));
            request.CookieContainer = ck;

            request.AllowAutoRedirect = true;
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string str = reader.ReadToEnd();
            if (str.Contains("TokenExpired"))
            {
                Response.Write("Failed");
            }
            else
            {
                Response.Write("OK");
            }
            //Response.Write("ok");
        }
    }
}
