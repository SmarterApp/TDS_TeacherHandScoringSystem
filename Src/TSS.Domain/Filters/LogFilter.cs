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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSS.Domain.DataModel;

namespace TSS.Domain.Filters
{
    public class LogFilter : BaseFilter
    {
        public LogFilter()
        {
            SortColumn = "LogDate";
            SortDirection = Domain.SortDirection.Descending;
        }

        /// <summary>
        /// Filter logs created from a specific user's actions
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Filter a specific category
        /// </summary>
        public LogCategory? LogCategory { get; set; }

        /// <summary>
        /// Filter a specific level
        /// </summary>
        public LogLevel? LogLevel { get; set; }

        /// <summary>
        /// Filter logs from this date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Filter logs up to this date
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Search details field
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Search message field
        /// </summary>
        public string Message { get; set; }
    }
}

