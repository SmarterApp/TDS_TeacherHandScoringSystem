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
using System.Threading;
using System.Web;
using TSS.Data.DataDistribution;

namespace TSS.MVC
{
    public class SessionConnector : ISessionConnector
    {
        
        public string districtId
        {
            get {
                return HttpContext.Current.Request.Cookies.Get("ACTIVE_DISTRICT").Value;
            }
        }        

        public static SessionConnector Instance
        {
            get
            {
                SessionConnector conn = null;
                Interlocked.Exchange(ref conn, _sessionConnector);
                if (conn == null)
                {
                    lock (HttpContext.Current.Server)
                    {
                        if (_sessionConnector == null)
                        {
                            _sessionConnector = new SessionConnector();
                        }
                        conn = _sessionConnector;
                    }
                }
                return conn;
            }
        }
        private static SessionConnector _sessionConnector;
    }

}
