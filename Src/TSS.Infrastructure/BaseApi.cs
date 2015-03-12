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
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System;

namespace TSS.Data
{
    public class BaseApi
    {
        protected string RootUrl;

        protected BaseApi(string rootUrl)
        {
            RootUrl = rootUrl;
        }

        protected string GetUrl(string path)
        {
            return RootUrl + path;
        }

        protected HttpWebRequest CreateRequest(string path, string method = "GET",
                                        NetworkCredential credentials = null,
                                        IDictionary<string, string> headers = null)
        {

            string url = path;

            // if URL is a relative URL, append the root URL
            if (path.Substring(0, 4) != "http") url = GetUrl(path);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Timeout = 20000;

            if (headers != null)
            {
                foreach (var h in headers)
                {
                    request.Headers.Add(h.Key, h.Value);
                }
            }

            if (credentials != null) request.Credentials = credentials;

            return request;
        }


        protected string SendRequest(string path, string method = "GET",
                                    NameValueCollection data = null,
                                    NetworkCredential credentials = null)
        {
            HttpWebRequest request = CreateRequest(path, method, credentials, null);
            string postData = "";
            if (data != null)
            {
                postData = String.Join("&", Array.ConvertAll(data.AllKeys,
                    key => string.Format("{0}={1}", key, data[key])));
            }

            if (!String.IsNullOrEmpty(postData))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            return ExecuteRequest(request);
        }

        protected string ExecuteRequest(HttpWebRequest request)
        {
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string content = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)response;
            if (httpResponse.StatusCode != HttpStatusCode.OK && httpResponse.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception(httpResponse.StatusCode.ToString() + " error: " + content);
            }
            return content;
        }
    }
}

