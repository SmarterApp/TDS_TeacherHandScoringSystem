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
using System.Configuration;
using System.Xml;
using System;
using System.Data;
using System.Data.SqlClient;


namespace TSS.Data.Sql
{
    /// <summary>
    /// Base data access helper class
    /// </summary>
    public abstract class BaseRepository
    {
        private static string _connectionString;

        private static readonly int SqlCommandTimeout;

        /// <summary>
        /// Initializes the <see cref="BaseRepository"/> class.
        /// </summary>
        static BaseRepository()
        {
            SqlCommandTimeout = ConfigurationManager.AppSettings["SqlCommandTimeout"] != null ? Convert.ToInt32(ConfigurationManager.AppSettings["SqlCommandTimeout"]) : 60;
        }

        public static void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlCommand CreateCommand(CommandType cmdType, string cmdText)
        {
            if (string.IsNullOrEmpty(_connectionString)) _connectionString = ConfigurationManager.ConnectionStrings["TSS"].ConnectionString;
            SqlConnection connection = new SqlConnection(_connectionString);

            SqlCommand cmd = new SqlCommand(cmdText, connection)
                {
                    CommandType = cmdType,
                    CommandTimeout = SqlCommandTimeout
                };

            return cmd;
        }

        protected int ExecuteNonQuery(SqlCommand cmd)
        {
            int count = 0;

            try
            {
                cmd.Connection.Open();
                count = cmd.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {
                if (!(ex.Number.Equals(2627) && (ex.Procedure.Equals("WriteActivityLog"))))
                {
// ReSharper disable PossibleIntendedRethrow
                    throw ex;
// ReSharper restore PossibleIntendedRethrow
                }
            }
            finally
            {
                cmd.Connection.Close();
            }

            return count;
        }

        protected void ExecuteReader(SqlCommand cmd, Action<IColumnReader> readerProcessing)
        {
            try
            {
                cmd.Connection.Open();

                using (IColumnReader reader = new ColumnReader(cmd.ExecuteReader()))
                {
                    readerProcessing(reader);
                }
            }           
            finally
            {
                cmd.Connection.Close();
            }
        }

        protected T ExecuteScalar<T>(SqlCommand cmd)
        {
            try
            {
                cmd.Connection.Open();

                return (T)cmd.ExecuteScalar();

            }            
            finally
            {
                cmd.Connection.Close();
            }
        }
    }

    public static class SqlClientExtensions
    {
        /// <summary>
        /// Get a types value for use with SQL and deal with possible NULL's.
        /// </summary>
        private static object GetValue(object value)
        {
            if (value == null) return DBNull.Value;
            return value;
        }

        public static void AddValue(this SqlCommand cmd, string parameterName, object value)
        {
            cmd.Parameters.AddWithValue(parameterName, GetValue(value));
        }
    }
}
