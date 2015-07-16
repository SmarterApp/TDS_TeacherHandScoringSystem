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
using System.Net;
using System.Web;
using System.Web.Mvc;
using TSS.Data;
using TSS.Domain.DataModel;

namespace TSS.MVC.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            //Determine the return type of the action
            string actionName = filterContext.RouteData.Values["action"] as string;
            string controllerName = filterContext.RouteData.Values["controller"] as string;

            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            //If the exeption is already handled we do nothing (having try catch blocks in the Action itself)
            if (filterContext.ExceptionHandled)
            {
                return;
            }
            else
            {
                //If the action that generated the exception returns a view
                if (Request.IsAjaxRequest())
                {
                    filterContext.Result = new PartialViewResult()
                    {
                        ViewName = "Error",
                        ViewData = new ViewDataDictionary(model),
                        TempData = filterContext.Controller.TempData,
                    };
                }
                else
                {
                    filterContext.Result = new ViewResult()
                    {
                        ViewName = "Error",
                        ViewData = new ViewDataDictionary(model),
                        TempData = filterContext.Controller.TempData
                    };
                }
            }

            // Log.Write(filterContext.Exception);
            LoggerRepository.SaveLog(new Log
            {
                Category = LogCategory.Application,
                Level = LogLevel.Error,
                Message = string.Format("Ajax Error") + ","+model.ControllerName,
                Details = Convert.ToString(model.Exception)
            });
            //Make sure that we mark the exception as handled
            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            filterContext.HttpContext.Response.StatusDescription = HttpStatusCode.InternalServerError.ToString();
            base.OnException(filterContext);
            base.OnException(filterContext);
        }

    }
}
