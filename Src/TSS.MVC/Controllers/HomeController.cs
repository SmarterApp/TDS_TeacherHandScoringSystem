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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSS.Data;
using TSS.Domain.DataModel;
using TSS.MVC.Models;
using TSS.Domain;
using TSS.Services;
using System.Xml;
using System.Text;
using System.Web.Helpers;
using System.IO;
using System.Configuration;
using TSS.MVC.Helpers;
using TSS.MVC.Attributes;
using System.Net;

namespace TSS.MVC.Controllers
{
    public class HomeController : BaseController
    {


        public readonly IExportService _exportService;
        private readonly IStudentResponseService _studentResponseService;

		public HomeController( IExportService exportService, IStudentResponseService studentResponseService)
        {
            _exportService = exportService;
            _studentResponseService = studentResponseService;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Index";
            //var x = UserAttributes.TeacherCache;

            return View();
        }

        [NoCache]
        public ActionResult ItemList()
        {
            ViewBag.Title = "Response List";
            var itemListViewModel = new ItemListViewModel();

            return View(itemListViewModel);

        }

        public ActionResult ItemDetail(Guid id)
        {
            try
            {
                ItemDetailModel idm = new ItemDetailModel();
                idm.AssignmentId = id;
                return View(idm);

            }
            catch (Exception ex)
            {
                LoggerRepository.LogException(ex, "Error getting IRIS Url's");
                throw new Exception("Error getting IRIS Url's", ex);
            }

        }

        public ActionResult ItemView(Guid id)
        {
            try
            {
                ViewBag.Title = "Response Detail";

                var assignment = _studentResponseService.GetAssignmentById(id);

                #region Redirect to Response list if item is not assigned to current user
                UserAttributes ua = new UserAttributes();
                if (!(String.Equals(assignment.Teacher.TeacherID.Trim(), ua.TSSUserID.Trim(), StringComparison.CurrentCultureIgnoreCase))) // check if item is NOT assigned to current user
                {
                    //if item is not assigned to current user, throw error
                    throw new Exception("The student response is not assigned to current user.");
                }
                #endregion

                var itemDetailViewModel = new ItemDetailViewModel(assignment);

                // If this item is in a group, find the other items in the group from the db
                List<ItemGroupEntry> otherItems = new List<ItemGroupEntry>();
                List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
                var itemType = itemTypes.SingleOrDefault(i => i.BankKey == assignment.StudentResponse.BankKey && i.ItemKey == assignment.StudentResponse.ItemKey);
                int passage = itemType.Passage;
                if (itemType.Passage != 0)
                {
                    otherItems = _studentResponseService.GetResponsesFromItemGroup(assignment, itemType.Passage);
                }
                List<ContentRequestItem> Items = new List<ContentRequestItem>();
                Items.Add(
                    new ContentRequestItem()
                        {
                            Id = "I-" + assignment.StudentResponse.BankKey + "-" +
                                    assignment.StudentResponse.ItemKey,
                            Response = Regex.Replace(assignment.StudentResponse.Response, @"\t|\n|\r", "").Replace("\"", "&quot;").Replace("\'", "&#39;")
                        }
                    );
                if (passage != 0)
                {
                    // If this item is in a group, populate the group based on passage.
                    foreach (ItemGroupEntry other in otherItems)
                    {
                        // Don't add the item we're trying to score twice
                        if (other.ItemKey != assignment.StudentResponse.ItemKey)
                        {
                            Items.Add(
                                new ContentRequestItem()
                                    {
                                        Id = "I-" + other.BankKey + "-" +
                                                other.ItemKey,
		                                Response = other.Response
                                    }
                                );
                        }
                    }

                }

                ContentRequest request = new ContentRequest();
                request.Layout = itemType.Layout;
                request.Items = Items;
                if (passage == 0)
                {
                    request.Passage = new { autoLoad = "false" };
                }
                else
                {
                    request.Passage = new ContentRequestPassage()
		                {
		                    Id = "P-" +
                        itemType.BankKey.ToString() +
                            "-" + 
                        passage.ToString()
		                };

                }

                itemDetailViewModel.ContentToken = JsonConvert.SerializeObject(request);
                return View(itemDetailViewModel);
            }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc, "Getting IRIS Response Failed in /home/ItemView", Convert.ToString(Response));
                throw new Exception("Error Code: 4005", exc);
            }

        }

        [HttpPost]
        public JsonResult SubmitScores(Guid id, string[] scores, string[] conditions, string[] dimensions)
        {

            try
            {
                var assignment = _studentResponseService.GetAssignmentById(id);

                #region Return error if we try to score the item which is  already been marked as complete
                if (assignment.Teacher.TeacherID == null && assignment.ScoreStatus == StudentResponseAssignment.ScoreStatusCode.NotScored)
                {
                    return Json(new { success = false, data = String.Empty, message = "Item has already been marked as completed" });
                }
                #endregion

                #region Return error if item is not assigned to current user
                UserAttributes ua = new UserAttributes();
                if (!(String.Equals(assignment.Teacher.TeacherID.Trim(), ua.TSSUserID.Trim(), StringComparison.CurrentCultureIgnoreCase))) // check if item is NOT assigned to current user
                {
                    // if item is not assigned to user, don't save the score and instead return an error message
                    return Json(new { success = false, data = String.Empty, message = "You cannot score responses that are assigned to another user." });
                }
                #endregion

                var scoreType = "dimension";
                if (scores.Length == 1 && dimensions[0] == string.Empty)
                {
                    //If true, then this is a rubric score type.
                    scoreType = "rubric";
                }
                //form scoredata 
                var xml = string.Empty;
                var scoreHelper = new ScoringHelper();

                scoreHelper.Type = scoreType;

                for (int i = 0; i < scores.Length; i++)
                {
                    scoreHelper.Dimensions.Add(new ScoreDimension
                    {
                        Name = dimensions[i],
                        Score = scores[i],
                        ConditionCode = conditions[i]
                    });
                }
                xml = ScoringHelper.ToXmlString(scoreHelper);
                //Save It
                assignment.ScoreData = xml;
                assignment.ScoreStatus = StudentResponseAssignment.ScoreStatusCode.TentativeScore;
                _studentResponseService.UpdateAssignment(assignment);

                //return
                var successMessage = string.Empty;
                if (ConfigurationManager.AppSettings["SCORE_SUBMITTED_MESSAGE"] != null)
                {
                    successMessage = ConfigurationManager.AppSettings["SCORE_SUBMITTED_MESSAGE"].ToString();
                }
                return Json(new { success = true, data = xml.ToString(), message = successMessage, ScoreStatus = (StudentResponseAssignment.ScoreStatusCode.TentativeScore.ToString() == "TentativeScore" ? "Tentatively Scored" : "Scored") });
            }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc, "Item Scoring Failed");
                return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4004, exc.Message) });
            }
        }

        [HttpPost]
        public JsonResult LogException(string msg, string details)
        {
            try
            {
                LoggerRepository.LogException(null, details, msg, "NA", false);
            }
            catch
            {
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
