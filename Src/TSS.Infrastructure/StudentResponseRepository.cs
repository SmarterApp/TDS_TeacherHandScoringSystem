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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using TSS.Data.Sql;

using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.Data;

using TSS.Data;
using TSS.Data.DataDistribution;


namespace TSS.Data
{
    public class StudentResponseRepository :  BaseRepository,IStudentResponseRepository
    {

        public StudentResponseRepository()
        {
        }

        
        void LogParameters(SqlCommand command)
        {
            string output = "";
            if (ConfigurationManager.AppSettings["debug.logsql"] == "true" ||
                ConfigurationManager.AppSettings["debug.loglocal"] == "true")
            {
                output = "SQL: " + command.CommandText;

                foreach (SqlParameter param in command.Parameters)
                {
                    output += "    param: " + param.ParameterName + " value: " + param.Value.ToString();
                }

                // This code is compiled out in prod versions...
                System.Diagnostics.Debug.WriteLine(output);

                if (ConfigurationManager.AppSettings["debug.logsql"] == "true")
                {
                    SqlCommand lcmd = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveLog");
                    Log log = new Log();
                    log.Category = LogCategory.Application;
                    log.Level = LogLevel.Debug;
                    log.IpAddress = "NA";
                    log.LogDate = DateTime.UtcNow;
                    log.Details = output;
                    log.Message = "Sproc";

                    // Todo - fix log repository to be more public
                    lcmd.AddValue("Category", log.Category);
                    lcmd.AddValue("Details", log.Details);
                    lcmd.AddValue("IpAddress", log.IpAddress);
                    lcmd.AddValue("Level", log.Level);
                    lcmd.AddValue("LogDate", log.LogDate);
                    lcmd.AddValue("Message", log.Message);
                    ExecuteNonQuery(lcmd);
                }
            }
        }

