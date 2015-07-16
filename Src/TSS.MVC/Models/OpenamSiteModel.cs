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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using TSS.Domain;

namespace TSS.MVC.Models
{
    /**
     * Get the data that describes the other openAM sites we can navigate to
     * **/
    public class OpenamSiteModel
    {
        // Create threadsafe globalton container in the usual way...
        private class OpenamSitesContainer
        {
            public List<OpenAmSite> Sites { get; set; }
        }
        private static OpenamSitesContainer _container = null;

        // Parse the XML file.  Currently this is a file, later this will be modified to read from a
        // web service.
        private static void ParseXmlConfig()
        {
            OpenamSitesContainer container = new OpenamSitesContainer();
            container.Sites = new List<OpenAmSite>();
            List<OpenAmSite> sites = container.Sites;
            String client = ConfigurationManager.AppSettings["ART_API_CLIENT"];

            /* this will be replaced with web service some fine day */
            XmlDocument doc = new XmlDocument();
            doc.Load(HostingEnvironment.ApplicationPhysicalPath + "\\App_Data\\OpenAmSites.xml");
            /*    */
            using (XmlNodeList contentNodes = doc.GetElementsByTagName("Content"))
            {
                foreach (XmlNode node in contentNodes)
                {
                    XmlAttribute id = node.Attributes != null ? node.Attributes["ForState"] : null ;
                    if (id == null || id.Value != client)
                    {
                        continue;
                    }
                    // Parse the node for our client
                    using (XmlNodeList siteNodes = node.SelectNodes("ClientSites/ClientSite"))
                    {
                        if (siteNodes == null)
                        {
                            break;
                        }
                        foreach (XmlNode site in siteNodes)
                        {
                            XmlNode description = site.SelectSingleNode("Description");
                            XmlNode url = site.SelectSingleNode("Url");
                            XmlNode clientId = site.SelectSingleNode("ClientID");

                            if (description == null || url == null || clientId == null)
                            {
                                break;
                            }
                            sites.Add(new OpenAmSite{ClientID=clientId.InnerText,
                                Description=description.InnerText,
                                Url = url.InnerText});
                        }
                    }
                }
            }
            // Set global object 
            Interlocked.Exchange(ref _container,container);
        }

        // Get the global data one time exactly
        private static IEnumerable<OpenAmSite> GetSites()
        {
            if (_container == null)
            {
                ParseXmlConfig();
            }
            return _container.Sites;
        }

        public List<SelectListItem> DropdownInfo { get; set; }

        public OpenamSiteModel()
        {
            IEnumerable<OpenAmSite> sites = GetSites();

            DropdownInfo = sites.Select(site => new SelectListItem { Value = site.Url, Text = site.Description }).ToList();

            DropdownInfo.Insert(0, new SelectListItem { Value = "-1", Text = string.Format("Navigate to another application") });
        }
    }
}
