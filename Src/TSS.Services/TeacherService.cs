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
        private static Dictionary<string, TeacherCache> _cache;
        private static Dictionary<string, TeacherCache> Cache
        {
            get
            {
                if (_cache == null)
                    _cache = new Dictionary<string, TeacherCache>();

                //CLEAN UP CACHE
                List<string> keysToRemove = new List<string>();
                //FIND EXPIRED CACHES
                foreach (KeyValuePair<string, TeacherCache> c in _cache)
                    if (c.Value.Expires < DateTime.Now)
                        keysToRemove.Add(c.Key);
                //REMOVE EXPIRED CACHES
                for (int i = 0; i < keysToRemove.Count; i++)
                    _cache.Remove(keysToRemove[i]);

                return _cache;
            }
        }
        private class TeacherCache
        {
            public DateTime Expires
            {get;set;}
            public TeacherResult Data
            {get;set;}
        
        
        }

        public TeacherService( IUserManagementApi userManagementApi)
        {
            _userManagementApi = userManagementApi;
        }
      
		public TeacherResult GetTeachersFromApi(int pageNumber, int pageSize, string role, string associatedEntityId, string level, string state)
        {
            
            var Teachers = _userManagementApi.GetTeachersFromApi(pageNumber, pageSize, role, associatedEntityId, level, state);

            return Teachers;

        }
        public List<SearchResults> GetListOfPossibleScorers(List<TenancyChain> tens, List<RoleSet> roles)
        {
           
            HashSet<SearchResults> allScorers = new HashSet<SearchResults>();
            

            foreach (TenancyChain t in tens)
            {
                foreach (RoleSet rs in roles)
                {
                    TeacherResult teachers;
                    string key = rs.role + t.EntityId + t.Entity.ToString() + t.State;
                    if (!Cache.ContainsKey(key) || Cache[key].Expires < DateTime.Now)
                    {
                        teachers = _userManagementApi.GetTeachersFromApi(0, 10000, rs.role, t.EntityId, t.Entity.ToString(), t.State);

                        if (teachers.LoginFailed)
                        {
                            throw new Exception("LoginFailed: TSS was not able to connect to the ART service. key:" + key + ",role:" + rs.role + ",entityid:" + t.EntityId+", entity:"+ t.Entity.ToString() + "state:" + t.State, teachers.Error);
                        }

                        if (teachers.Error != null)
                        {
                            throw new Exception(" Error:TSS was an error connecting to the ART service. key:" + key, teachers.Error);
                        }

                        double expiration = double.Parse(ConfigurationManager.AppSettings["ART_SCORER_DATA_CACHING_MINS"].ToString());
                        Cache.Add(key, new TeacherCache(){
                                                            Data = teachers,
                                                            Expires = DateTime.Now.AddMinutes(expiration)                            
                                                        });
                    }
                    else
                    {
                        teachers = Cache[key].Data;
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
