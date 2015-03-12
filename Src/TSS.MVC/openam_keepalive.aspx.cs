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
using SAML.Common;
using System;
using System.IO;
using System.Net;

namespace TSS.MVC
{
    public partial class openam_keepalive : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
                Response.Write("ok");
            }
        }
    }
}
