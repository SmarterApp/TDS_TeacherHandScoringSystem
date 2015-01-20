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

namespace TSS.Domain
{
    public class AssignedItemsQuery
    {
        public int pageNumber { get; set; }
        public int pageLength { get; set; }
        public Dictionary<string, string> filters { get; set; }
        public List<string> teacherUUIDs { get; set; }
        public string sortColumn { get; set; }
        public string sortDirection { get; set; }
        public bool hasRun { get; set; }
        public string UserUUID { get; set; }
        public bool GetFilters { get; set; }

        public AssignedItemsQuery(int _pageNumber, int _pageLength, List<string> _teacherUUIDs ,Dictionary<string, string> _filters)
        {
            this.pageNumber = _pageNumber;
            this.pageLength = _pageLength;
            this.filters = _filters;
            this.teacherUUIDs = _teacherUUIDs;
        }

        public AssignedItemsQuery()
        { }
    }
}
