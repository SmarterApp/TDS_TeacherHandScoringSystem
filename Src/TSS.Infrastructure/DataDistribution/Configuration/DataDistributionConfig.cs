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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace TSS.Data.DataDistribution
{
    /// <summary>
    /// Implement the config section that contains the list of connection strings
    /// and district mappings
    /// </summary>
    internal class DataDistributionConfig : ConfigurationSection
    {

        private static Dictionary<string, string> _connectionStrings;
        private static string _defaultConnectionString;
        private static readonly Object locker = new Object();

        /// <summary>
        /// Return the list of all districts to connection strings 
        /// </summary>
        public static Dictionary<string, string> ConnectionStrings
        {
            get
            {
                Dictionary<string, string> connectionStrings = null;
                Interlocked.Exchange(ref connectionStrings, _connectionStrings);
                if (connectionStrings == null)
                {
                    lock (locker)
                    {
                        if (_connectionStrings != null)
                        {
                            return _connectionStrings;
                        }
                        DataDistributionConfig _config =
                            ConfigurationManager.GetSection(DataDistributionConfig.SectionName) as
                            DataDistributionConfig;
                        _connectionStrings = new Dictionary<string, string>();
                        DataDistributionConfig dbConfig = _config;
                        foreach (DataStoreConnectionElement connectionInfo in dbConfig.DataStoreConnections)
                        {
                            string connection = connectionInfo.ConnectionString;
                            if (connectionInfo.IsDefault || _defaultConnectionString == null)
                            {
                                _defaultConnectionString = connection;
                            }
                            foreach (DistirctElement element in connectionInfo.Districts)
                            {
                                _connectionStrings.Add(element.Id, connection);
                            }
                        }
                        connectionStrings = _connectionStrings;
                    }
                }
                return connectionStrings;
            }
        }

        /// <summary>
        /// we always have a default connection string.  It is build when the mapping list is built.
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                if (ConnectionStrings != null)
                {
                    return _defaultConnectionString;
                }

                return "Error"; // this should not happen, side effect of ConnectionStrings attr 
            }
        }

        private const string SectionName = "TSSDataDistribution";
        private const string ConnectionStringsSection = "ConnectionStrings";
        private const string DistrictIdSection = "District";

        [ConfigurationProperty(ConnectionStringsSection)]
        [ConfigurationCollection(typeof(DataStoresCollection), AddItemName = "ConnectionString")]
        private DataStoresCollection DataStoreConnections { get { return (DataStoresCollection)base[ConnectionStringsSection]; } }

    }

    internal class DataStoresCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataStoreConnectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataStoreConnectionElement)element).Name;
        }
    }

    internal class DataStoreConnectionElement : ConfigurationElement
    {

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("connectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["connectionString"]; }
            set { this["connectionString"] = value; }
        }

        [ConfigurationProperty("default", IsRequired = false)]
        public bool IsDefault
        {
            get { return (bool)this["default"]; }
            set { this["default"] = value; }
        }


        [ConfigurationProperty("Districts",IsRequired=true)]
        [ConfigurationCollection(typeof(DistrictCollection), AddItemName = "add")]
        public DistrictCollection Districts { get { return (DistrictCollection)base["Districts"]; } }
        
    }

    internal class DistrictCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DistirctElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DistirctElement)element).Id;
        }
    }

    internal class DistirctElement : ConfigurationElement
    {

        [ConfigurationProperty("id", IsRequired = true)]
        public string Id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }
    }
}

