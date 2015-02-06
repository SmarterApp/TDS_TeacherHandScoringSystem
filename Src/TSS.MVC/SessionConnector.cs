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