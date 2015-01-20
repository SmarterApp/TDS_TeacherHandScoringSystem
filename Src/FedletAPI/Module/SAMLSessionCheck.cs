using SAML.Common;
using Sun.Identity.Saml2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace SAML.Module
{
    public class SAMLSessionCheck : IHttpModule, IReadOnlySessionState
    {
        private static readonly string pageToSkip = "SessionEnd";

        void IHttpModule.Init(HttpApplication application)
        {
            if (application == null)
                throw new ArgumentNullException("Application is null");

            OnInit(application);

            application.PreRequestHandlerExecute += application_PreRequestHandlerExecute;


        }

        void application_PostReleaseRequestState(object sender, EventArgs e)
        {
        }

        void Application_PostMapRequestHandler(object source, EventArgs e)
        {
        }

        void Application_PostAcquireRequestState(object source, EventArgs e)
        {

        }

        void application_PostAcquireRequestState(object sender, EventArgs e)
        {
        }

        void application_BeginRequest(object sender, EventArgs e)
        {
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
        }

        void application_PreRequestHandlerExecute(object sender, EventArgs e)
        {

            bool HasRun = HttpContext.Current.Request.Cookies.Get("HASRUN") != null;

            if (HasRun)
                return;


            //Session endded not to particupate in openam check
            if (HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains(pageToSkip.ToUpper()))
            {
                return;
            }

            Page requestedPage = HttpContext.Current.CurrentHandler as Page;
            if (requestedPage != null &&
                (!HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains("INITIATELOGIN.ASPX") &&
                 !HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains("/API/")))
            {
                requestedPage.Load += new EventHandler(Page_Load);
                return;
            }

            System.Web.Mvc.MvcHandler requestedMvcPage = HttpContext.Current.CurrentHandler as System.Web.Mvc.MvcHandler;
            if (requestedMvcPage != null && HttpContext.Current.Session["SAMLResponse"] == null && !HttpContext.Current.Request.Url.LocalPath.ToUpper().Contains("/API/"))
            {
                ServiceProviderUtility.Login(HttpContext.Current, HttpContext.Current.Request, Config.ConfigFolder);
            }
        }


        private void CheckIfAuthenticate()
        {
        }

        void IHttpModule.Dispose()
        {
            OnDispose();
        }

        private void Page_Load(object sender, EventArgs e)
        {
            Page requestedPage = sender as Page;
            #region OLD

            //            
            //            string sessionActive =
            //                string.Format(@"        
            //                        keepalive_url = '{0}'; interval = {1}; 
            //                        function ssoKeepAlive() 
            //                        {{
            //                            $.get(keepalive_url, function (data, status) 
            //                              {{
            //                                if (data.substring(0, 2) == 'OK') {{}}
            //                                else {{return;}}
            //                              setTimeout('ssoKeepAlive()', interval);                       
            //                              }});
            //                        }};
            //                window.onload = function () {{ setTimeout('ssoKeepAlive()', interval); }}", Config.KeepAlive, Config.IdealTimeout);

            //            requestedPage.ClientScript.RegisterClientScriptBlock(requestedPage.GetType(), "SAMLCheck", sessionActive, true);

            //            //            if (requestedPage.Session[SAML.Common.Config.UsernameCookie] != null && requestedPage.Session[SAML.Common.Config.UsernameCookie].ToString().ToUpper() != string.Empty)
            //            if (requestedPage.Request.Cookies[SAML.Common.Config.UsernameCookie] != null && requestedPage.Request.Cookies[SAML.Common.Config.UsernameCookie].ToString().ToUpper() != string.Empty)
            //            {
            //                //Response.Write("Authorized");
            //            }
            //            else
            //            {
            //                if (requestedPage.Session == null)
            //                    requestedPage.Response.Write("not autorized" + Config.ConfigFolder);
            //                else
            //                    ServiceProviderUtility.Login(HttpContext.Current, requestedPage.Request, Config.ConfigFolder);
            //            }
            #endregion

            if (HttpContext.Current.Session["SAMLResponse"] == null)
                ServiceProviderUtility.Login(HttpContext.Current, requestedPage.Request, Config.ConfigFolder);

        }

        protected virtual void OnInit(HttpApplication application) { }

        protected virtual void OnDispose() { }
    }
}
