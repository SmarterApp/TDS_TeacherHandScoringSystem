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
using System.Web;
using System.Xml;
using System.Text;
using System.IO;
using SAML.Common;
using TSS.Services;
using TSS.Domain;
using TSS.Data;
using System.Configuration;

namespace TSS.MVC
{
    public class UserAttributes
    {
        public const string CryptKey = "45AC7AE85F63E668";
        public const string AuthKey = "B262ACF431AD96F4";
        public const string SecMess = "";

        public bool UseEmailAsUuid
        {
            get { return (bool.Parse(ConfigurationManager.AppSettings["EMAIL_AS_UUID"])); }
        }
        //read from fedlet cookie
        public List<TenancyChain> TenancyChainList
        {
            get
            {
                //initiat lists
                List<TenancyChain> RList = new List<TenancyChain>();
                List<String> RawList = new List<string>();
                bool keepDigging = true;
                int count = 0;
                //scan cookies and build raw list
                while (keepDigging)
                {
                    if (HttpContext.Current.Request.Cookies.Get("_" + count.ToString()) != null)
                    {
                        RawList.Add(Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get("_" + count.ToString()).Value, CryptKey, AuthKey, SecMess.Length));
                    }
                    else
                    {
                        keepDigging = false;
                    }
                    count++;
                }

                //loop over raw list
                for (int i = 0; i < RawList.Count; i++)
                {
                    TenancyChain newTC = new TenancyChain(RawList[i]);
                    for (int j = 0; j < UserManagementApi.UserPermissions.roleSet.Length; j++)
                    {
                        if (bool.Parse(ConfigurationManager.AppSettings["IGNORE_TENANCY_CHAINS"]))
                        {
                            RList.Add(newTC);
                        }
                        else
                        {
                            if (UserManagementApi.UserPermissions.roleSet[j].role == newTC.Name)
                            {
                                RList.Add(newTC);
                            }
                        }
                    }
                }
                //end loop over raw
                RawList = null;
                return RList;
            }
        }
        public string sbacUUID
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.Get(Config.TSS_UUID) != null)
                    return Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get(Config.TSS_UUID).Value, CryptKey, AuthKey, SecMess.Length);
                else
                    return "";
            }
        }
        public string Mail
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.Get(Config.TSS_MAIL) != null)
                    return Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get(Config.TSS_MAIL).Value, CryptKey, AuthKey, SecMess.Length);
                else
                    return "";
            }

        }
        public string GivenName
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.Get(Config.TSS_GIVENNAME) != null)
                    return Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get(Config.TSS_GIVENNAME).Value, CryptKey, AuthKey, SecMess.Length);
                else
                    return "";
            }
        }
        /// <summary>
        /// current user's unique identifier(loaded from SAML response), which should match assignment's teacherid
        /// </summary>
        public string TSSUserID
        {
            get
            {
                if (UseEmailAsUuid) //RETURN MAIL
                    return Mail;
                else //RETURN UUID
                    return sbacUUID;
            }

        }

        public List<string> TeacherUUIDListCache
        {
            get
            {
                    List<RoleSet> roles = GetListOfRolesThatCanViewItems();
                    List<TenancyChain> t = TenancyChainList.Where(x => this.CanRoleViewAll(x)).ToList<TenancyChain>();
                    TeacherService _teacherService = new TeacherService(new UserManagementApi());
                    var r = _teacherService.GetListOfPossibleScorers(t, roles);
                    //ADD USER TO MAKE SURE THEY ARE IN THE MIX
                    r.Add(new SearchResults() { username = this.TSSUserID , email = this.Mail});
                    return UseEmailAsUuid ?(r.Select(result => result.email).ToList()) :(r.Select(result => result.username).ToList());
               
            }
        }
     
        public bool IsAdministrator
        {
            get
            {

                foreach (TenancyChain item in TenancyChainList)
                {
                    if (CanRoleViewAll(item) || item.Role.ToString().ToLower() == "administrator")
                        return true;
                }
                return false;
            }
        }
        public string ActiveDistrictId
        {
            get
            {
                //IF NO COOKIE SET RETURN FIRST TENANCY
                if (HttpContext.Current.Request.Cookies.Get(Config.ACTIVE_DISTRICT) == null)
                {
                    string userId = this.TenancyChainList[0].DistrictID;
                    HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.ACTIVE_DISTRICT, userId));
                    new TestImportRepository().UpdateTeacherDistrictRelationship(sbacUUID, userId);
                }

                return HttpContext.Current.Request.Cookies.Get(Config.ACTIVE_DISTRICT).Value;

            }
            set
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(Config.ACTIVE_DISTRICT, value));
            }
        }
      
        public bool CanRoleViewAll(TenancyChain tenancy)
        {
            string role = tenancy.Role;
            var roleSet = UserManagementApi.UserPermissions.roleSet;
            for (int i = 0; i < roleSet.Length; i++)
            {
                if (roleSet[i].role.ToLower() == role.ToLower())
                {
                    for (int j = 0; j < roleSet[i].mappings.Length; j++)
                    {
                        for (int k = 0; k < roleSet[i].mappings[j].permissions.Length; k++)
                        {
                            if (roleSet[i].mappings[j].permissions[k].name == "Can See All Items")
                                return true;

                        }
                    }
                }
            }
            return false;
        }
        public List<RoleSet> GetListOfRolesThatCanViewItems()
        {
            var roleSet = UserManagementApi.UserPermissions.roleSet;
            HashSet<RoleSet> returnList = new HashSet<RoleSet>();

            for (int i = 0; i < roleSet.Length; i++)
            {
                for (int j = 0; j < roleSet[i].mappings.Length; j++)
                {
                    for (int k = 0; k < roleSet[i].mappings[j].permissions.Length; k++)
                    {
                        if (roleSet[i].mappings[j].permissions[k].name == "Can See Own Items"
                            || roleSet[i].mappings[j].permissions[k].name == "Can See All Items")
                        {
                            if (!returnList.Contains(roleSet[i]))
                                returnList.Add(roleSet[i]);
                        }
                    }
                }

            }

            return returnList.ToList<RoleSet>();
        }

