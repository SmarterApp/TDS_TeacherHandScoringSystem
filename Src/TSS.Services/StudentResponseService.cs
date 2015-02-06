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
using TSS.Data;
using TSS.Data.DataDistribution;
using TSS.Domain;
using TSS.Domain.DataModel;

namespace TSS.Services
{
    public class StudentResponseService : IStudentResponseService
    {
        private readonly IStudentResponseRepository _studentResponseRepository;

        public StudentResponseService(IStudentResponseRepository studentResponseRepository)
        {
            _studentResponseRepository = studentResponseRepository;
        }
        public bool ReAssign(string[] assignmentIDs, Teacher teacher)
        {
            return _studentResponseRepository.ReAssign(assignmentIDs, teacher);
        }
        public bool RemoveAssignments(string[] assignmentIDs)
        {
            return _studentResponseRepository.RemoveAssignments(assignmentIDs);
        }

        public IList<StudentResponseAssignment> GetAssignmentsByIDList(string[] assignmentIDs)
        {
            return _studentResponseRepository.GetAssignmentsByIDList(assignmentIDs);
        }

        public bool UpdateAssignmentStatus(string[] assignmentIDs, int status)
        {
            return _studentResponseRepository.UpdateAssignmentStatus(assignmentIDs, status);
        }
        public bool UpdateAssignment(StudentResponseAssignment assignment)
        {
            return _studentResponseRepository.UpdateAssignment(assignment);
        }

        public StudentResponseAssignment GetAssignmentById(Guid assignmentId)
        {
            return _studentResponseRepository.GetAssignmentById(assignmentId);
        }

        public AssignmentPage GetAssignmentsByAssignedToTeacherIDList(AssignedItemsQuery query)
        {
            return _studentResponseRepository.GetAssignmentsByAssignedToTeacherIDList(query);
        }
        public List<ItemGroupEntry> GetResponsesFromItemGroup(StudentResponseAssignment assignment, int passage)
         {
             return _studentResponseRepository.GetResponsesFromItemGroup(assignment, passage);
             
         }



    }
}
