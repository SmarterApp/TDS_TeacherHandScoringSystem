﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace TSS.Data.DataDistribution
{
    /// <summary>
    /// Allow repository clients to access connection strings
    /// </summary>
    public class DataConnections
    {
        // Default string, used for most things.
        // Map of districts to local users.
        private static Dictionary<string, string> _districtLookUp;

        /// <summary>
        /// Return a map of districts to connection strings
        /// </summary>
        public static Dictionary<string,string> DistrictLookUp
        {
            get
            {
                return DataDistributionConfig.ConnectionStrings;
            }
        }

        /// <summary>
        /// Get the connection string to a particular district
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public static string GetConnectionString(string districtId)
        {
            if (DistrictLookUp.ContainsKey(districtId))
            {
                return DistrictLookUp[districtId];
            }
            return DefaultConnectionString;
        }

        /// <summary>
        /// Get the default connection string for this system.
        /// </summary>
        public static string DefaultConnectionString
        {
            get
            {
                return DataDistributionConfig.DefaultConnectionString;
            }
        }      
    }
}