/*
        public List<TenancyChain> Clients
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.CLIENT).ToList();
            }

        }
        public List<TenancyChain> GroupOfStates
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFSTATES).ToList();
            }

        }
        public List<TenancyChain> GroupofDistricts
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFDISTRICTS).ToList();
            }

        }
        public List<TenancyChain> GroupofInstitutions
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFINSTITUTIONS).ToList();
            }

        }
        public List<TenancyChain> Institutions
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.INSTITUTION).ToList();
            }

        }
        public List<TenancyChain> Districs
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.DISTRICT).ToList();
            }
        }
        public List<TenancyChain> States
        {
            get
            {
                return this.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.STATE).ToList();
            }
        }
        public TenancyChain HighestEntity
        {
            get
            {
                for (int i = 0; i < 7; i++)
                {
                    foreach (TenancyChain t in this.TenancyChainList)
                    {
                        if (i == 0 && t.Entity == TenancyChain.EntityType.CLIENT)
                        {
                            return t;
                        }
                        if (i == 1 && t.Entity == TenancyChain.EntityType.GROUPOFSTATES)
                        {
                            return t;
                        }

                        if (i == 2 && t.Entity == TenancyChain.EntityType.STATE)
                        {
                            return t;
                        }
                        if (i == 3 && t.Entity == TenancyChain.EntityType.GROUPOFDISTRICTS)
                        {
                            return t;
                        }
                        if (i == 4 && t.Entity == TenancyChain.EntityType.DISTRICT)
                        {
                            return t;
                        }
                        if (i == 5 && t.Entity == TenancyChain.EntityType.GROUPOFINSTITUTIONS)
                        {
                            return t;
                        }
                        if (i == 6 && t.Entity == TenancyChain.EntityType.INSTITUTION)
                        {
                            return t;
                        }
                    }
                }
                return null;
            }
        }


      



        */

    }
}