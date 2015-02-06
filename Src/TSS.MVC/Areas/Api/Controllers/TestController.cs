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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using TSS.Data;
using TSS.Data.DataDistribution;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Areas.Api.Models;
using TSS.Services;

namespace TSS.MVC.Areas.Api.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestImportService _testImportService;

        public TestController(ITestImportService testImportService)
        {
            _testImportService = testImportService;
        }


        // POST api/test/submit
        [System.Web.Mvc.HttpPost]
        public ActionResult Submit()
        {
            var apiResult = new TestSubmitApiResultModel();
            if (Request.Files.Count > 0)
            {
                try
                {

                    foreach (string fileName in Request.Files)
                    {
                        var fileResult = new TestSubmitApiResultFileModel();
                        fileResult.Success = false;

                        HttpPostedFileBase file = Request.Files[fileName];
                        if (file != null && file.ContentLength > 0)
                        {
                            fileResult.FileName = file.FileName;
                            var binReader = new BinaryReader(file.InputStream);
                            var binData = binReader.ReadBytes((int) file.InputStream.Length);
                            var memoryStream = new MemoryStream(binData);
                            var streamReader = new StreamReader(memoryStream);


                            ////validate xml file
                            XmlDocument doc = new XmlDocument();
                            doc.Load(streamReader);

                            //log all xml requests to database if the site is running under debug mode.
                            if (HttpContext.IsDebuggingEnabled)
                                LoggerRepository.SaveLog(new Log
                                                             {
                                                                 Category = LogCategory.Application,
                                                                 Level = LogLevel.Warning,
                                                                 Message = string.Format("/api/test/submit"),
                                                                 Details = file.FileName + ":" + doc.OuterXml
                                                             });

                            string xsdPath = Server.MapPath("~/App_Data/reportxml_os.xsd");
                            string errorString = Helpers.SchemaHelper.Validate(xsdPath, doc);
                            string valdiationOutput = string.IsNullOrEmpty(errorString)
                                                          ? String.Empty
                                                          : " File is not in a correct format. Validation Error:" +
                                                            errorString;

                            if (string.IsNullOrEmpty(errorString))
                            {
                                var serializer = new XmlSerializer(typeof (ItemScoreRequest));
                                try
                                {
                                    memoryStream.Position = 0;
                                    streamReader.DiscardBufferedData();
                                    var itemScoreRequest = (ItemScoreRequest) serializer.Deserialize(streamReader);
                                    streamReader.Close();
                                    memoryStream.Close();
                                    binReader.Close();

                                    ProcessScoreRequest(itemScoreRequest);

                                    fileResult.Success = true;
                                }
                                catch (Exception ex)
                                {
                                    fileResult.ErrorMessage = "There was an error processing the file. " + ex.Message +
                                                              ex.StackTrace;

                                    LoggerRepository.SaveLog(new Log
                                                                 {
                                                                     Category = LogCategory.Application,
                                                                     Level = LogLevel.Error,
                                                                     Message = string.Format("/api/test/submit"),
                                                                     Details = fileResult.ErrorMessage
                                                                 });
                                }
                            }
                            else
                            {
                                // if validation fails, then log the validation error and proceed with next request
                                fileResult.ErrorMessage = valdiationOutput;
                                LoggerRepository.SaveLog(new Log
                                                             {
                                                                 Category = LogCategory.Application,
                                                                 Level = LogLevel.Error,
                                                                 Message = string.Format("/api/test/submit"),
                                                                 Details = fileResult.ErrorMessage
                                                             });
                            }
                        }
                        else
                        {
                            fileResult.ErrorMessage = "Error Code: 1002 Message: File does not contain any data.";
                        }
                        apiResult.Files.Add(fileResult);
                    }
                }
                catch (Exception exp)
                {
                    LoggerRepository.LogException(exp);
                }
            }
            return Json(apiResult);
        }


        private TestImportRepository _testRepository;

        private void ProcessScoreRequest(ItemScoreRequest itemScoreRequest)
        {
            var tdsReport = itemScoreRequest.TDSReport;
            var district = _testImportService.PopulateDistrictFromTdsReport(tdsReport);
            _testRepository = new TSS.Data.TestImportRepository();


            //test info
            var test = _testImportService.PopulateTestFromTdsReport(tdsReport);
            _testRepository.SaveTest(test, district.DistrictID);
            //_testService.SaveTest(test);
            var teacher = _testImportService.PopulateTeacherFromTdsReport(tdsReport);
            //_teacherService.SaveTeacher(teacher);
            _testRepository.SaveTeacher(teacher, district.DistrictID);
            var student = _testImportService.PopulateStudentFromTdsReport(tdsReport);
            //_studentService.SaveStudent(student);
            _testRepository.SaveStudent(student, district.DistrictID);

            var school = _testImportService.PopulateSchoolFromTdsReport(tdsReport);
            school.DistrictID = district.DistrictID;
            StringBuilder xmlInputs = new StringBuilder();
            xmlInputs.Append(@"<Root>");
            xmlInputs.Append(@"<District");
            xmlInputs.Append(" DistrictId=\"" + district.DistrictID + "\"");
            xmlInputs.Append(" DistrictName=\"" + district.DistrictName + "\"");
            xmlInputs.Append(@"/>");
            xmlInputs.Append(@"<School");
            xmlInputs.Append(" SchoolId=\"" + school.SchoolID + "\"");
            xmlInputs.Append(" SchoolName=\"" + school.SchoolName + "\"");
            xmlInputs.Append(" StateName=\"" + school.StateName + "\"");
            xmlInputs.Append(@"/></Root>");
            _testRepository.SaveDistrictAndSchool(xmlInputs.ToString(), district.DistrictID);

            // returns all items from tdsReport that have status NOT SCORED and that have a matching item type in the system
            xmlInputs.Clear();
            xmlInputs.Append(@"<Root>");
            xmlInputs.Append(@"<Assignment");
            xmlInputs.Append(" TestId=\"" + test.TestId + "\"");
            xmlInputs.Append(" TeacherId=\"" + teacher.TeacherID + "\"");
            xmlInputs.Append(" StudentId=\"" + student.StudentId + "\"");
            xmlInputs.Append(" SchoolId=\"" + school.SchoolID + "\"");
            xmlInputs.Append(" SessionId=\"" + tdsReport.Opportunity.sessionId + "\"");
            xmlInputs.Append(" OpportunityId=\"" + tdsReport.Opportunity.oppId + "\"");
            xmlInputs.Append(" OpportunityKey=\"" + tdsReport.Opportunity.key + "\"");
            xmlInputs.Append(" ClientName=\"" + tdsReport.Opportunity.clientName + "\"");
            xmlInputs.Append(" CallbackUrl=\"" + itemScoreRequest.callbackUrl + "\"");
            xmlInputs.Append(@"/><ItemList>");
            var responses = _testImportService.PopulateItemsFromTdsReport(tdsReport);
            foreach (var response in responses)
            {
                xmlInputs.Append(@"<Item");
                xmlInputs.Append(" ItemKey=\"" + response.ItemKey + "\"");
                xmlInputs.Append(" BankKey=\"" + response.BankKey + "\"");
                xmlInputs.Append(" ContentLevel=\"" + response.ContentLevel + "\"");
                xmlInputs.Append(" Format=\"" + response.Format + "\"");
                xmlInputs.Append(" SegmentId=\"" + response.SegmentId + "\"");
                xmlInputs.Append(" ScoreStatus=\"" + response.ScoreStatus + "\"");
                xmlInputs.Append(" ResponseDate=\"" + response.ResponseDate + "\"");
                xmlInputs.Append(">");
                xmlInputs.Append("<Response>");
                xmlInputs.Append("<![CDATA[" + response.Response + "]]>");
                xmlInputs.Append("</Response>");
                xmlInputs.Append(@"</Item>");
            }
            xmlInputs.Append(@"</ItemList></Root>");
            _testRepository.BatchProcessAssingmentAndResponse(xmlInputs.ToString(), district.DistrictID);
        }

        // api/test/successresponse - xml
        //[System.Web.Mvc.HttpPost]
        public ActionResult SuccessResponse(string xml)
        {
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}
