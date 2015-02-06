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
using TSS.Domain;
using TSS.MVC.Areas.Api.Controllers;
using TSS.Services;

namespace TSS.MVC.Helpers
{
	public class DropdownHelper
	{

		#region Constants

		public const int DefaultSelector = -1;
		
		#endregion

		#region Internal Methods

        internal static List<SelectListItem> GetTssTests(List<FilterResult> filterItems)
		{

            var tssTests = new List<SelectListItem>();
            foreach(FilterResult filterResult in filterItems)
            {
                if (tssTests.Any(t => t.Value == filterResult.TestID)) continue;
                var selectListItem = new SelectListItem();
                selectListItem.Value = filterResult.TestID;
                selectListItem.Text = filterResult.TestName;
                tssTests.Add(selectListItem);
            }
            InsertBlankSelectItem(tssTests, "Test");
            return tssTests;
		}

        internal static List<SelectListItem> GetTssSessions(List<FilterResult> filterItems)
		{
            var tssSessions = new List<SelectListItem>();
            foreach (FilterResult filterResult in filterItems)
            {
                if (tssSessions.Any(t => t.Value.ToLower() == filterResult.SessionId.ToLower())) continue;
                var selectListItem = new SelectListItem();
                selectListItem.Value = filterResult.SessionId;
                selectListItem.Text = filterResult.SessionId;
                tssSessions.Add(selectListItem);
            }

            InsertBlankSelectItem(tssSessions, "Session");
			return tssSessions;
		}

        internal static List<SelectListItem> GetTssGrades(List<FilterResult> filterItems)
		{

            var tssGrades = new List<SelectListItem>();
            foreach (FilterResult filterResult in filterItems)
            {
                if (tssGrades.Any(t => t.Value == filterResult.Grade)) continue;
                var selectListItem = new SelectListItem();
                selectListItem.Value = filterResult.Grade;
                selectListItem.Text = filterResult.Grade;
                tssGrades.Add(selectListItem);
            }

            InsertBlankSelectItem(tssGrades,"Grades");

			return tssGrades;
		}

        internal static List<SelectListItem> GetTssSubjects(List<FilterResult> filterItems)
		{

            var tssSubjects = new List<SelectListItem>();
            foreach (FilterResult filterResult in filterItems)
            {
                if (tssSubjects.Any(t => t.Value == filterResult.Subject)) continue;
                var selectListItem = new SelectListItem();
                selectListItem.Value = filterResult.Subject;
                selectListItem.Text = (filterResult.Subject.ToUpper() == "EL") ?filterResult.Subject.ToUpper() + "A" : filterResult.Subject.ToUpper() ; 
                tssSubjects.Add(selectListItem);
            }


            InsertBlankSelectItem(tssSubjects,"Subjects");

			return tssSubjects;
		}

        internal static List<SelectListItem> GetTssTestAdministrators(List<FilterResult> filterItems)
		{
            var tssTestAdministrators = new List<SelectListItem>();
            
            foreach (FilterResult filterResult in filterItems)
            {
                if (tssTestAdministrators.Any(t => t.Value == filterResult.AssignedTeacherID)) continue;
                var selectListItem = new SelectListItem();
                selectListItem.Value = filterResult.AssignedTeacherID;
                selectListItem.Text = filterResult.AssignedTeacherName;
                tssTestAdministrators.Add(selectListItem);
            }


            InsertBlankSelectItem(tssTestAdministrators, "Scorers");

			return tssTestAdministrators;
		}

		internal static List<SelectListItem> GetTssEntities()
		{

			var tssEntities = new List<SelectListItem>
			{
				new SelectListItem { Value="State", Text="State" },
				new SelectListItem { Value="District", Text="District" },
				new SelectListItem { Value="School", Text="School" }
			};

			InsertBlankSelectItem(tssEntities);



			return tssEntities;
		}

		internal static List<SelectListItem> GetTssReassignTeachers()
		{
            var tssReassignTeachers = new List<SelectListItem>();

            InsertBlankSelectItem(tssReassignTeachers);

            return tssReassignTeachers;
		}

		internal static List<SelectListItem> GetTssConditionCodes()
		{

            var tssConditionCodess = new List<SelectListItem>();

			return tssConditionCodess;
		}

		#endregion

		#region Private Methods

		private static void InsertBlankSelectItem(List<SelectListItem> selectItemList)
		{
			selectItemList.Insert(0, new SelectListItem { Value = "-1", Text = "Select .." });
		}

        private static void InsertBlankSelectItem(List<SelectListItem> selectItemList, string name)
        {
            selectItemList.Insert(0, new SelectListItem { Value = "-1", Text = string.Format("Select {0}...",name) });
        }


		#endregion


	}
}