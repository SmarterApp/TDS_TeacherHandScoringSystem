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
            //var school = _testImportService.PopulateSchoolFromTdsReport(tdsReport);
            //school.DistrictID = district.DistrictID;
            _testRepository = new TSS.Data.TestImportRepository();
            // insert/update district and school
            //_testRepository.SaveDistrictAndSchool(district, school, district.DistrictID);
            // insert/update test
            var test = _testImportService.PopulateTestFromTdsReport(tdsReport);
            _testRepository.SaveTest(test, district.DistrictID);
            //insert/update teacher
            var teacher = _testImportService.PopulateTeacherFromTdsReport(tdsReport);
            _testRepository.SaveTeacher(teacher, district.DistrictID);
            //insert/update student
            var student = _testImportService.PopulateStudentFromTdsReport(tdsReport);
            _testRepository.SaveStudent(student, district.DistrictID);

            //insert/update assignments and responses
            StringBuilder xmlInputs = new StringBuilder();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = Encoding.UTF8;
            // returns all items from tdsReport that have status NOT SCORED and that have a matching item type in the system
            using (XmlWriter xmlWriter = XmlWriter.Create(xmlInputs, xmlWriterSettings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Root");
                xmlWriter.WriteStartElement("Assignment");
                xmlWriter.WriteAttributeString("TestId", test.TestId);
                xmlWriter.WriteAttributeString("TeacherId", teacher.TeacherID);
                xmlWriter.WriteAttributeString("StudentId", student.StudentId.ToString());
                //xmlWriter.WriteAttributeString("SchoolId", school.SchoolID);
                xmlWriter.WriteAttributeString("SessionId", tdsReport.Opportunity.sessionId);
                xmlWriter.WriteAttributeString("OpportunityId", tdsReport.Opportunity.oppId);
                xmlWriter.WriteAttributeString("OpportunityKey", tdsReport.Opportunity.key);
                xmlWriter.WriteAttributeString("ClientName", tdsReport.Opportunity.clientName);
                xmlWriter.WriteAttributeString("CallbackUrl", itemScoreRequest.callbackUrl);
                xmlWriter.WriteEndElement();//end of assignment node
                xmlWriter.WriteStartElement("ItemList");
                var responses = _testImportService.PopulateItemsFromTdsReport(tdsReport);
                foreach (var response in responses)
                {

                    xmlWriter.WriteStartElement("Item");
                    xmlWriter.WriteAttributeString("ItemKey", response.ItemKey.ToString());
                    xmlWriter.WriteAttributeString("BankKey", response.BankKey.ToString());
                    xmlWriter.WriteAttributeString("ContentLevel", response.ContentLevel);
                    xmlWriter.WriteAttributeString("Format", response.Format);
                    xmlWriter.WriteAttributeString("SegmentId", response.SegmentId);
                    xmlWriter.WriteAttributeString("ScoreStatus", response.ScoreStatus.ToString());
                    xmlWriter.WriteAttributeString("ResponseDate", response.ResponseDate.ToString());
                    xmlWriter.WriteStartElement("Response");
                    // xmlWriter.WriteString("<![CDATA[" + response.Response + "]]>");
                    // xmlWriter.WriteString(response.Response);
                    xmlWriter.WriteCData(response.Response);
                    xmlWriter.WriteEndElement();//end of response node
                    xmlWriter.WriteEndElement(); //end of item node
                }
                xmlWriter.WriteEndElement();//end of itemlist node
                xmlWriter.WriteEndElement();//end of root node
                xmlWriter.Close();
            }
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

