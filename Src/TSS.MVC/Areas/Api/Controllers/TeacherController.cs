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
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using TSS.Data;
using TSS.Domain;
using TSS.MVC.Areas.Api.Models;
using TSS.Services;
using System.IO;
using System.Text;

namespace TSS.MVC.Areas.Api.Controllers
{
    public class TeacherController : Controller
    {

        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        //public ActionResult GetTeachersFromApi(int pageNo, int pageSize, string role, string entityId, string level, string state)
        //{
        //    var result = _teacherService.GetTeachersFromApi(pageNo, pageSize, role, entityId, level, state);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetScorersforReAssign()
        {
            try
            {
                //GET LIST OF SCORER ROLES
                List<RoleSet> roles = UserAttributes.SAML.GetListOfRolesThatCanViewItems();
                //GET USER'S TENANCIES
                List<TenancyChain> t = UserAttributes.SAML.TenancyChainList;
                //GET ALL SCORERS MATCHING ALL THE USER'S TENANCIES WHERE THE ROLES ARE OF TSS SCORER ROLES
                var r = _teacherService.GetListOfPossibleScorers(t, roles);

                if (r != null)
                {
                    r = r.OrderBy(e => e.firstName).ToList();
                }

                return Json(r, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                LoggerRepository.LogException(exc);
                return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4004, exc.ToString()) }, JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult GetScoreresToView()
        //{
        //    try
        //    {
        //        //GET LIST OF SCORER ROLES
        //        List<RoleSet> roles = UserAttributes.SAML.GetListOfRolesThatCanViewItems(); //WHAT ROLES DENOTE A TSS SCORER?
        //        //GET LIST OF USERS TENANCIES THAT CAN VIEW ALL
        //        //WHICH TENANCIES DOES THIS USER HAVE THAT ALLOW HIM TO SEE OTHERS?
        //        List<TenancyChain> t = UserAttributes.SAML.TenancyChainList.Where(x => UserAttributes.CanRoleViewAll(x)).ToList<TenancyChain>();
        //        //GET TEACHERS BASED ON THOSE TENANCIES AND ROLES
        //        var r = _teacherService.GetListOfPossibleScorers(t, roles);

        //        return Json(r, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception exc)
        //    {
        //        return Json(new { success = false, data = string.Format("\"Error Code\": \"{0}\"  \"Message\":\"{1}\"", 4003, exc.Message) }, JsonRequestBehavior.AllowGet);
        //    }
        //}





    }
}