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
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Web;
using TSS.Data.Sql;
using TSS.Domain.DataModel;
using TSS.Data.DataDistribution;
using System.Diagnostics;

namespace TSS.Data
{
    public class LoggerRepository: BaseRepository,ILoggerRepository
    {
		private static LoggerRepository _instance = null;
        private static readonly Object locker = new Object();

        private static LoggerRepository Instance
        {
            get
            {
                Object rv=null;
                Interlocked.Exchange(ref rv, _instance);
                if (rv == null)
                {
                    lock (locker)
                    {
                        if (_instance != null)
                        {
                            return _instance;
                        }
                        _instance = new LoggerRepository();
                        rv = _instance;
                    }
                }
                return rv as LoggerRepository;
            }
        }

        public LoggerRepository()
        {
        }

        /// <summary>
        /// Save a log to db.  If isError is set, we write it to (error)Log, else activity log
        /// </summary>
        /// <param name="log"></param>
        /// <param name="isError">If true, logs to error table, else activity table</param>
        public static void SaveLog(Log log,bool isError=false)
        {
            try
            {
                string sproc = "dbo.sp_SaveActivityLog";
                if (isError)
                {
                    sproc = "dbo.sp_SaveLog";
                }
                SqlCommand command = Instance.CreateCommand(CommandType.StoredProcedure,sproc);
                command.AddValue("Category", log.Category);
                command.AddValue("Details", log.Details);
                command.AddValue("Level", log.Level);
                command.AddValue("IpAddress",
                                 HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
                command.AddValue("LogDate", log.LogDate);
                command.AddValue("Message", log.Message);
                Instance.ExecuteNonQuery(command);
            }
            catch (Exception e)
            {
                // If we can't log for some reason, silently fail.  If we throw another exception here
                // it will loop-d-loop.
                // EventLog.WriteEntry("THSS", e.Message + ":"+ e.StackTrace,EventLogEntryType.Error);
                System.Diagnostics.Debug.WriteLine("THSS " +  e.Message + ":" + e.StackTrace);
            }
        }

        /// <summary>
        /// Log an exception with optional extra information
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="details"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <param name="includeStackTrace"></param>
        public static void LogException(Exception exp, string details = null, string message = null, string ip = null,bool includeStackTrace = true)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = exp.Message;
            }
            if (string.IsNullOrEmpty(details))
            {
                details = "Uncaught Exception";
            }
            if (includeStackTrace)
            {
                details = details + " stack: " + exp.StackTrace;
            }
            if (String.IsNullOrEmpty(ip))
            {
                ip = exp.Source;
            }

            Log log = new Log();
            log.Category = LogCategory.Security;
            log.Level = LogLevel.Error;
            log.LogDate = DateTime.Now;
            log.IpAddress = ip;
            log.Message = message;
            log.Details = details;
            SaveLog(log,true);
        }
    }
}
