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
using System.Threading;
using System.Web;

namespace TSS.Data.DataDistribution
{
    public class SessionConnector : ISessionConnector
    {
        private static readonly Object locker = new Object();

        public string districtId
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies.Get("ACTIVE_DISTRICT");
                if (cookie != null)
                {
                    return cookie.Value;
                }
                return "Unauthorized"; // hack since dist not authorized.
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
                    lock (locker)
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
