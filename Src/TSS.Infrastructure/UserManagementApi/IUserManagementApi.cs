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
using TSS.Domain;

namespace TSS.Data
{
    public interface IUserManagementApi
    {
        EntityResultSet GetEntitiesFromApi(string entityType, string parentEntityType, string parentEntityId);
        TeacherResult GetTeachersFromApi(int pageNumber, int pageSize, string role, string associatedEntityId, string level, string state);
        //UserPermissions GetUserPermissions();
    }
}

