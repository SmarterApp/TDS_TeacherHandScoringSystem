using SAML.Common;
using Sun.Identity.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SAML.Handler
{
    class InitiateLogout : System.Web.UI.Page, IReadOnlySessionState
    {
        protected override void OnLoad(EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
            FormsAuthentication.SignOut();
            ServiceProviderUtility.LogoutAndRedirect(this.Request, this.Response, this.Context, Config.ConfigFolder,
                Config.Portal, true);
        }
    }
}