        public bool RemoveAssignments(string[] assignmentIDs)
        {
            bool rtnCode = false;
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_RemoveAssignments]");
            command.AddValue("AssignmentList", string.Join("|", assignmentIDs));
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0);
                }
            });
            return rtnCode == false;
        }

        public bool ReAssign(string[] assignmentIDs, Teacher teacher)
        {
            bool rtnCode = false;
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_ReAssignAssignments]");
            command.AddValue("TeacherID", teacher.TeacherID);
            command.AddValue("TeacherName", teacher.Name);
            command.AddValue("AssignmentList", string.Join("|", assignmentIDs));
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0);
                }
            });
            return rtnCode == false;
        }
        public bool UpdateAssignment(StudentResponseAssignment assignment)
        {
            bool rtnCode = false;
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_UpdateAssignmentScore]");
            command.AddValue("AssignmentId", assignment.AssignmentId);
            command.AddValue("ScoreData", assignment.ScoreData);
            command.AddValue("ScoreStatus", (int)assignment.ScoreStatus);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0);
                }
            });
            return rtnCode == false;
        }

        public List<ItemGroupEntry> GetResponsesFromItemGroup(StudentResponseAssignment assignment, int passage)
        {
            List<ItemGroupEntry> rv = new List<ItemGroupEntry>();
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetResponsesForGroup]");
            command.AddValue("Assignment",assignment.AssignmentId);
            command.AddValue("Passage", passage);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                while (reader.Read())
                {
                    reader.FixNulls = true;
                    ItemGroupEntry entry = new ItemGroupEntry();
                    entry.BankKey = reader.GetInt32("BankKey");
                    entry.ItemKey = reader.GetInt32("ItemKey");
                    entry.Response = reader.GetString("Response");
                    rv.Add(entry);
                }
            });
            return rv;
        }

        public bool UpdateAssignmentStatus(string[] assignmentIDs, int status)
        {
            bool rtnCode = false;
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_UpdateAssignmentsStatus]");
            command.AddValue("AssignmentList", string.Join("|", assignmentIDs));
            command.AddValue("Status",status);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0);
                }
            });
            return rtnCode == false;
        }

        public IList<StudentResponseAssignment> GetAssignmentsByIDList(string[] assignmentIDs)
        {
            IList<StudentResponseAssignment> assignments = new List<StudentResponseAssignment>();
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetAssignmentsByIDList]");
            command.AddValue("AssignmentList", string.Join("|", assignmentIDs));
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                while (reader.Read())
                {
                    reader.FixNulls = true;
                    StudentResponseAssignment assignment = new StudentResponseAssignment();
                    assignment.AssignmentId = reader.GetGuid("AssignmentId");
                    assignment.CallbackUrl = reader.GetString("CallbackUrl");
                    assignment.ClientName = reader.GetString("ClientName");
                    assignment.OpportunityKey = reader.GetGuid("OpportunityKey"); 
                    assignment.ScoreData = reader.GetString("ScoreData");
                    assignment.ScoreStatus = (StudentResponseAssignment.ScoreStatusCode)(reader.GetInt32("ScoreStatus"));
                    assignment.StudentResponse.ItemKey = reader.GetInt32("ItemKey");
                    assignment.StudentResponse.BankKey = reader.GetInt32("BankKey");
                    assignment.StudentResponse.Format = reader.GetString("Format");
                    assignment.Teacher.TeacherID = reader.GetString("TeacherId");
                    assignments.Add(assignment);
                }
            });
            return assignments;
        }

        public StudentResponseAssignment GetAssignmentById(Guid assignmentId)
        {
            StudentResponseAssignment assignment = new StudentResponseAssignment();
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetAssignmentById]");
            command.AddValue("AssignmentId", assignmentId);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                while (reader.Read())
                {
                    reader.FixNulls = true;
                    assignment.AssignmentId = reader.GetGuid("AssignmentId");
                    assignment.CallbackUrl = reader.GetString("CallbackUrl");
                    assignment.ClientName = reader.GetString("ClientName");
                    assignment.OpportunityId = reader.GetInt64("OpportunityId"); ;
                    assignment.OpportunityKey = reader.GetGuid("OpportunityKey");
                    assignment.ScoreData = reader.GetString("ScoreData");
                    assignment.ScoreStatus = (StudentResponseAssignment.ScoreStatusCode)(reader.GetInt32("ScoreStatus"));
                    assignment.SessionId = reader.GetString("SessionId");
                    assignment.StudentResponse.BankKey = reader.GetInt32("BankKey");
                    assignment.StudentResponse.ItemKey = reader.GetInt32("ItemKey");
                    assignment.StudentResponse.Response = reader.GetString("Response");
                    assignment.Student.Name = reader.GetString("StudentName");
                    assignment.Teacher.TeacherID = reader.GetString("TeacherId"); 
                    assignment.Test.TestId = reader.GetString("TestId");
                    assignment.Test.Name = reader.GetString("TestName"); 
                }
            });
            return assignment;
        }

        /// <summary>
        /// Get full list of assignment ids for the current user, sorted by parameters
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public AssignmentPage GetSortedAssignmentIds(AssignedItemsQuery query)
        {
            string sortColumn = "";
            switch (query.sortColumn)
            {
                case "Name":
                    sortColumn = "StudentName";
                    break;
                case "Item":
                    sortColumn = "ItemKey";
                    break;
                case "Session":
                    sortColumn = "SessionId";
                    break;
                case "Status":
                    sortColumn = "ScoreStatus";
                    break;
                case "AssignedTo":
                    sortColumn = "AssignedTo";
                    break;
            }

            // Test filter needs some finessing due to name/value pair
            string testFilter = query.filters.Where(x => x.Key == "test-Name").Select(x => x.Value).FirstOrDefault();

            List<String> assignments = new List<String>();

            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetSortedAssignmentIds]");
            command.AddValue("SortColumn", sortColumn);
            command.AddValue("SortDirection", query.sortDirection);
            command.AddValue("TeacherId", query.UserUUID);
            command.AddValue("TestFilter", testFilter);
            command.AddValue("SessionFilter",
                             query.filters.Where(x => x.Key.Contains("a-SessionId")).Select(x => x.Value).FirstOrDefault
                                 ());
            command.AddValue("GradeFilter",
                             query.filters.Where(x => x.Key.Contains("test-Grade")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("SubjectFilter",
                             query.filters.Where(x => x.Key.Contains("test-Subject")).Select(x => x.Value).
                                 FirstOrDefault());

            LogParameters(command);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                while (reader.Read())
                {
                    Guid guid = reader.GetGuid("AssignmentId");
                    assignments.Add(guid.ToString());
                }
            });
     
            var page = new AssignmentPage();

            // Create pipe-delimited list for the json object
            page.AllAssignmentIds = string.Join("|", assignments);
            return page;
        }

        public AssignmentPage GetAssignmentsByAssignedToTeacherIDList(AssignedItemsQuery query)
        {
            string sortColumn = "";
            switch (query.sortColumn)
            {
                case "Name":
                    sortColumn = "StudentName";
                    break;
                case "Item":
                    sortColumn = "ItemKey";
                    break;
                case "Session":
                    sortColumn = "SessionId";
                    break;
                case "Status":
                    sortColumn = "ScoreStatus";
                    break;
                case "AssignedTo":
                    sortColumn = "AssignedTo";
                    break;
            }

            string testFilter = query.filters.Where(x => x.Key == "test-Name").Select(x => x.Value).FirstOrDefault();
            var pagedListResults = GetPageListResults(query, sortColumn, testFilter);
            var rowCount = GetRowCount(query, testFilter);
            var filterItems = GetFilterResults(query);

            // var assignedItems = GetSortedAssignments(query, sortColumn, rowCount, xxx);

            var page = new AssignmentPage();
            //SET ROW COUNT
            page.rowcount = rowCount;//PagedListResults.Count();
            
            // LIST OF ASSIGNMENT IDS
            page.AllAssignmentIds = "";  // this is now filled in elsewhere

            page.FilterItems = filterItems;

            //PAGED RESULT
            page.Assignments = pagedListResults.ToList();

            return page;
        }

        #region Private helper
        private IEnumerable<Guid> GetAssignedItems(AssignedItemsQuery query,string testFilter)
        {
            List<Guid> assignedResponseIDs = new List<Guid>();
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetAssignedItems]");
            command.AddValue("TeacherId", query.UserUUID);
            command.AddValue("TestFilter", testFilter);
            command.AddValue("SessionFilter",query.filters.Where(x => x.Key.Contains("a-SessionId")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("GradeFilter", query.filters.Where(x => x.Key.Contains("test-Grade")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("SubjectFilter", query.filters.Where(x => x.Key.Contains("test-Subject")).Select(x => x.Value).
                                 FirstOrDefault());
            command.AddValue("ScorerFilter", query.filters.Where(x => x.Key == "t-Name").Select(x => x.Value).FirstOrDefault());

            LogParameters(command);

            ExecuteReader(command, delegate(IColumnReader reader)

            {
                while (reader.Read())
                {
                    Guid responseId = reader.GetGuid("AssignmentId");
                    assignedResponseIDs.Add(responseId);
                }
            });
               
            return assignedResponseIDs;
        }

        private List<FilterResult> GetFilterResults(AssignedItemsQuery query)
        {
            //[sp_get_itemListFilters2]
            List<FilterResult> FilterItems = new List<FilterResult>();
            if (query.GetFilters)
            {
                SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetItemListFilters]");
                command.AddValue("teacherIDlist", string.Join("|", query.teacherUUIDs.Distinct().ToList()));
                LogParameters(command);

                ExecuteReader(command, delegate(IColumnReader reader)
                                           {
                                               while (reader.Read())
                                               {
                                                   FilterResult result = new FilterResult()
                                                   {
                                                       Grade = reader.GetString("Grade"),
                                                       Subject = reader.GetString("Subject"),
                                                       TestName = reader.GetString("TestName"),
                                                       TestID = reader.GetString("TestID"),
                                                       SessionId = reader.GetString("SessionId"),
                                                       AssignedTeacherName = reader.GetString("AssignedTo"),
                                                       AssignedTeacherID = reader.GetString("TeacherID")

                                                   };

                                                   FilterItems.Add(result);
                                               }
                                           });
            }
            return FilterItems;
        }

        private int GetRowCount(AssignedItemsQuery query, string xxx)
        {
            int rowCount = 0;
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetItemCount]");
            command.AddValue("EmailList", string.Join("|", query.teacherUUIDs.Distinct().ToList()));
            command.AddValue("TestFilter", xxx);
            command.AddValue("SessionFilter",
                             query.filters.Where(x => x.Key.Contains("a-SessionId")).Select(x => x.Value).FirstOrDefault
                                 ());
            command.AddValue("GradeFilter",
                             query.filters.Where(x => x.Key.Contains("test-Grade")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("SubjectFilter",
                             query.filters.Where(x => x.Key.Contains("test-Subject")).Select(x => x.Value).
                                 FirstOrDefault());
            command.AddValue("ScorerFilter",
                             query.filters.Where(x => x.Key == "t-Name").Select(x => x.Value).FirstOrDefault());

            LogParameters(command);

            ExecuteReader(command, delegate(IColumnReader reader)
                                       {
                                           reader.FixNulls = true;
                                           if (reader.Read())
                                           {
                                               rowCount = reader.GetInt32(0);
                                           }
                                       });


            return rowCount;
        }

        private List<String> GetSortedAssignmentIdsSql(AssignedItemsQuery query, string sortColumn, string testFilter)
        {
            List<String> assignments = new List<String>();

            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetSortedAssignmentIds]");
            command.AddValue("SortColumn", sortColumn);
            command.AddValue("SortDirection", query.sortDirection);
            command.AddValue("TeacherId", query.UserUUID);
            command.AddValue("TestFilter", testFilter);
            command.AddValue("SessionFilter",
                             query.filters.Where(x => x.Key.Contains("a-SessionId")).Select(x => x.Value).FirstOrDefault
                                 ());
            command.AddValue("GradeFilter",
                             query.filters.Where(x => x.Key.Contains("test-Grade")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("SubjectFilter",
                             query.filters.Where(x => x.Key.Contains("test-Subject")).Select(x => x.Value).
                                 FirstOrDefault());

            LogParameters(command);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                while (reader.Read())
                {
                    Guid guid = reader.GetGuid("AssignmentId");
                    assignments.Add(guid.ToString());
                }
            });
     
            return assignments;
        }

        private List<AssignmentResult> GetPageListResults(AssignedItemsQuery query, string sortColumn,string testFilter)
        {
            List<AssignmentResult> pagedListResults = new List<AssignmentResult>();
           
            int startRow = (query.pageNumber - 1)*query.pageLength;
            int endRow = ((query.pageNumber - 1)*query.pageLength) + query.pageLength;

            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetItemList]");
            command.AddValue("StartRow", startRow.ToString());
                    command.AddValue("EndRow", endRow.ToString());
                    command.AddValue("SortColumn", sortColumn);
            command.AddValue("SortDirection", query.sortDirection);
            command.AddValue("EmailList", string.Join("|", query.teacherUUIDs.Distinct().ToList()));
            command.AddValue("TestFilter", testFilter);
            command.AddValue("SessionFilter",
                             query.filters.Where(x => x.Key.Contains("a-SessionId")).Select(x => x.Value).FirstOrDefault
                                 ());
            command.AddValue("GradeFilter",
                             query.filters.Where(x => x.Key.Contains("test-Grade")).Select(x => x.Value).FirstOrDefault());
            command.AddValue("SubjectFilter",
                             query.filters.Where(x => x.Key.Contains("test-Subject")).Select(x => x.Value).
                                 FirstOrDefault());
            command.AddValue("ScorerFilter",
                             query.filters.Where(x => x.Key == "t-Name").Select(x => x.Value).FirstOrDefault());
            
            LogParameters(command);
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                while (reader.Read())
                {
                    AssignmentResult result = new AssignmentResult()
                                                  {
                                                      AssignedTeacherName = reader.GetString("AssignedTo"),
                                                      SessionId = reader.GetString("SessionId"),
                                                      ScoreStatus =
                                                          (StudentResponseAssignment.ScoreStatusCode)
                                                          (reader.GetInt32("ScoreStatus")),
                                                      ItemKey = reader.GetInt32("ItemKey"),
                                                      ItemTypeDescription = reader.GetString("ItemTypeDescription"),
                                                      StudentName = reader.GetString("StudentName"),
                                                      AssignmentId = reader.GetGuid("AssignmentId"),
                                                      TeacherId = reader.GetString("TeacherUUID")
                                                  };
                    pagedListResults.Add(result);
                }
            });
     
            return pagedListResults;
        }
        #endregion
    }
}
