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
using TSS.Data.Sql;
using TSS.Domain.DataModel;

namespace TSS.Data
{
    public class LoggerRepository: BaseRepository,ILoggerRepository
    {
        public static LoggerRepository Instance = null;

        public LoggerRepository()
        {
            Instance = this;
        }
        public void SaveLog(Log log)
        {
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveLog");
            command.AddValue("Category", log.Category);
            command.AddValue("Details", log.Details);
            command.AddValue("IpAddress", log.IpAddress);
            command.AddValue("Level", log.Level);
            command.AddValue("LogDate", log.LogDate);
            command.AddValue("Message", log.Message);
            ExecuteNonQuery(command);
        }

        /// <summary>
        /// Log an exception with optional extra information
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="details"></param>
        /// <param name="message"></param>
        /// <param name="ip"></param>
        /// <param name="includeStackTrace"></param>
        public void LogException(Exception exp, string details = null, string message = null, string ip = null,bool includeStackTrace = true)
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
            log.IpAddress = ip;
            log.Level = LogLevel.Error;
            log.LogDate = DateTime.Now;
            log.Message = message;
            log.Details = details;
            SaveLog(log);
        }
    }
}
