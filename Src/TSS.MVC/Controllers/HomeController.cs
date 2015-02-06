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

namespace TSS.MVC.Controllers
{
	public class HomeController : Controller
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
            try
            {
                var itemListViewModel = new ItemListViewModel();

                return View(itemListViewModel);
            }
            catch (Exception exc)
            {
                throw new Exception("Error Code: 4003", exc);
            }
            
		}

        
        [HttpPost]
        public JsonResult ReassignScorer(string[] ids, string scorerUUID, string name)
        {
            try
            {
                Teacher teacher = new Teacher(){Name = name, TeacherID = scorerUUID};
                //Save It
                _studentResponseService.ReAssign(ids, teacher);

                return Json(new { success = true, data = "" });
            }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc);
                return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4002, exc.Message) });
            }
        }

        [HttpPost]
        public JsonResult MarkItems(string[] ids, int status)
        {

            try
            {
                //GET ASSIGNMENTS
                IList<StudentResponseAssignment> tentativelyScoredassignments = _studentResponseService.GetAssignmentsByIDList(ids);

                //Mark as scored, so no other users will mark them?
                List<Guid> tentativelyScoredAssignmentIds = tentativelyScoredassignments.Select(a => a.AssignmentId).ToList();
                _studentResponseService.UpdateAssignmentStatus(Array.ConvertAll(tentativelyScoredAssignmentIds.ToArray(), x => x.ToString()), status);
                
                List<string> notsuccessfullyScored = new List<string>();
                List<string> successfullyScored = new List<string>();

                //For assignments that are marked Completed, then send report
                foreach (var assignment in tentativelyScoredassignments)
                {
                    try
                    {
                        //save TIS callback request in debug mode
                        if (HttpContext.IsDebuggingEnabled)
                            LoggerRepository.SaveLog(new Log
                                                       {
                                                           Category = LogCategory.Application,
                                                           Level = LogLevel.Warning,
                                                           Message = assignment.CallbackUrl,
                                                           Details = _exportService.GetScoreReport(assignment).ToString()
                                                       });
                        _exportService.SendScoreReport(assignment);
                        successfullyScored.Add(assignment.AssignmentId.ToString());

                    }
                    catch (Exception ex)
                    {
                        // Add to not scored list if not successfully scored.
                        notsuccessfullyScored.Add(assignment.AssignmentId.ToString());
                        LoggerRepository.SaveLog(new Log
                        {
                            Category = LogCategory.Application,
                            Level = LogLevel.Error,
                            Message = "Unable to export score to scoring service.",
                            Details = ex.Message + "\r\n" + ex.StackTrace
                        });
                    }
                }

                //remove scored responses and assignments
                _studentResponseService.RemoveAssignments(successfullyScored.ToArray());
                //Mark as tentative if not scored.
                _studentResponseService.UpdateAssignmentStatus(notsuccessfullyScored.ToArray(), 1);

                return Json(new { success = true, data = "" });
            }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc);
                return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4001, exc.Message) });
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
                    return Json(new {success = false,data = String.Empty,message = "Item has already been marked as completed"});
                }
                #endregion

                #region Return error if item is not assigned to current user
                if (!(String.Equals(assignment.Teacher.TeacherID.Trim(), UserAttributes.SAML.TSSUserID.Trim(), StringComparison.CurrentCultureIgnoreCase))) // check if item is NOT assigned to current user
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
                LoggerRepository.LogException(exc);
                return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4004, exc.Message) });            
            }
		}

        public ActionResult ItemDetail(Guid id)
        {
            ItemDetailModel idm = new ItemDetailModel();
            idm.AssignmentId = id;

            return View(idm);
        }

		public ActionResult ItemView(Guid id)
		{
            try
            {
                ViewBag.Title = "Response Detail";

                var assignment = _studentResponseService.GetAssignmentById(id);

                #region Redirect to Response list if item is not assigned to current user
                if (!(String.Equals(assignment.Teacher.TeacherID.Trim(), UserAttributes.SAML.TSSUserID.Trim(), StringComparison.CurrentCultureIgnoreCase))) // check if item is NOT assigned to current user
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
                                    // Open source system does not support label attributes
                            // Label = assignment.StudentResponse.ItemKey.ToString(),
		                    Response = assignment.StudentResponse.Response
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
		        request.Passage = (passage == 0) ? null : 
                    new ContentRequestPassage()
		                {
		                    Id = "P-" +
                            itemType.BankKey.ToString() +
                            "-" + 
                            passage.ToString()
		                };

                itemDetailViewModel.ContentToken = JsonConvert.SerializeObject(request);
                return View(itemDetailViewModel);
		        }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc);
                throw new Exception("Error Code: 4005", exc);
            }
         
		}

	}
}
