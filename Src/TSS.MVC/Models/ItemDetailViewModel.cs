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
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TSS.Data;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Helpers;
using TSS.Services;

namespace TSS.MVC.Models
{
	public class ItemDetailViewModel
	{

        public string IriSBlackboxRootURL
        {
            get
            {
                return ConfigurationManager.AppSettings["IRISBlackbox_ROOT_URL"].ToString();
            }
        }

		#region Dropdown Lists

		public List<SelectListItem> ConditionCodes { get; set; }
		public List<RubricListModel> RubricList { get; set; }
		public List<ScoringCriteriaModel> ScoringCriteria { get; set; }
		public List<ItemDimensionModel> ItemDimensions { get; set; }
        
		#endregion

		#region Properties

		[Display(Name = "Condition Code")]
		public int ConditionCode { get; set; }

        [Display(Name = "Item")]
        public int ItemKey { get; set; }

		[Display(Name = "Assignment")]
		public Guid AssignmentId { get; set; }

		[Display(Name = "Desc")]
		public string ItemDesc { get; set; }

		[Display(Name = "Test")]
		public string Test { get; set; }

		[Display(Name = "Status")]
		public string ScoreStatus { get; set; }

		[Display(Name = "Session")]
		public string Session { get; set; }

		[Display(Name = "Student")]
		public string StudentName { get; set; }

        public string ContentToken { get; set; }

		public string ExemplarUrl { get; set; }

		public string TrainingGuideUrl { get; set; }

        public Guid AssignedAssignmentId { get; set; }

        public int NumberOfDimensions { get; set; }

        public string ScoreData { get; set; }

        public string Response { get; set; }

        public ItemType ItemType { get; set; }

	    public string VendorId
        {
            get
            {
                return ConfigurationManager.AppSettings["IRIS_VENDOR_ID"].ToString();
            }
        }

            /// <summary>
            /// this needs to be redefined in ItemView Class
            /// </summary>
        public string IRiSRootURL
        {
            get
            {
                return ConfigurationManager.AppSettings["IRIS_ROOT_URL"].ToString();
            }
        }

        public string IRISBlackbox_ROOT_URL
        {
            get
            {
                return ConfigurationManager.AppSettings["IRISBlackbox_ROOT_URL"].ToString();
            }
        }

		#endregion

		#region Constructors

        public ItemDetailViewModel(StudentResponseAssignment assignment)
		{
            List<ItemType> itemTypes = ItemConfigSingleton.Instance.LoadItemTypes();
            var itemType = itemTypes.SingleOrDefault(i => i.BankKey == assignment.StudentResponse.BankKey && i.ItemKey == assignment.StudentResponse.ItemKey);
            if (itemType != null) this.RubricList = RubricListHelper.GetRubricList(itemType.RubricListXML);
            this.ItemDimensions = ItemDimensionsHelper.GetItemDimensionList(assignment.ScoreData,itemType);

            //SET DROPDOWNS
            this.ItemDimensions.ForEach(delegate(ItemDimensionModel item) {
                item.ConditionCodes.ForEach(delegate(SelectListItem dropdownItems) {
                    //dropdownItems.Selected = (dropdownItems.Text == item.ConditionCode);
                    dropdownItems.Selected = (dropdownItems.Value == item.ConditionCode);

                });
            });
             
           
			this.AssignmentId = assignment.AssignmentId;
		    this.ItemKey = assignment.StudentResponse.ItemKey;
            if (itemType != null)
            {
                this.ItemType = itemType;
                this.ItemDesc = itemType.Description;
                this.ExemplarUrl = itemType.ExemplarURL;
                this.TrainingGuideUrl = itemType.TrainingGuideURL;
            }
            this.NumberOfDimensions = this.ItemDimensions.Count;
            this.Test = assignment.Test.Name;
			this.Session = assignment.SessionId;
            this.StudentName = assignment.Student.Name;
			this.AssignedAssignmentId = assignment.AssignmentId;
            this.Response = HttpUtility.HtmlEncode(assignment.StudentResponse.Response);
            
            this.ScoreStatus = (assignment.ScoreStatus == StudentResponseAssignment.ScoreStatusCode.NotScored ? "Not Scored" : assignment.ScoreStatus == StudentResponseAssignment.ScoreStatusCode.TentativeScore ? "Tentatively Scored" : "Scored");

		}

		#endregion

	}
}
