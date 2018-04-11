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
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSS.Data;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Areas.Api.Models;
using TSS.Services;
using TSS.Data.DataDistribution;
using System.Net;
using TSS.MVC.Controllers;

namespace TSS.MVC.Areas.Api.Controllers
{
    public class ItemController : BaseController
    {
        static string BASE_URL = Path.GetFullPath(@"Item-Manager\OH_ Items");

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
                    var assignment = _studentResponseService.GetAssignmentById((Guid)assignmentId);
                    if (assignment != null)
                    {
                        apiResult.Success = true;
                        apiResult.Assignment = assignment;
                    }
                }
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp, "api/Item/Get/Id", "Getting Assignment by Id Failed");
            }
            return Json(apiResult, JsonRequestBehavior.AllowGet);
        }

        public bool RefreshFilter(Dictionary<string, string> newFilters)
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
            UserAttributes ua = new UserAttributes();
            query.UserUUID = ua.TSSUserID;

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
                //throw new Exception("Test");
                UserAttributes ua = new UserAttributes();
                query.UserUUID = ua.TSSUserID;
                query.teacherUUIDs = ua.TeacherUUIDListCache; 
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
                                                         ua.TSSUserID.Trim(),
                                                         StringComparison.CurrentCultureIgnoreCase)),
                                      TeacherId = assignment.TeacherId.ToString()
                                  };

                    apiResult.StudentItem.Add(itm);
                }
                var jsonResult = Json(apiResult, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp, "/api/ItemList");
                if (Request.Cookies.Get("TSS-MAIL") == null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return Json("", JsonRequestBehavior.AllowGet);
                }
                //else
                //{
                //    // what to return?
                //    var jsonResult = Json(new StudentItemsApiResultModel(new AssignmentPage()), JsonRequestBehavior.AllowGet);
                //    jsonResult.MaxJsonLength = int.MaxValue;
                //    return jsonResult;
                //}

                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = HttpStatusCode.InternalServerError.ToString();
                return View("Error", new HandleErrorInfo(exp, "Item", "List"));

                //return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        // Return the default name of the training guide/exemplar
        private string GetDefaultFileName(int itemId, string fileType)
        {
            return string.Format("G3_{0}_{1}.pdf", itemId, fileType);
        }

        // Prepend the default root location to the filename to make it an absolute path.
        private string GetAbsoluteFileLocation(string baseUrl, string fileName)
        {            
            return Path.Combine(baseUrl, fileName);
        }

        // Returns file location of the training guide/exemplar if the configuration value is not specified
        private string GetDefaultFileLocation(string baseUrl, int itemId, string fileType)
        {
            return GetAbsoluteFileLocation(baseUrl, GetDefaultFileName(itemId, fileType));
        }

        // Returns file location based on training guide/exemplar configuration value
        //
        // Use the configuration value as is if it is an absolute path.
        // Otherwise prepend the default root location to the value to make it an absolute path.
        private string GetFileLocation(string baseUrl, string fileName)
        {
            if (Path.IsPathRooted(fileName))
            {
                return fileName;
            } else
            {
                return GetAbsoluteFileLocation(baseUrl, fileName);
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
                        var binData = binReader.ReadBytes((int)file.InputStream.Length);
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
                                var baseUrl = (obj.baseUrl != null) ? obj.baseUrl : BASE_URL;

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
                                itemType.ExemplarURL = (obj.exemplar != null) ? GetFileLocation(baseUrl, obj.exemplar) : GetDefaultFileLocation(baseUrl, obj.itemId, "TM");                                
                                itemType.TrainingGuideURL = (obj.trainingGuide != null) ? GetFileLocation(baseUrl, obj.trainingGuide) : GetDefaultFileLocation(baseUrl, obj.itemId, "SG");

                                System.Diagnostics.Debug.WriteLine(string.Format("Exemplar      : {0}", itemType.ExemplarURL));
                                System.Diagnostics.Debug.WriteLine(string.Format("Training Guide: {0}", itemType.TrainingGuideURL));

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
                            fileResult.Success = true;
                        }
                        catch (Exception ex)
                        {
                            fileResult.Success = false;
                            fileResult.ErrorMessage = ex.Message;
                            LoggerRepository.LogException(ex, "Item Json parsing failed");
                        }

                        
                    }
                    else
                    {
                        fileResult.ErrorMessage = "Error Code: 1001 Message:File does not contain any data.";
                    }

                    apiResult.Files.Add(fileResult);
                }
            }
            else
            {
                LoggerRepository.LogException(null, "No File Found for Item data Importing into the Database");
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
        /// <summary>
        /// Delete an assessment from THSS.
        /// api/item/delete?bankKey={bankKey}&itemKeys={itemKey1,itemKey2,...}
        /// </summary>
        /// <param name="bankKey">BankKey of the list of items to delete</param>
        /// <param name="itemKeys">Comma delemited list of item keys to delete</param>
        [System.Web.Mvc.HttpDelete] 
        public ActionResult Delete(int bankKey, string itemKeys)
        {
            var apiResult = new ItemDeleteApiResultModel();
            apiResult.BankKey = bankKey;
            apiResult.ItemKeys = itemKeys;
            try
            {
                string[] itemKeyArray = itemKeys.Split(',');
                List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
                foreach (var itemKeyString in itemKeyArray)
                {
                    var itemKey = 0;
                    Int32.TryParse(itemKeyString, out itemKey);
                    // remove existing item in cache
                    itemTypes.RemoveAll(i => i.BankKey == bankKey && i.ItemKey == itemKey);
                }
                ItemConfigSingleton.Instance.DeleteItemTypes(bankKey, itemKeys);
                apiResult.Success = true;
            }
            catch (Exception exp)
            {
                LoggerRepository.LogException(exp);
                apiResult.Success = false;
                apiResult.ErrorMessage = exp.Message;
            }
            return Json(apiResult);
        }
    }
}
