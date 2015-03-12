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

         public void UpdateTeacherDistrictRelationship(string teacherId, string districtId)
         {
            _testImportRepository.UpdateTeacherDistrictRelationship(teacherId, districtId);
         }
        public School PopulateSchoolFromTdsReport(TDSReport tdsReport)
        {
            var school = new School();
            if (tdsReport.Examinee.Items != null)
            {
                foreach (var obj in tdsReport.Examinee.Items.OfType<TDSReportExamineeExamineeRelationship>())
                {
                    // if (obj.context != Context.FINAL) continue;
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
                    // if (obj.context != Context.FINAL) continue;
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
                    // if (obj.context != Context.FINAL) continue;
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
                        case "LastOrSurname":
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
        /// <summary>
        /// This is where we import all the things from TDS XML report.  
        /// There are a couple of assumptions we make here:
        /// 1.  The entire opportunity is imported at one time.  The dependency-checking logic expects this.
        /// 2.  The items are all in the same bank key.  This is always true in TDS tests.
        /// </summary>
        /// <param name="tdsReport"></param>
        /// <returns></returns>
        public List<StudentResponse> PopulateItemsFromTdsReport(TDSReport tdsReport)
        {
            List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
            var responses = new List<StudentResponse>();

            // We need to include some responses that may be scored, but have dependent responses.
            Dictionary<int, StudentResponse> allResponses = new Dictionary<int, StudentResponse>();
            Dictionary<int, List<StudentResponse>> responseToPassageMap = new Dictionary<int, List<StudentResponse>>();

            foreach (var item in tdsReport.Opportunity.Item)
            {
                //if an item hasn’t been configured in THSS – ignore the item.
                ItemType currentItem = itemTypes.Find(ci => ci.ItemKey == item.key && ci.BankKey == item.bankKey);
                if (currentItem == null)
                {
                    continue;
                }

                // Magic number: db uses '2' to mean scored.  We use it later so record that here.
                int score = (String.IsNullOrEmpty(item.scoreStatus) ||
                             item.scoreStatus == TDSReportOpportunityItemScoreStatus.SCORED.ToString())
                                ? 2
                                : 0;
                
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
                    ScoreStatus = score 
                };

                allResponses.Add(studentResponse.ItemKey, studentResponse);
                
                // If this item has a passage, record it.
                if (currentItem.Passage == 0)
                {
                    continue;
                }
                // If this item has a passage, add it to the list of items for that passage
                bool passageExists = responseToPassageMap.ContainsKey(currentItem.Passage);
                    List<StudentResponse> passageResponses = (passageExists)
                                                                 ? responseToPassageMap[currentItem.Passage]
                                                                 : new List<StudentResponse>();
                    
                if (!passageExists)
                {
                    responseToPassageMap.Add(currentItem.Passage,passageResponses);
                }
                passageResponses.Add(studentResponse);                
            }

            IEnumerable<int> keys = allResponses.Keys;
            foreach (int key in keys)
            {
                StudentResponse testResponse = allResponses[key];
                ItemType currentItem = 
                    itemTypes.Find(ci => ci.ItemKey == testResponse.ItemKey && 
                                         ci.BankKey == testResponse.BankKey);

                //If the item is not already scored, we for sure want it.
                if (testResponse.ScoreStatus != 2)
                {
                    responses.Add(testResponse);
                    continue;
                }
                
                // If the item is scored, and it doesn't have a passage, we for sure don't want it.
                if (!responseToPassageMap.ContainsKey(currentItem.Passage))
                {
                    continue;
                }

                // if the item is scored, but has a passage, see if an unscored item has the same passage.
                List<StudentResponse> passageResponses = responseToPassageMap[currentItem.Passage];
                foreach (StudentResponse passageResponse in passageResponses)
                {
                    if (passageResponse.ItemKey == testResponse.ItemKey ||
                        passageResponse.ScoreStatus == 2)
                    {
                        continue;
                    }
                    // An unscored item has the same passage.  We need this for scoring, so add it.
                    responses.Add(testResponse);
                    break;
                }
            }
            return responses;
        }
    }
}

