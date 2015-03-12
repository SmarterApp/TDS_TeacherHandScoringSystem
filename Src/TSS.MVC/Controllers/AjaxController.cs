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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using TSS.Data;
using TSS.Domain.DataModel;
using TSS.MVC.Helpers;
using TSS.Services;

namespace TSS.MVC.Controllers
{
    /// <summary>
    /// http://www.codeguru.com/csharp/.net/net_asp/mvc/working-with-asynchronous-operations-in-asp.net-mvc.htm
    /// http://stackoverflow.com/questions/4428413/why-would-multiple-simultaneous-ajax-calls-to-the-same-asp-net-mvc-action-cause
    /// </summary>
    [SessionState(SessionStateBehavior.Disabled)]
    public class AjaxController : Controller
    {
     
		public readonly IExportService _exportService;
	    private readonly IStudentResponseService _studentResponseService;

        public AjaxController(IExportService exportService, IStudentResponseService studentResponseService)
		{
            _exportService = exportService;
		    _studentResponseService = studentResponseService;
		}
        //
        // GET: /Ajax/
        public ActionResult Index()
        {
            ViewBag.Title = "Index";
            return View();
        }

        [HttpPost]
        public JsonResult ReassignScorer(string[] ids, string scorerUUID, string name)
        {
            try
            {
                Teacher teacher = new Teacher() { Name = name, TeacherID = scorerUUID };
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
    }
}

