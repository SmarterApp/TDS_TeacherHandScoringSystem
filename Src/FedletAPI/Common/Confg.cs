using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAML.Common
{

    public static class ConfigMultiClient
    {
        private static Dictionary<string, ConfigSAML> configs = new Dictionary<string, ConfigSAML>();
        public static void Initialize(string clientName,
            string refreshSessionURL, string portal, string landingPage, string idealTimeout, string tokenCookieName,
            string usernameCookie, string configFolder, string keepAliveURL, bool forceReload)
        {
            if (!configs.ContainsKey(clientName))
            {
                ConfigSAML configSAML = new ConfigSAML();
                configSAML.Initialize(refreshSessionURL, portal, landingPage, idealTimeout, tokenCookieName, usernameCookie, configFolder, keepAliveURL, forceReload);
                configs.Add(clientName, configSAML);
            }
            else if (forceReload)
            {
                ConfigSAML configSAML = new ConfigSAML();
                configSAML.Initialize(refreshSessionURL, portal, landingPage, idealTimeout, tokenCookieName, usernameCookie, configFolder, keepAliveURL, forceReload);
                configs[clientName] = configSAML;
            }
        }

        public static ConfigSAML GetConfig(string clientName)
        {
            return configs[clientName];
        }
    }

    /// <summary>
    /// To be user for multi client system using same website
    /// </summary>
    public class ConfigSAML
    {
        public string Portal;
        public string LandingPage;
        public string TokenCookie;
        public string IdealTimeout;
        public string RefreshSessionURL;
        public string UsernameCookie;
        public string ConfigFolder;
        public string KeepAlive;
        private bool initialized = false;

        public void Initialize(
            string refreshSessionURL, string portal, string landingPage, string idealTimeout, string tokenCookieName,
            string usernameCookie, string configFolder, string keepAliveURL, bool forceReload)
        {
            if (!initialized)
            {
                RefreshSessionURL = refreshSessionURL;
                Portal = portal;
                IdealTimeout = idealTimeout;
                TokenCookie = tokenCookieName;
                UsernameCookie = usernameCookie;
                ConfigFolder = configFolder;
                initialized = true;
                KeepAlive = keepAliveURL;
                LandingPage = landingPage;
            }
        }
    }

    /// <summary>
    /// To be used for single client system
    /// </summary>
    public static class Config
    {
        public static string Portal;
        public static string LandingPage;
        public static string TokenCookie;
        public static string IdealTimeout;
        public static string RefreshSessionURL;
        public static string UsernameCookie;
        public static string ConfigFolder;
        public static string KeepAlive;
        private static bool initialized = false;
        

        //public static string RefreshSessionURL = "http://ssotest.airast.org:8080/auth/identity/attributes?attributenames=idletime&attributenames=maxidletime&attributenames=timeleft&attributenames=maxsessiontime&refresh=true";
        //public static string Portal = "http://tide.uat.airast.org/samlDemo/portal.aspx";
        //public static string IdealTimeout = "6000";
        //public static string TokenCookie = "iPlanetDirectoryPro";
        //public static string LogoutURL = "http://ssotest.airast.org:8080/auth/UI/Logout?goto=" + Portal;
        //public static string UsernameCookie = "Username";
        //public static string ConfigFolder = @"D:\inetpub\tide_uat_airast_org\samlDemo\App_Data\";
        ////C:\Projects\PreProcessing\CLS\OpenAM\OpenAMClient\FedletSample\App_Data\


        public static void Initialize(string refreshSessionURL, string portal, string landingPage, string idealTimeout, string tokenCookieName, string usernameCookie, string configFolder, string keepAliveURL)
        {
            if (!initialized)
            {
                RefreshSessionURL = refreshSessionURL;
                Portal = portal;
                IdealTimeout = idealTimeout;
                TokenCookie = tokenCookieName;
                UsernameCookie = usernameCookie;
                ConfigFolder = configFolder;
                initialized = true;
                KeepAlive = keepAliveURL;
                LandingPage = landingPage;
            }
        }
    }
}
