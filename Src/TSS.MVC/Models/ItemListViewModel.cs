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

namespace TSS.MVC.Models
{
	public class ItemListViewModel
	{
		#region Dropdown Lists

		public List<SelectListItem> Tests { get; set; }
		public List<SelectListItem> Sessions { get; set; }
		public List<SelectListItem> Grades { get; set; }
		public List<SelectListItem> Subjects { get; set; }
		public List<SelectListItem> TestAdministrators { get; set; }
		public List<SelectListItem> Entities { get; set; }
		public List<SelectListItem> ReassignTeachers { get; set; }

		#endregion

		#region Selected Values

		[Display(Name = "Test:")]
		public int Test { get; set; }

		[Display(Name = "Session:")]
		public int Session { get; set; }

		[Display(Name = "Grade")]
		public int Grade { get; set; }

		[Display(Name = "Subject")]
		public int Subject { get; set; }

		[Display(Name = "Test Administrator")]
		public int TestAdministrator { get; set; }

		[Display(Name = "Entity Filter")]
		public int Entity { get; set; }

		[Display(Name = "Reassign Selected Responses To")]
		public int ReassignTeacher { get; set; }

		public bool IsAdmin { get; set; }
        public string TSSUserID { get; set; }
        public bool UserEmailAsUuid { get; set; }

	    #endregion

		#region Constructors

        public ItemListViewModel()
		{
			//See if the logged in user is an Admin
			this.IsAdmin = UserAttributes.SAML.IsAdministrator;
            this.TSSUserID = UserAttributes.SAML.TSSUserID;
            this.UserEmailAsUuid = UserAttributes.UseEmailAsUuid;

		}

	    #endregion


	}
}
