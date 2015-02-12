﻿#region License
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
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSS.Data;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Areas.Api.Models;
using TSS.Services;
using TSS.Data.DataDistribution;

namespace TSS.MVC.Areas.Api.Controllers
{
    public class ItemController : Controller
    {
        private readonly ITeacherService _teacherService;
        private readonly IStudentResponseService _studentResponseService;
        
        public ItemController(IStudentResponseService studentResponseService)
        {
            _studentResponseService = studentResponseService;
        }       

        // GET api/item/get/id
        [System.Web.Mvc.HttpGet]
        public ActionResult Get(Guid? assignmentId)
        {
            var apiResult = new ItemGetApiResultModel();
            apiResult.Success = false;
            try
            {
                if (assignmentId != null)
                {
                    var assignment = _studentResponseService.GetAssignmentById((Guid) assignmentId);
                    if (assignment != null)
                    {
                        apiResult.Success = true;
                        apiResult.Assignment = assignment;
                    }
                }
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp);
            }
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        public bool RefreshFilter(Dictionary<string,string> newFilters)
        {
            if (Session["OLDFILTERS"] == null)
            {
                Session["OLDFILTERS"] = newFilters;
                return true;
            }
            if (newFilters.Equals((Dictionary<string, string>)Session["OLDFILTERS"]))
            {
                return false;
            }
            else
            {
                Session["OLDFILTERS"] = newFilters;
                return true;
            }

        }

        // get a full list of assignment ids for this user, sorted according to 
        // latest query parameters
        [System.Web.Mvc.HttpPost]
        public ActionResult GetAssignedItems(AssignedItemsQuery query)
        {
            List<string> assignments = new List<string>();
            query.UserUUID = UserAttributes.SAML.TSSUserID;

            // User ID list is really just this user, used to get row count.
            AssignmentPage page = _studentResponseService.GetSortedAssignmentIds(query);            
            return Json(page);
        }

        // GET api/item/list
        [System.Web.Mvc.HttpPost]
        public ActionResult List(AssignedItemsQuery query)
        {
            try
            {
                query.UserUUID = UserAttributes.SAML.TSSUserID;
                query.teacherUUIDs = UserAttributes.TeacherUUIDListCache;
                var assignmentPage = _studentResponseService.GetAssignmentsByAssignedToTeacherIDList(query);
                var assignmentList = assignmentPage.Assignments.ToList();
                var apiResult = new StudentItemsApiResultModel(assignmentPage);
                apiResult.RefreshFilters = RefreshFilter(query.filters);
                foreach (var assignment in assignmentList)
                {
                    var itm = new StudentItem
                                  {
                                      AssignedTo = assignment.AssignedTeacherName,
                                      AssignmentID = assignment.AssignmentId,
                                      Item = assignment.ItemKey + ": " + assignment.ItemTypeDescription,
                                      ItemKey = assignment.ItemKey,
                                      Session = assignment.SessionId.ToLower(),
                                      Status =
                                          (assignment.ScoreStatus == StudentResponseAssignment.ScoreStatusCode.NotScored
                                               ? "Not Scored"
                                               : assignment.ScoreStatus ==
                                                 StudentResponseAssignment.ScoreStatusCode.TentativeScore
                                                     ? "Tentatively Scored"
                                                     : "Scored"),
                                      StatusId = assignment.ScoreStatus.ToString(),
                                      StudentName = assignment.StudentName,
                                      CanScore =
                                          (String.Equals(assignment.TeacherId.Trim(),
                                                         UserAttributes.SAML.TSSUserID.Trim(),
                                                         StringComparison.CurrentCultureIgnoreCase))
                                  };

                    apiResult.StudentItem.Add(itm);
                }
                var jsonResult = Json(apiResult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp);

                // what to return?
                var jsonResult = Json(new StudentItemsApiResultModel(new AssignmentPage()), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        // TODO: POST api/item/submit  add a configurationRepository
        [System.Web.Mvc.HttpPost]
        public ActionResult Submit()
        {

            var apiResult = new TestSubmitApiResultModel();
            if (Request.Files.Count > 0)
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

                        try
                        {
                            string parseData = streamReader.ReadToEnd();
                            System.Diagnostics.Debug.WriteLine("parseData is " + parseData);
                            var rootObject = JsonConvert.DeserializeObject<List<ItemTypeJson.RootObject>>(parseData);

                            streamReader.Close();
                            memoryStream.Close();
                            binReader.Close();

                            List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
                            foreach (var obj in rootObject)
                            {
                                // load existing item to update if it exists
                                var itemType = itemTypes.SingleOrDefault(i => i.BankKey == obj.bankKey && i.ItemKey == obj.itemId);
                                bool isRelativeURI = !obj.baseUrl.ToLower().Contains("http");
                                Uri baseUri;

                                if (!isRelativeURI)
                                {
                                    baseUri = new Uri(obj.baseUrl);
                                }
                                else
                                {
                                    baseUri = new Uri(obj.baseUrl, UriKind.Relative);
                                }

                                // For new item types, create a new object
                                if (itemType == null)
                                {
                                    itemType = new ItemType();
                                    itemTypes.Add(itemType);
                                }
                                itemType.ItemKey = obj.itemId;
                                itemType.BankKey = obj.bankKey;
                                itemType.Passage = obj.passage != null ? int.Parse(obj.passage) : 0;
                                itemType.HandScored = obj.handScored.Contains("1");
                                itemType.Description = obj.description;
                                itemType.Subject = obj.subject;
                                itemType.Grade = obj.grade;
                                itemType.ExemplarURL = isRelativeURI ? new Uri(baseUri + obj.exemplar, UriKind.Relative).ToString() : new Uri(baseUri, obj.exemplar).AbsoluteUri;
                                itemType.TrainingGuideURL = isRelativeURI ? new Uri(baseUri + obj.trainingGuide, UriKind.Relative).ToString() : new Uri(baseUri, obj.trainingGuide).AbsoluteUri;
                                itemType.RubricListXML = obj.rubricList;
                                itemType.Layout = obj.Layout;

                                itemType.Modified = true;

                                itemType.Dimensions.Clear();
                                foreach (var dimension in obj.dimensions)
                                {
                                    var d = new Dimension();
                                    d.DimensionId = -1;  // convention used to distinguish new dimensions
                                    d.BankId = itemType.BankKey;
                                    d.ItemKey = itemType.ItemKey;
                                    d.ConditionCodes = new List<ConditionCode>();

                                    d.Name = dimension.description;
                                    d.Min = dimension.minpoints;
                                    d.Max = dimension.maxpoints;
                                    
                                    foreach (var cc in dimension.conditions)
                                    {
                                        var c = new ConditionCode
                                                    {
                                                        DimensionId = d.DimensionId,
                                                        FullName = cc.description,
                                                        ShortName = cc.code
                                                    };

                                        d.ConditionCodes.Add(c);
                                    }
                                    itemType.Dimensions.Add(d); 
                                }

                                //_itemTypeService.Save(itemType);
                            }
                        }
                        catch (Exception ex)
                        {
                            fileResult.ErrorMessage = ex.Message;
                            LoggerRepository.LogException(ex);
                        }

                        fileResult.Success = true;
                    }
                    else
                    {
                        fileResult.ErrorMessage = "Error Code: 1001 Message:File does not contain any data.";
                    }

                    apiResult.Files.Add(fileResult);
                }
            }
            try
            {
                ItemConfigSingleton.Instance.UpdateItemTypes();
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp);
            }
            return Json(apiResult);
        }
    }
}
