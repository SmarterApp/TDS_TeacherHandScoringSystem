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

namespace TSS.MVC.Models
{
	public class ScoringCriteriaModel
	{

		[Display(Name = "ID")]
		public int ScoringCriteriaId { get; set; }

		[Display(Name = "Scoring Criteria")]
		public string ScoringCriteria { get; set; }

		[Display(Name = "Points")]
		public int Points { get; set; }

		[Display(Name = "Score")]
		public string Score { get; set; }

		//The following ConditionCodes may be implemented in the future.
		public List<SelectListItem> ConditionCodes { get; set; }

		public string ConditionCode { get; set; }
	}
}