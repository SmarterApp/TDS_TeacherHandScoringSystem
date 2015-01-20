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
using System.Data;
using System.Data.SqlClient;
using TSS.Data.Sql;
using TSS.Domain.DataModel;

namespace TSS.Data
{
    public class TestImportRepository: BaseRepository, ITestImportRepository
    {
        public bool SaveTeacher(Teacher aTeacher)
        {
            
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveTeacher");
            command.AddValue("TeacherID", aTeacher.TeacherID);
            command.AddValue("TeacherName", aTeacher.Name);
            bool rtnCode = true;
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0); // read error flag
                }
            });

            return rtnCode == false;
        }

        public bool SaveStudent(Student aStudent)
        {
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveStudent");
            command.AddValue("StudentId", aStudent.StudentId);
            command.AddValue("DOB", aStudent.Dob);
            command.AddValue("FirstName", aStudent.FirstName);
            command.AddValue("LastName", aStudent.LastName);
            command.AddValue("SSID", aStudent.SSID);
            command.AddValue("Name", aStudent.Name);
            command.AddValue("TdsLoginId", aStudent.TdsLoginId);
            command.AddValue("Grade", aStudent.Grade);
            bool rtnCode = true;
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0); // read error flag
                }
            });

            return rtnCode == false;
        }

        public bool SaveTest(Test aTest)
        {
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveTest");
            command.AddValue("TestID", aTest.TestId);
            command.AddValue("Name", aTest.Name);
            command.AddValue("Mode", aTest.Mode);
            command.AddValue("Grade", aTest.Grade);
            command.AddValue("Subject", aTest.Subject);
            command.AddValue("Version", aTest.Version);
            command.AddValue("AcademicYear", aTest.AcademicYear);
            command.AddValue("AssessmentType", aTest.AssessmentType);
            command.AddValue("Bank", aTest.Bank);
            command.AddValue("Contract", aTest.Contract);
            
            bool rtnCode = true;
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0); // read error flag
                }
            });
            return rtnCode == false;
        }

        public bool SaveDistrictAndSchool(string xmlInputs)
        {
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveDistrictAndSchool");
            command.AddValue("xmlInputs",xmlInputs);
            bool rtnCode = true;
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0); // read error flag
                }
            });

            return rtnCode == false;
        }

        public bool BatchProcessAssingmentAndResponse(string xmlInputs)
        {
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "dbo.sp_SaveAssignmentAndResponses");
            command.AddValue("xmlInputs", xmlInputs);

            bool rtnCode = true;
            ExecuteReader(command, delegate(IColumnReader reader)
            {
                reader.FixNulls = true;
                if (reader.Read())
                {
                    rtnCode = reader.GetBoolean(0); // read error flag
                }
            });

            return rtnCode == false;
        }
    }
}
