#region License
// /*******************************************************************************                                                                                                                                    
//  * Educational Online Test Delivery System                                                                                                                                                                       
//  * Copyright (c) 2014 American Institutes for Research                                                                                                                                                              
//  *                                                                                                                                                                                                                  
//  * Distributed under the AIR Open Source License, Version 1.0                                                                                                                                                       
//  * See accompanying file AIR-License-1_0.txt or at                                                                                                                                                                  
//  * http://www.smarterapp.org/documents/American_Institutes_for_Research_Open_Source_Software_License.pdf                                                                                                                                                 
//  ******************************************************************************/ 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Sun.Identity.Saml2;
using Sun.Identity.Saml2.Exceptions;
using Sun.Identity.Common;

namespace TSS.MVC
{
    public partial class InitiateLogin : System.Web.UI.Page
    {

      

        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> logoutURLs = new Dictionary<string, string>();
            string username = string.Empty;
            if (this.Context.Session == null)
                throw new Exception("Seesion null in page");

            ServiceProviderUtility.LoginAndRedirect(this.Context, this.Response,
                SAML.Common.Config.ConfigFolder, SAML.Common.Config.Portal, out username, out logoutURLs);
            
            this.Response.Cookies.Add(new HttpCookie(SAML.Common.Config.UsernameCookie, username));
            this.Response.Cookies.Add(new HttpCookie("LogoutURL", logoutURLs["IDP_HttpPostProtocolBinding"]));
            //Session["Username"] = username;
            //Session["LogoutURL"] = logoutURLs["IDP_HttpPostProtocolBinding"];
            //Response.Redirect(SAML.Common.Config.Portal);
        }
    }
}