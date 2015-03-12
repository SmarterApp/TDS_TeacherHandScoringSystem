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
using TSS.Domain.DataModel;

namespace TSS.Domain
{
    public class AssignmentResult
    {

        public string AssignedTeacherName { get; set; }
        public Guid AssignmentId { get; set; }
        public string ItemTypeDescription { get; set; }
        public Int64 ItemKey { get; set; }
        public string SessionId { get; set; }
        public StudentResponseAssignment.ScoreStatusCode ScoreStatus { get; set; }
        public string StudentName { get; set; }
        public string TeacherId { get; set; }
    }

    public class FilterResult
    {
        public string AssignedTeacherName { get; set; }
        public string AssignedTeacherID { get; set; }
        public string SessionId { get; set; }
        public string Grade { get; set; }
        public string Subject { get; set; }
        public string TestName { get; set; }
        public string TestID { get; set; }
    }
}

