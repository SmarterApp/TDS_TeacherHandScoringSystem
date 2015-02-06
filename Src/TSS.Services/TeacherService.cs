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
using System.Threading.Tasks;
using TSS.Domain;
using TSS.Data;
using System.Configuration;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUserManagementApi _userManagementApi;
        private static readonly Object locker = new Object();

        /// <summary>
        /// Provide a server-level caching mechanism for teacher data.
        /// </summary>
        private static Dictionary<string, TeacherCache> _cache = new Dictionary<string, TeacherCache>();

        /// <summary>
        /// Get the cached value from the teacher role cache.  Return null if doesn't exists.
        /// As a side-effect. remove all the expired values.  We return the data for expired keys
        /// anyway, thinking that the expiration should be less than the actual.
        /// </summary>
        /// <param name="institution"></param>
        /// <returns></returns>
        private static TeacherResult GetCachedValue(string institution)
        {
            TeacherResult rv = null;
            lock (locker)
            {
                //CLEAN UP CACHE
                List<string> keysToRemove = new List<string>();
                foreach (KeyValuePair<string, TeacherCache> pair in _cache)
                {
                    if (pair.Value.Expires < DateTime.Now)
                    {
                        keysToRemove.Add(pair.Key);
                    }
                    if (pair.Key == institution)
                    {
                        rv = pair.Value.Data;
                    }
                }

                foreach (string key in keysToRemove)
                {
                    _cache.Remove(key);
                }
            }
            return rv;
        }

        static readonly double CacheExpiration = double.Parse(ConfigurationManager.AppSettings["ART_SCORER_DATA_CACHING_MINS"]);
        private static void SetCachedValue(string institution, TeacherResult value)
        {
            lock (locker)
            {
                TeacherCache entry = new TeacherCache()
                                         {
                                             Expires = DateTime.Now.AddMinutes(CacheExpiration),
                                             Data = value
                                         };
                _cache.Add(institution, entry);
            }
        }
 
        private class TeacherCache
        {
            public DateTime Expires
            {get;set;}
            public TeacherResult Data
            {get;set;}

            public TeacherCache()
            {
            }
        }

        public TeacherService( IUserManagementApi userManagementApi)
        {
            _userManagementApi = userManagementApi;
        }
      
        public List<SearchResults> GetListOfPossibleScorers(List<TenancyChain> tens, List<RoleSet> roles)
        {
            // TSS.Data.DataDistribution.ISessionConnector sesssionObj = new SessionConnector("9999");
            // _loggerService = new LoggerService(new LoggerRepository());
            HashSet<SearchResults> allScorers = new HashSet<SearchResults>();
            
            foreach (TenancyChain t in tens)
            {
                foreach (RoleSet rs in roles)
                {
                    string key = rs.role + t.EntityId + t.Entity.ToString() + t.State;
                    TeacherResult teachers = GetCachedValue(key);
                    if (teachers == null)
                    {
                        teachers = _userManagementApi.GetTeachersFromApi(0, 10000, rs.role, t.EntityId, t.Entity.ToString(), t.State);

                        if (teachers.LoginFailed)
                        {
                            LoggerRepository.SaveLog(new Log
                                                       {
                                                           Category = LogCategory.Application,
                                                           Level = LogLevel.Warning,
                                                           Message = string.Format("ART_API"),
                                                           Details = "LoginFailed: TSS was not able to connect to the ART service. key:" + key + ",role:" + rs.role + ",entityid:" + t.EntityId + ", entity:" + t.Entity.ToString() + "state:" + t.State + "; teacher error"+ teachers.Error
                                                       });
                           //  throw new Exception("LoginFailed: TSS was not able to connect to the ART service. key:" + key + ",role:" + rs.role + ",entityid:" + t.EntityId+", entity:"+ t.Entity.ToString() + "state:" + t.State, teachers.Error);
                        }

                        if (teachers.Error != null)
                        {
                           //  throw new Exception(" Error:TSS was an error connecting to the ART service. key:" + key, teachers.Error);
                            LoggerRepository.SaveLog(new Log
                                                       {
                                                           Category = LogCategory.Application,
                                                           Level = LogLevel.Warning,
                                                           Message = string.Format("ART_API"),
                                                           Details = "Error:TSS was an error connecting to the ART service. key:" + key + "; teacher error"+ teachers.Error
                                                       });
                        }

                        SetCachedValue(key, teachers);
                    }

                    if (teachers.returnCount > 0)
                    {
                        for (int i = 0; i < teachers.searchResults.Length; i++)
                        {
                            if (!allScorers.Contains(teachers.searchResults[i]))
                            {
                                allScorers.Add(teachers.searchResults[i]);
                            }
                        }
                    }

                }
            }


            return allScorers.ToList<SearchResults>();

        }
        
    }
}
