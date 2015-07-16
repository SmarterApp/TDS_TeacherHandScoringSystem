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
using TSS.Domain;

namespace TSS.MVC.Areas.Api.Models
{
    public class StudentItem
    {
        /// <summary>
        /// Public properties
        /// </summary>
        public virtual Guid AssignmentID { get; set; }
        public virtual Int64 StudentId { get; set; }
        public virtual string StudentName { get; set; }
        public virtual Guid ItemId { get; set; }
        public virtual Int64 ItemKey { get; set; }
        public virtual Int64 ItemBank { get; set; }
        public virtual string Item { get; set; }
        public virtual string Session { get; set; }
        public virtual string StatusId { get; set; }
        public virtual string Status { get; set; }
        public virtual string AssignedTo { get; set; }
        public virtual string School { get; set; }
        public virtual string Grade { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Test { get; set; }
        public virtual bool CanScore { get; set; }
        public virtual string TeacherId { get; set; }
    }
}
