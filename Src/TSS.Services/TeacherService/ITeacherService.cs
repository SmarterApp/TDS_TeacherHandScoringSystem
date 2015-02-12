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

namespace TSS.Services
{
    public interface ITeacherService
    {
        IPagedList<Teacher> GetTeachers();
        ATeacher PopulateTeacherFromTdsReport(TDSReport tdsReport);
        bool SaveTeacher(ATeacher aTeacher);

		TeacherResult GetTeachersFromApi(int pageNumber, int pageSize, string role, string associatedEntityId, string level, string state);
        List<SearchResults> GetListOfPossibleScorers(List<TenancyChain> tens, List<RoleSet> roles);
        Teacher GetTeacherByUUID(Teacher teacher);
        List<Teacher> GetTearchersByUUIDs(IEnumerable<string> emailList);
       
    }
}

