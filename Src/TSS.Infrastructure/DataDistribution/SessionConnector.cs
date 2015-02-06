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