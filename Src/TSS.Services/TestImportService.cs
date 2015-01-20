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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSS.Data;
using TSS.Domain;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public class TestImportService: ITestImportService
    {
        private readonly ITestImportRepository _testImportRepository;
        public TestImportService(ITestImportRepository testImportRepository)
        {
            _testImportRepository = testImportRepository;
        }


        public bool SaveDistrictAndSchool(string xmlInputs)
        {
            return _testImportRepository.SaveDistrictAndSchool(xmlInputs);
        }

        public bool SaveStudent(Student aStudent)
        {
            return _testImportRepository.SaveStudent(aStudent);
        }


        public bool SaveTeacher(Teacher aTeacher)
        {
            return _testImportRepository.SaveTeacher(aTeacher);
        }

        public bool SaveTest(Test aTest)
        {
            return _testImportRepository.SaveTest(aTest);
        }

        public bool BatchProcessAssingmentAndResponse(string xmlInputs)
        {
            return _testImportRepository.BatchProcessAssingmentAndResponse(xmlInputs);
        }

        public School PopulateSchoolFromTdsReport(TDSReport tdsReport)
        {
            var school = new School();
            if (tdsReport.Examinee.Items != null)
            {
                foreach (var obj in tdsReport.Examinee.Items.OfType<TDSReportExamineeExamineeRelationship>())
                {
                    if (obj.context != Context.FINAL) continue;
                    switch (obj.name)
                    {
                        case "SchoolID":
                            {
                                school.SchoolID = obj.value;
                                break;
                            }
                        case "SchoolName":
                            {
                                school.SchoolName = obj.value;
                                break;
                            }
                        case "StateName":
                            {
                                school.StateName = obj.value;
                                break;
                            }
                    }
                }
            }

            if (string.IsNullOrEmpty(school.SchoolID) && string.IsNullOrEmpty(school.SchoolName) && string.IsNullOrEmpty(school.StateName))
            {
                school.SchoolID = "0";
                school.SchoolName = "unknown";
                school.StateName = "unknown";
            }
            return school;
        }
        public District PopulateDistrictFromTdsReport(TDSReport tdsReport)
        {
            var district = new District();
            if (tdsReport.Examinee.Items != null)
            {
                foreach (var obj in tdsReport.Examinee.Items.OfType<TDSReportExamineeExamineeRelationship>())
                {
                    if (obj.context != Context.FINAL) continue;
                    switch (obj.name)
                    {
                        case "DistrictID":
                            {
                                district.DistrictID = obj.value;
                                break;
                            }
                        case "DistrictName":
                            {
                                district.DistrictName = obj.value;
                                break;
                            }
                    }
                }
            }

            if (string.IsNullOrEmpty(district.DistrictName) && string.IsNullOrEmpty(district.DistrictID))
            {
                district.DistrictID = "0";
                district.DistrictName = "unknown";
            }

            return district;
        }
        public Student PopulateStudentFromTdsReport(TDSReport tdsReport)
        {

            var student = new Student
            {
                StudentId = tdsReport.Examinee.key,
                Dob = DateTime.MaxValue
            };

            if (tdsReport.Examinee.Items != null && tdsReport.Examinee.Items.Count() > 0)
            {
                foreach (var obj in tdsReport.Examinee.Items.OfType<TDSReportExamineeExamineeAttribute>())
                {
                    if (obj.context == Context.FINAL) break ;
                    switch (obj.name)
                    {
                        case "DOB":
                            {
                                DateTime stuDob;
                                if (DateTime.TryParseExact(obj.value, "MMddyyyy", null, DateTimeStyles.None, out stuDob))
                                {
                                    student.Dob = stuDob;
                                }
                                break;
                            }
                        case "FirstName":
                            {
                                student.FirstName = obj.value;
                                break;
                            }
                        case "LastName":
                            {
                                student.LastName = obj.value;
                                break;
                            }
                        case "Grade":
                            {
                                student.Grade = obj.value;
                                break;
                            }
                        case "SSID":
                            {
                                student.SSID = obj.value;
                                break;
                            }
                        case "TDSLoginID":
                            {
                                student.TdsLoginId = obj.value;
                                break;
                            }
                        case "TDSTesteeName":
                            {
                                student.Name = obj.value;
                                break;
                            }
                    }
                }
            }
            return student;
        }
        public Teacher PopulateTeacherFromTdsReport(TDSReport tdsReport)
        {
            var teacher = new Teacher
            {
                Name = tdsReport.Opportunity.taName,
                TeacherID = tdsReport.Opportunity.taId
            };
            return teacher;
        }
        public Test PopulateTestFromTdsReport(TDSReport tdsReport)
        {
            var test = new Test();

            test.AcademicYear = (int)tdsReport.Test.academicYear;
            test.AssessmentType = tdsReport.Test.assessmentType == null ? "TEST" : tdsReport.Test.assessmentType;
            test.Bank = (int)tdsReport.Test.bankKey;
            test.Contract = tdsReport.Test.contract;
            test.Grade = tdsReport.Test.grade;
            test.TestId = tdsReport.Test.testId;

            switch (tdsReport.Test.mode)
            {
                case TDSReportTestMode.online:
                    {
                        test.Mode = "online";
                        break;
                    }
                case TDSReportTestMode.paper:
                    {
                        test.Mode = "paper";
                        break;
                    }
                default:
                    {
                        test.Mode = "scanned";
                        break;
                    }
            }

            test.Name = tdsReport.Test.name;
            test.Subject = tdsReport.Test.subject;
            test.Version = tdsReport.Test.assessmentVersion == null ? "1" : tdsReport.Test.assessmentVersion;

            return test;
        }
        public List<StudentResponse> PopulateItemsFromTdsReport(TDSReport tdsReport)
        {
            List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
            var responses = new List<StudentResponse>();

            foreach (var item in tdsReport.Opportunity.Item)
            {
                //if an item hasn’t been configured in THSS – ignore the item.
                if (!itemTypes.Any(l => l.ItemKey == (int)item.key && l.BankKey == (int)item.bankKey)) continue;
                //If the item is ‘scored’ in the XML – ignore the item.
                if (item.scoreStatus == TDSReportOpportunityItemScoreStatus.SCORED) continue;
                //If the item is ‘not scored’ in the THSS and has any status other than ‘scored’ in the XML , set status at not scored in thss
                int scoreStatus = (item.scoreStatus != TDSReportOpportunityItemScoreStatus.SCORED) ? 0 : 2;
                
                if (item.Response.Text == null) item.Response.Text = new string[0];
                var studentResponse = new StudentResponse()
                {
                    BankKey = (int)item.bankKey,
                    ContentLevel = item.contentLevel,
                    Format = item.format,
                    ItemKey = (int)item.key,
                    ResponseDate = item.Response.date,
                    Response = string.Join("", item.Response.Text),
                    SegmentId = item.segmentId,
                    ScoreStatus = scoreStatus
                };

                responses.Add(studentResponse);
            }

            return responses;
        }
    }
}
