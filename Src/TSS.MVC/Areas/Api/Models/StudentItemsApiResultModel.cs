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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSS.Domain;
using TSS.MVC.Helpers;
using TSS.Services;
using System.Text;

namespace TSS.MVC.Areas.Api.Models
{
    public class StudentItemsApiResultModel
    {

        public List<SelectListItem> Test { get; set; }
        public List<SelectListItem> Session { get; set; }
        public List<SelectListItem> Grade { get; set; }
        public List<SelectListItem> Subject { get; set; }
        public List<SelectListItem> TestAdministrator { get; set; }
        //public List<SelectListItem> Entitie { get; set; }
        public List<SelectListItem> ReassignTeacher { get; set; }
        public int RowCount { get; set; }
        public string District { get; set; }
        public string School { get; set; }
        public List<StudentItem> StudentItem { get; set; }
        public bool RefreshFilters { get; set; }
        public string IdList { get; set; }
        public StudentItemsApiResultModel()
        {
            StudentItem = new List<StudentItem>();
        }

        public StudentItemsApiResultModel(AssignmentPage page)
        {
            List<FilterResult> filters = page.FilterItems;
            
            this.IdList = page.AllAssignmentIds;
            this.StudentItem = new List<StudentItem>();
            this.RowCount = page.rowcount;
            this.Test = DropdownHelper.GetTssTests(filters);
            this.Session = DropdownHelper.GetTssSessions(filters);
            this.Grade = DropdownHelper.GetTssGrades(filters);
            this.Subject = DropdownHelper.GetTssSubjects(filters);
            this.TestAdministrator = DropdownHelper.GetTssTestAdministrators(filters);
            //this.Entitie = DropdownHelper.GetTssEntities().GroupBy(ts => ts.Text).Select(g => g.First()).ToList();
            this.ReassignTeacher = new List<SelectListItem>(); // DropdownHelper.GetTssReassignTeachers();
        }
    }
}
