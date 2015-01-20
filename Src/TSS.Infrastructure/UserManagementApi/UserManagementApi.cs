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
        private oAuthToken oAuth
        {
            get
            {
                if (_oauth == null)
                {
                    _oauth = GetAccessToken();
                }
                return _oauth;
            }

            set {

                _oauth = value;
            }

        }
        public UserManagementApi()
        { }
        public bool HasRun = false;



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

                //OAUTH - TOGGLE
                if (ConfigurationManager.AppSettings["ART_OAUTH_REQUIRED"].ToString().ToLower() == "true")
                {
                    if (oAuth.access_token == "ERROR")
                        throw new Exception("Error Code:3001  Message:oAuth Token Failed to load");
                    
                    request.Headers.Add("Authorization", string.Format("Bearer {0}", oAuth.access_token));
                }

                //GET DATA
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    return  SerializeData(response) ;
                }
            }
            catch (Exception e)
            {
                //TRY AGAIN
                if (!HasRun)
                {
                    //MARK HAS RUN
                    HasRun = true;
                    //RESET oAuth
                    oAuth = null;
                    //TRY AGAIN
                    return GetTeachersFromApi(pageNumber, pageSize, role, associatedEntityId, level, state);
                }
                else
                {
                    TeacherResult result = new TeacherResult();
                    result.LoginFailed = true;
                    result.Error = new Exception("Error Code: 3002", e);
                    HasRun = false;
                    return result;
                }
            }

        }
        

        private oAuthToken GetAccessToken()
        {

            var requestUrl = ConfigurationManager.AppSettings["ART_OAUTH_URL"].ToString();

            
            HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            string formData = "grant_type=password&username={0}&password={1}&client_id={2}&client_secret={3}";
            formData = string.Format(formData, ConfigurationManager.AppSettings["ART_OAUTH_USERNAME"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_PASSWORD"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_CLIENTID"],
                                               ConfigurationManager.AppSettings["ART_OAUTH_SECRET"]);

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
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(TeacherResult));
            object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
            TeacherResult jsonResponse = objResponse as TeacherResult;
            jsonResponse.LoginFailed = false;
            return jsonResponse;
        }

    }
}
