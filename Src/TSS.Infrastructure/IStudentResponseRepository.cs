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
using TSS.Domain;
using TSS.Domain.DataModel;

namespace TSS.Data
{
    public interface IStudentResponseRepository
    {
        AssignmentPage GetAssignmentsByAssignedToTeacherIDList(AssignedItemsQuery query);
        StudentResponseAssignment GetAssignmentById(Guid assignmentId);
        AssignmentPage GetSortedAssignmentIds(AssignedItemsQuery query);
        bool ReAssign(string[] assignmentIds, Teacher teacher);
        bool RemoveAssignments(string[] assignmentIDs);
        IList<StudentResponseAssignment> GetAssignmentsByIDList(string[] assignmentIDs);
        bool UpdateAssignmentStatus(string[] assignmentIDs, int status);
        bool UpdateAssignment(StudentResponseAssignment assignment);
        List<ItemGroupEntry> GetResponsesFromItemGroup(StudentResponseAssignment assignment, int passage);
    }
}
