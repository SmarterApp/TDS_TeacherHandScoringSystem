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
using System.Threading;
using TSS.Domain;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Text;
using System.IO;
using System.Configuration;

namespace TSS.Data
{
    
    public class UserManagementApi : IUserManagementApi
    {
        private static oAuthToken _oauth;
        private static readonly Object locker = new Object();

        // This object has a side-effect - get my result in fetch from ART server
        private static oAuthToken oAuth
        {
            get
            {
                oAuthToken safeToke = null;
                Interlocked.Exchange(ref safeToke, _oauth);
                if (safeToke == null || safeToke.Error())
                {
                    _oauth = null;
                    lock (locker)
                    {
                        // Another thread may have set the lock first
                        if (_oauth != null)
                        {
                            return _oauth;
                        }
                        safeToke = GetAccessToken();
                        _oauth = safeToke;
                    }
                }
                return safeToke;
            }            
        }
        private static oAuthToken RefreshToken()
        {
            // Force error condition and deep fetch
            lock (locker)
            {
                if (_oauth != null)
                {
                    _oauth.SetError();
                }
            }
            return  oAuth;
        }
       
        public bool HasRun = false;

        /// <summary>
        /// load all child entities for a given parent entityid, for instance, if given a stateid, 
        /// it returns the district entities or if given a distirctid, it returns the institution entities.
        /// </summary>
        /// <param name="entityType">DISTRICT/INSTITUTION</param>
        /// <param name="parentEntityType">STATE/DISTRICT</param>
        /// <param name="parentEntityId">DS1/SC12</param>
        /// <returns></returns>
        public EntityResultSet GetEntitiesFromApi(string entityType, string parentEntityType, string parentEntityId)
        {
            string requestEntityApiUrl = ConfigurationManager.AppSettings["ART_API_REST_API_BASE_URL"].ToString() +
                                         string.Format("/{0}?{1}Id={2}",
                                                       HttpUtility.UrlEncode(entityType.ToLower()),
                                                       HttpUtility.UrlEncode(parentEntityType.ToLower()),
                                                       HttpUtility.UrlEncode(parentEntityId));

            HttpWebRequest request = WebRequest.Create(requestEntityApiUrl) as HttpWebRequest;
            request.ContentType = "application/json";
            request.AllowAutoRedirect = false;
            //GET DATA
            try
            {
                if (ConfigurationManager.AppSettings["ART_OAUTH_REQUIRED"].ToString().ToLower() == "true")
                {
                    if (oAuth.Error())
                        throw new Exception("Error Code:3001  Message:oAuth Token Failed to load");

                    request.Headers.Add("Authorization", string.Format("Bearer {0}", oAuth.access_token));
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(EntityResult[]));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    EntityResult[] jsonResponse = objResponse as EntityResult[];
                    EntityResultSet result = new EntityResultSet();
                    if (jsonResponse != null)
                    {
                        result.EntityResults = jsonResponse;
                        result.LoginFailed = false;
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                EntityResultSet result = new EntityResultSet();
                result.LoginFailed = true;
                result.Error = new Exception("Error Code: 3004", e);
                return result;
            }

        }

        // public static int errMod = 0;
        public TeacherResult GetTeachersFromApi(int pageNumber, int pageSize, string role, string associatedEntityId, string level, string state)
        {
            string requestUrl = ConfigurationManager.AppSettings["ART_API_URL"].ToString()  
                + string.Format("?currentPage={0}&pageSize={1}&role={2}&associatedEntityId={3}&ClientID={4}&level={5}&state={6}",
                                               pageNumber,
                                               pageSize,
                                               HttpUtility.UrlEncode(role),
                                               HttpUtility.UrlEncode(associatedEntityId),
                                               HttpUtility.UrlEncode(ConfigurationManager.AppSettings["ART_API_CLIENT"].ToString()),
                                               HttpUtility.UrlEncode(level),
                                               HttpUtility.UrlEncode(state));
            

            try
            {

                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.ContentType = "application/json";
                request.AllowAutoRedirect = false;


                /* errMod = errMod + 1;  // Debugging code for the concurrency feature.
                if (errMod%4 == 3)
                {
                    oAuth.access_token = "xxxxxxx";
                }  */
                //OAUTH - TOGGLE
                if (ConfigurationManager.AppSettings["ART_OAUTH_REQUIRED"].ToString().ToLower() == "true")
                {
                    if (oAuth.Error())
                        throw new Exception("Error Code:3001  Message:oAuth Token Failed to load");
                    
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", oAuth.access_token));
                }

                //GET DATA
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    return SerializeData(response);
                }
            }
            catch (WebException e)
            {
                //TRY AGAIN
                if (!HasRun)
                {
                    //MARK HAS RUN
                    HasRun = true;
                    //RESET oAuth
                    //TRY AGAIN

                    RefreshToken();
                    return GetTeachersFromApi(pageNumber, pageSize, role, associatedEntityId, level, state);
                }
                else
                {
                    using (Stream stream = e.Response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        String responseString = reader.ReadToEnd();
                    

                    TeacherResult result = new TeacherResult();
                    result.LoginFailed = true;
                    result.Error = new Exception("Error Code: 3002; " + responseString , e);
                    HasRun = false;
                    return result;
                    }
                }
            }

        }

