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
using System.Collections.Generic;


namespace TSS.Domain
{
    public class AssignmentPage
    {
        public List<AssignmentResult> Assignments { get; set; }
        public List<FilterResult> FilterItems { get; set; }
        public string AllAssignmentIds { get; set; }
        public int rowcount { get; set; }
        public AssignmentPage() { }

        

    }
}
