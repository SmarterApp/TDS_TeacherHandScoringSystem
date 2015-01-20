using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SAML.Handler
{
    class HandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context,
            string requestType, String url, String pathTranslated)
        {
            string pageUrl = System.IO.Path.GetFileName(pathTranslated).ToLowerInvariant();

            IHttpHandler handlerToReturn = null;
            if (pageUrl.ToUpper().Contains("INITIATELOGIN"))
            {
                handlerToReturn = new InitiateLogin();
            }
            else if (pageUrl.ToUpper().Contains("INITIATELOGOUT"))
            {
                handlerToReturn = new InitiateLogout();
            }
            else if (pageUrl.ToUpper().Contains("OPENAM_KEEPALIVE"))
            {
                handlerToReturn = new openam_keepalive();
            }

                context.Response.Redirect(string.Format("~/{0}", url));

            return handlerToReturn;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

}