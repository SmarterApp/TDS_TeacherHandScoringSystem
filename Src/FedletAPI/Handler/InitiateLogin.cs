using SAML.Common;
using Sun.Identity.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.SessionState;

namespace SAML.Handler
{
    class InitiateLogin : System.Web.UI.Page, IReadOnlySessionState
    {
        protected override void OnLoad(EventArgs e)
        {
            Dictionary<string, string> logoutURLs = new Dictionary<string, string>();
            string username = string.Empty;
            if (this.Context.Session == null)
                throw new Exception("Seesion null in page");
            ServiceProviderUtility.LoginAndRedirect(this.Context, this.Response,
                Config.ConfigFolder, Config.Portal, out username, out logoutURLs);

            System.Web.Security.FormsAuthentication.SetAuthCookie(username, false);

            username = SAML.Handler.MachineKeyData.Protect(username, false);

            this.Session["SAMLResponse"] = this.Request.Form["SAMLResponse"] != null ? Saml2Utils.ConvertFromBase64(this.Request.Form["SAMLResponse"]) : null;
            

            this.Response.Cookies.Add(new System.Web.HttpCookie(SAML.Common.Config.UsernameCookie, username));
            this.Response.Cookies.Add(new System.Web.HttpCookie("LogoutURL", logoutURLs["IDP_HttpPostProtocolBinding"]));
            Response.Redirect(Config.LandingPage);
        }
    }
}
