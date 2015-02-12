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

        public static bool UseEmailAsUuid
        {
            get { return (bool.Parse(ConfigurationManager.AppSettings["EMAIL_AS_UUID"])); }
        }

        public static List<string> TeacherUUIDListCache
        {
            get
            {
                if (HttpContext.Current.Session["TeacherUUIDListCache"] == null)
                {
                    List<RoleSet> roles = UserAttributes.SAML.GetListOfRolesThatCanViewItems();
                    List<TenancyChain> t = UserAttributes.SAML.TenancyChainList.Where(x => UserAttributes.CanRoleViewAll(x)).ToList<TenancyChain>();
                    TeacherService _teacherService = new TeacherService(new UserManagementApi());
                    var r = _teacherService.GetListOfPossibleScorers(t, roles);
                    //ADD USER TO MAKE SURE THEY ARE IN THE MIX
                    r.Add(new SearchResults() { username = UserAttributes.SAML.TSSUserID , email = UserAttributes.SAML.Mail, firstName = UserAttributes.SAML.FirstName, lastName = UserAttributes.SAML.LastName});
                    HttpContext.Current.Session["TeacherUUIDListCache"] = UseEmailAsUuid ?
                    (r.Select(result => result.email).ToList()) :
                    (r.Select(result => result.username).ToList());
                }
                return (List<string>)HttpContext.Current.Session["TeacherUUIDListCache"];
            }
        }
        private const string SAMLRESPONSE = "SAMLResponse";
        private const string USER_ATTRIBUTES = "USER_ATTRIBUTES";
        private string OWNER_PREFIX
        {
            get
            {

                return ConfigurationManager.AppSettings["SAML_OWNER_PREFIX"].ToString();

            }
        }
        /// <summary>
        /// current user's unique identifier(loaded from SAML response), which should match assignment's teacherid
        /// </summary>
        public string TSSUserID
        {
            get {
                if (UseEmailAsUuid) //RETURN MAIL
                    return this.Mail;
                else //RETURN UUID
                    return this.sbacUUID;
            }
        
        }
        private static UserPermissions permissions;
        public static UserPermissions Permissions
        {
            get
            {
                if (permissions == null)
                {
                    TSS.Data.UserManagementApi userManagement = new UserManagementApi();
                    TSS.Services.UserService userService = new UserService(userManagement);
                    permissions = userService.GetUserPermissions();

                    userManagement = null;
                    userService = null;
                }
                return permissions;
            }
        }
        public string DecodedSAML { get; set; }
        public string EncodedeSAML { get; set; }
        public string Audience { get; set; }
        public string SubjectNameID { get; set; }
        public string FirstName { get; set; }
        public const string TSS_MAIL = "TSS-MAIL";
        public const string TSS_UUID = "TSS-UUID";
        public string Mail
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.Get(TSS_MAIL) != null)
                    return Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get(TSS_MAIL).Value, CryptKey, AuthKey, SecMess.Length);
                else
                    return "";
            }

        }
        public string LastName { get; set; }
        public bool AuthenticationStatus { get; set; }
        public string Issuer { get; set; }
        public string Destination { get; set; }
        public string ResponseID { get; set; }
        public bool VerifiedResponse { get; set; }
        public string SignatureValue { get; set; }
        public string SignatureReferenceDigestValue { get; set; }
        public DateTime AutheticationTime { get; set; }
        public string AuthenticationSession { get; set; }
        public const string ACTIVE_DISTRICT = "ACTIVE_DISTRICT";
        public string ActiveDistrictId
        {
            get
            {
                //IF NO COOKIE SET RETURN FIRST TENANCY
                if (HttpContext.Current.Request.Cookies.Get(ACTIVE_DISTRICT) == null)
                {
                    string userId = UserAttributes.SAML.TenancyChainList[0]
                        .DistrictID;
                    HttpContext.Current.Response.Cookies.Set(new HttpCookie(ACTIVE_DISTRICT,userId));
                    new TestImportRepository().UpdateTeacherDistrictRelationship(this.sbacUUID, userId);
                }

                return HttpContext.Current.Request.Cookies.Get(ACTIVE_DISTRICT).Value; 
                
            }
            set
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(ACTIVE_DISTRICT, value));
            }
        }
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
                    for (int j = 0; j < UserAttributes.Permissions.roleSet.Length; j++)
                    {
                        if (bool.Parse(ConfigurationManager.AppSettings["IGNORE_TENANCY_CHAINS"]))
                        {
                            RList.Add(newTC);
                        }
                        else
                        {
                            if (UserAttributes.Permissions.roleSet[j].role == newTC.Name)
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
        public string SN { get; set; }
        public string CN { get; set; }
        public string sbacUUID
        {
            get
            {

                if (HttpContext.Current.Request.Cookies.Get(TSS_UUID) != null)
                    return Encryption.SimpleDecrypt(HttpContext.Current.Request.Cookies.Get(TSS_UUID).Value, CryptKey, AuthKey, SecMess.Length);
                else
                    return "";
            }
        }
        public string GivenName { get; set; }
        public List<TenancyChain> Clients
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.CLIENT).ToList();
            }

        }
        public List<TenancyChain> GroupOfStates
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFSTATES).ToList();
            }

        }
        public List<TenancyChain> GroupofDistricts
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFDISTRICTS).ToList();
            }

        }
        public List<TenancyChain> GroupofInstitutions
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.GROUPOFINSTITUTIONS).ToList();
            }

        }
        public List<TenancyChain> Institutions
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.INSTITUTION).ToList();
            }

        }
        public List<TenancyChain> Districs
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.DISTRICT).ToList();
            }
        }
        public List<TenancyChain> States
        {
            get
            {
                return UserAttributes.SAML.TenancyChainList.Where(x => x.Entity == TenancyChain.EntityType.STATE).ToList();
            }
        }
        public TenancyChain HighestEntity
        {
            get
            {
                for (int i = 0; i < 7; i++)
                {
                    foreach (TenancyChain t in UserAttributes.SAML.TenancyChainList)
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
        public bool IsAdministrator
        {
            get
            {

                foreach (TenancyChain item in UserAttributes.SAML.TenancyChainList)
                {
                    if (UserAttributes.CanRoleViewAll(item) || item.Role.ToString().ToLower() == "administrator")
                        return true;
                }
                return false;
            }
        }
        public static string RawSAMLDocument
        {
            get
            {
                try
                {
                    return HttpContext.Current.Session[SAMLRESPONSE].ToString();
                }
                catch
                {
                    throw new Exception("Error Code: 5000 Message: SAML Response is null");
                }
            }
        }
        public static UserAttributes SAML
        {
            get
            {
                //TODO: USER COOKIE INSTEAD
                if (HttpContext.Current.Session[USER_ATTRIBUTES] == null)
                {
                    UserAttributes ua = new UserAttributes();
                    HttpContext.Current.Session[USER_ATTRIBUTES] = ua;
                }
                return (UserAttributes)HttpContext.Current.Session[USER_ATTRIBUTES];
            }
        }
        private UserAttributes()
        {

            bool HasRun = HttpContext.Current.Request.Cookies.Get("HASRUN") != null;

            if (!HasRun)
            {
                this.SamlParser(UserAttributes.RawSAMLDocument);
            }

        }
        public static bool CanRoleViewAll(TenancyChain tenancy)
        {
            string role = tenancy.Role;
            var roleSet = UserAttributes.Permissions.roleSet;
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
            var roleSet = UserAttributes.Permissions.roleSet;
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
            XmlNode xNode = xDoc.SelectSingleNode("/samlp:Response/samlp:Status/samlp:StatusCode/@Value", xMan);
            if (xNode != null)
            {
                this.AuthenticationStatus = false;
                string statusCode = xNode.Value;
                if (statusCode.EndsWith("status:Success"))
                {
                    this.AuthenticationStatus = true;
                }

            }



            XmlNodeList xNodeList;
            xNodeList = xDoc.SelectNodes("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = '" + OWNER_PREFIX + "TenancyChain']/saml:AttributeValue", xMan);
            if (xNodeList != null)
            {

                //SAVE TENANCY LIST TO COOKIES
                for (int i = 0; i < xNodeList.Count; i++)
                {
                    HttpContext.Current.Response.Cookies.Set(new HttpCookie("_" + i.ToString(), Encryption.SimpleEncrypt(xNodeList[i].InnerText.ToString(), CryptKey, AuthKey, SecMess)) { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
                }


            }

            xNode = xDoc.SelectSingleNode("/samlp:Response/@Destination", xMan);
            if (xNode != null)
            {
                this.Destination = xNode.Value;
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/@IssueInstant", xMan);
            if (xNode != null)
            {
                this.AutheticationTime = Convert.ToDateTime(xNode.Value);
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/@ID", xMan);
            if (xNode != null)
            {
                this.ResponseID = xNode.Value;
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Issuer", xMan);
            if (xNode != null)
            {
                this.Issuer = xNode.InnerText;
            }

            xNode = xDoc.SelectSingleNode("/samlp:Response/ds:Signature/ds:SignedInfo/ds:Reference/ds:DigestValue", xMan);
            if (xNode != null)
            {
                this.SignatureReferenceDigestValue = xNode.InnerText;
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/ds:Signature/ds:SignatureValue", xMan);
            if (xNode != null)
            {
                this.SignatureValue = xNode.InnerText;
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/@ID", xMan);
            if (xNode != null)
            {
                this.AuthenticationSession = xNode.Value;
            }

            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Subject/saml:NameID", xMan);
            if (xNode != null)
            {
                this.SubjectNameID = xNode.InnerText;
            }
            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:Conditions/saml:AudienceRestriction/saml:Audience", xMan);
            if (xNode != null)
            {
                this.Audience = xNode.InnerText;
            }


            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'givenName']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                this.GivenName = xNode.InnerText;
            }


            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'mail']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(TSS_MAIL, Encryption.SimpleEncrypt(xNode.InnerText, CryptKey, AuthKey, SecMess)) { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
            }


            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'cn']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                this.CN = xNode.InnerText;
            }

            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = 'sn']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                this.SN = xNode.InnerText;
            }

            xNode = xDoc.SelectSingleNode("/samlp:Response/saml:Assertion/saml:AttributeStatement/saml:Attribute[@Name = '" + OWNER_PREFIX + "UUID']/saml:AttributeValue", xMan);
            if (xNode != null)
            {
                HttpContext.Current.Response.Cookies.Set(new HttpCookie(TSS_UUID, Encryption.SimpleEncrypt(xNode.InnerText, CryptKey, AuthKey, SecMess)));
            }

            //THROW ERROR IF USER DOES NOT BELONG TO A TENANCY CHAIN WITHIN
            if (TenancyChainList.Count == 0)
            {
                throw new Exception("The current user [" + this.Mail + "] is not authorized to access TSS. There are no TSS roles within the user's tenancy chain.");
            }

            //SET HAS RUN COOKIE
            HttpContext.Current.Response.Cookies.Set(new HttpCookie("HASRUN", "true") { Expires = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["COOKIE_TIMEOUT_MINS"].ToString())) });
        }
        private static string Decode64Bit(string rawSamlData)
        {
            byte[] samlData = Convert.FromBase64String(rawSamlData);
            string samlAssertion = Encoding.UTF8.GetString(samlData);
            return samlAssertion;
        }





    }
}