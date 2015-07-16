using System.Configuration;
using System.Web;
using System.Xml;
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
        private string OWNER_PREFIX
        {
            get
            {

                return ConfigurationManager.AppSettings["SAML_OWNER_PREFIX"].ToString();

            }
        }
     

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

            if (Request.Form["SAMLResponse"] != null)
                SamlParser(Saml2Utils.ConvertFromBase64(this.Request.Form["SAMLResponse"]));

            this.Response.Cookies.Add(new HttpCookie(Config.UsernameCookie, username));
            this.Response.Cookies.Add(new HttpCookie(Config.LogoutURL, logoutURLs["IDP_HttpPostProtocolBinding"]));
            Response.Redirect(Config.LandingPage);
        }


        private void SamlParser(string samlXMLdata)
        {
         
        
            string samldata = samlXMLdata;
            if (!samldata.StartsWith(@"<?xml version="))
            {
                samldata = Decode64Bit("PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4=") + samldata;
            }

            //PRE LOAD USER PERMISSIONS SCHEMA


            //LOAD XML DOCMENT
            XmlDocument xDoc = new XmlDocument();
            samldata = samldata.Replace(@"\", "");
            xDoc.LoadXml(samldata);

            //CREATE NAMESPACE MANAGER
            XmlNamespaceManager xMan = new XmlNamespaceManager(xDoc.NameTable);
            xMan.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            xMan.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            xMan.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");

            //BEGIN XML PARSE
            // TENANCY CHAIN COOKIE
            XmlNodeList xNodeList;
            List<String> RawList = new List<string>();
            xNodeList = xDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = '" + OWNER_PREFIX + "TenancyChain']/saml:AttributeValue", xMan);
            if (xNodeList != null)
            {//SAVE TENANCY LIST TO COOKIES
                for (int i = 0; i < xNodeList.Count; i++)
                {
                    string tenancyChain = Encryption.SimpleEncrypt(xNodeList[i].InnerText.ToString(), Encryption.CryptKey, Encryption.AuthKey, Encryption.SecMess);
                    RawList.Add(tenancyChain);
                    this.Response.Cookies.Add(new HttpCookie("_" + i.ToString(), tenancyChain ) { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
                }
            }
            //TSS MAIL COOKIE
            XmlNode xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'mail']/saml:AttributeValue", xMan);
            string mail = string.Empty;
            if (xNode != null)
            {
                mail = Encryption.SimpleEncrypt(xNode.InnerText, Encryption.CryptKey, Encryption.AuthKey, Encryption.SecMess);
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.TSS_MAIL, mail) { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
            }

            //TSS GivenName COOKIE
            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'givenName']/saml:AttributeValue", xMan);
            string givenName = string.Empty;
            if (xNode != null)
            {
                givenName = Encryption.SimpleEncrypt(xNode.InnerText, Encryption.CryptKey, Encryption.AuthKey, Encryption.SecMess);
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.TSS_GIVENNAME, givenName) { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
            }
           

            //TSS UUID COOKIE
            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = '" + OWNER_PREFIX + "UUID']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.TSS_UUID, Encryption.SimpleEncrypt(xNode.InnerText, Encryption.CryptKey, Encryption.AuthKey, Encryption.SecMess)));
            }

            //THROW ERROR IF USER DOES NOT BELONG TO A TENANCY CHAIN WITHIN
            if (RawList.Count == 0)
            {
                throw new Exception("The current user [" + mail + "] is not authorized to access TSS. There are no TSS roles within the user's tenancy chain.");
            }

            //SET HAS RUN COOKIE
            HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.HASRUN, "true") { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
        }
        private static string Decode64Bit(string rawSamlData)
        {
            byte[] samlData = Convert.FromBase64String(rawSamlData);
            string samlAssertion = Encoding.UTF8.GetString(samlData);
            return samlAssertion;
        }
    }
}
