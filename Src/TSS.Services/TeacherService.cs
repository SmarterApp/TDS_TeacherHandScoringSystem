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

        #region ART API data caching 
        /// <summary>
        /// Provide a server-level caching mechanism for teacher data.
        /// </summary>
        private static Dictionary<string, TeacherCache> _cache = new Dictionary<string, TeacherCache>();
        private static List<EntityCache> _entityCaches = new List<EntityCache>();
        static readonly double CacheExpiration = double.Parse(ConfigurationManager.AppSettings["ART_SCORER_DATA_CACHING_MINS"]);
        static readonly double EntityCacheExpiration = double.Parse(ConfigurationManager.AppSettings["ART_ENTITIES_DATA_CACHING_DAYS"]);
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
        private static List<EntityResult> GetEntitiesCachedValue(string entityKey, string entityType)
        {
            lock (locker)
            {
                foreach (EntityCache entry in _entityCaches)
                {
                    if (entry.EntityResults.SingleOrDefault(e=>e.entityId == entityKey) !=null)
                    {
                        if (entityType.ToUpper() == TenancyChain.EntityType.DISTRICT.ToString() && entry.EntityResults.SingleOrDefault(e=>e.entityType == TenancyChain.EntityType.STATE.ToString()) !=null)
                            continue;
                        return entry.EntityResults;
                    }
                    if (entry.Expires < DateTime.Now)
                    {
                        _entityCaches.Remove(entry);
                    }
                }
            }
            return null;
        }
        private static void SetEntitiesCachedValue(List<EntityResult> entityResults)
        {
            lock (locker)
            {
                EntityCache entry = new EntityCache()
                {
                    Expires = DateTime.Now.AddDays(EntityCacheExpiration),
                    EntityResults = entityResults
                };
                _entityCaches.Add(entry);
            }
        }
        private class TeacherCache
        {
            public DateTime Expires{get;set;}
            public TeacherResult Data{get;set;}
        }
        private class EntityCache
        {
            public DateTime Expires { get; set; }
            public List<EntityResult> EntityResults { get; set; }
        }
        #endregion caching

        public TeacherService( IUserManagementApi userManagementApi)
        {
            _userManagementApi = userManagementApi;
        }
      
        public List<SearchResults> GetListOfPossibleScorers(List<TenancyChain> tens, List<RoleSet> roles)
        {
            HashSet<SearchResults> allScorers = new HashSet<SearchResults>();
            List<EntityResult> entityResults = new List<EntityResult>();
            foreach (TenancyChain t in tens)
            {
                if (t.Entity.ToString().ToUpper() == TenancyChain.EntityType.INSTITUTION.ToString())
                {
                    entityResults.Add(new EntityResult(){entityType = t.Entity.ToString(), entityId = t.EntityId});
                }
                else 
                    entityResults = GetAssociatedEntities(t);

                foreach (EntityResult entityResult in entityResults)
                {
                    foreach (RoleSet rs in roles)
                    {
                        string key = rs.role + entityResult.entityId + entityResult.entityType + t.State;
                        TeacherResult teachers = GetCachedValue(key);
                        if (teachers == null)
                        {
                            teachers = _userManagementApi.GetTeachersFromApi(0, 10000, rs.role, entityResult.entityId, entityResult.entityType, t.State);

                            if (teachers.LoginFailed)
                            {
                                LoggerRepository.SaveLog(new Log
                                                             {
                                                                 Category = LogCategory.Application,
                                                                 Level = LogLevel.Warning,
                                                                 Message = string.Format("ART_API"),
                                                                 Details = "LoginFailed: TSS was not able to connect to the ART service. key:" + key + ",role:" + rs.role + ",entityid:" + entityResult.entityId + ", entity:" + entityResult.entityType.ToString() + "state:" + t.State + "; teacher error" + teachers.Error
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
                                                                 Details = "Error:TSS was an error connecting to the ART service. key:" + key + "; teacher error" + teachers.Error
                                                             });
                            }

                            SetCachedValue(key, teachers);
                        }

                        if (teachers.returnCount > 0)
                        {
                            for (int i = 0; i < teachers.searchResults.Length; i++)
                            {
                                if (!allScorers.Any(s => s.username == teachers.searchResults[i].username && s.email == teachers.searchResults[i].email))
                                {
                                    allScorers.Add(teachers.searchResults[i]);
                                }
                            }
                        }

                    }
                }
            }


            return allScorers.ToList<SearchResults>();

        }

        private List<EntityResult> GetAssociatedEntities(TenancyChain t)
        {
            List<EntityResult> entityResults = new List<EntityResult>();
            //load all the entities
            //load current entity and its child entities from cache if cached
            var entitiesfromCache = GetEntitiesCachedValue(t.EntityId, t.Entity.ToString());
            if (entitiesfromCache != null) return entitiesfromCache;

            if (t.Entity.ToString().ToUpper() == TenancyChain.EntityType.STATE.ToString())
            {
                EntityResultSet districtEntityResultSet = _userManagementApi.GetEntitiesFromApi(TenancyChain.EntityType.DISTRICT.ToString(), TenancyChain.EntityType.STATE.ToString(), t.StateID);
                foreach (EntityResult entityResult in districtEntityResultSet.EntityResults)
                {
                    //load this district and its child entities from cache if cached
                    entitiesfromCache = GetEntitiesCachedValue(t.EntityId, t.Entity.ToString());
                    if (entitiesfromCache != null) 
                        entityResults.AddRange(entitiesfromCache);
                    else
                    {
                        //add the district
                        if (entityResults.SingleOrDefault(e => e.entityId == entityResult.entityId) == null)
                            entityResults.Add(entityResult);
                        //add the schools for this district
                        EntityResultSet schoolEntityResultSet = _userManagementApi.GetEntitiesFromApi(TenancyChain.EntityType.INSTITUTION.ToString(), entityResult.entityType, entityResult.entityId);
                        foreach (EntityResult schoolEntityResult in schoolEntityResultSet.EntityResults)
                        {
                            if (entityResults.SingleOrDefault(e => e.entityId == schoolEntityResult.entityId) == null)
                                entityResults.Add(schoolEntityResult);
                        }
                    }
                }
            }
            else
            {
                if (t.Entity.ToString().ToUpper() == TenancyChain.EntityType.DISTRICT.ToString())
                {
                    EntityResultSet entityResultSet = _userManagementApi.GetEntitiesFromApi(TenancyChain.EntityType.INSTITUTION.ToString(), TenancyChain.EntityType.DISTRICT.ToString(), t.DistrictID);
                    foreach (EntityResult schoolEntityResult in entityResultSet.EntityResults)
                    {
                        if (entityResults.SingleOrDefault(e => e.entityId == schoolEntityResult.entityId) == null)
                            entityResults.Add(schoolEntityResult);
                    }
                }
            }
            // add the current user entity if not present 
            if (entityResults.SingleOrDefault(e => e.entityId == t.EntityId) == null)
                entityResults.Add(new EntityResult(){entityType = t.Entity.ToString(), entityId = t.EntityId});

            //cache this given entity and its child entities
            SetEntitiesCachedValue(entityResults);
            return entityResults;
        }
    }
}