        private static oAuthToken GetAccessToken()
        {

            var requestUrl = ConfigurationManager.AppSettings["ART_OAUTH_URL"].ToString();

            
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string formData = string.Empty;
            if (ConfigurationManager.AppSettings["ART_OAUTH_PASSWORD_GRANTTYPE"] == null ||
                   ConfigurationManager.AppSettings["ART_OAUTH_PASSWORD_GRANTTYPE"].ToString().ToLower() == "true")
            {
                formData = "grant_type=password&username={0}&password={1}&client_id={2}&client_secret={3}";
                formData = string.Format(formData, ConfigurationManager.AppSettings["ART_OAUTH_USERNAME"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_PASSWORD"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_CLIENTID"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_SECRET"]);

            }
            else
            {
                formData = "grant_type=client_credentials&client_id={0}&client_secret={1}";
                formData = string.Format(formData, ConfigurationManager.AppSettings["ART_OAUTH_CLIENTID"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_SECRET"]);

            }
            
            System.Diagnostics.Debug.WriteLine("Sending art request: " + formData);
            byte[] bytedata = Encoding.UTF8.GetBytes(formData);
            request.ContentLength = bytedata.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(oAuthToken));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    oAuthToken jsonResponse = objResponse as oAuthToken;
                    System.Diagnostics.Debug.WriteLine("Token received, exp:{0} scope {1} access {2} type{3} ",
                        jsonResponse.expires_in,
                        jsonResponse.scope,
                        jsonResponse.access_token,
                        jsonResponse.token_type);

                    return jsonResponse;
                }
            }
            catch (Exception e)
            {

                oAuthToken oa = new oAuthToken();
                oa.access_token = "ERROR";
                return oa;
            }

        }

        public UserPermissions GetUserPermissions()
        {
            string isLoadFromLocal = ConfigurationManager.AppSettings["LOAD_PERMISSIONS_FROM_LOCAL"];

            try
            {
                DataContractJsonSerializer jsonSerializer =
                    new DataContractJsonSerializer(typeof (UserPermissions));
                object objResponse = null;
                if (Convert.ToBoolean(isLoadFromLocal))
                {
                    string jsonDataFile = HttpContext.Current.Server.MapPath("~/App_Data/role.json");
                    objResponse =
                        jsonSerializer.ReadObject(
                            new MemoryStream(Encoding.UTF8.GetBytes(File.ReadAllText(jsonDataFile))));
                }
                else
                {
                    string requestUrl = ConfigurationManager.AppSettings["PERMISSIONS_SCHEMA_URL"];
                    HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                    if (request != null)
                        using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                        {
                            objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                        }
                }
                UserPermissions jsonResponse = objResponse as UserPermissions;
                if (jsonResponse != null)
                {
                    jsonResponse.LoginFailed = false;
                }
                return jsonResponse;
                
            }
            catch (Exception e)
            {

                UserPermissions result = new UserPermissions();
                result.LoginFailed = true;
                result.Error = new Exception("Error Code: 3003", e);
                return result;
            }

        }
         
        public TeacherResult SerializeData(HttpWebResponse response)
        {
            TeacherResult result = new TeacherResult();

            if (response == null)
            {
                result.LoginFailed = true;
                result.Error = new Exception("response is null");
                HasRun = true;
                return result;
            }
            
            Stream responseStream = response.GetResponseStream();
            Stream newStream = new MemoryStream();
            if (responseStream != null)
            {
                responseStream.CopyTo(newStream);
                using (StreamReader sr = new StreamReader(responseStream))
                {
                    string responseText = sr.ReadToEnd();
                    if (responseText.Contains("{\"Error\":"))
                    {
                        result.LoginFailed = false;
                        result.Error = new Exception(responseText);
                        HasRun = true;
                        return result;
                    }
                }
                newStream.Position = 0;
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TeacherResult));
                object objResponse = jsonSerializer.ReadObject(newStream);
                result = objResponse as TeacherResult;
                if (result != null) result.LoginFailed = false;
            }
            return result;
        }
    }
}

