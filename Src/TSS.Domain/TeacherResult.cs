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
namespace TSS.Domain
{
    [System.Runtime.Serialization.DataContractAttribute()]
    public class TeacherResult : ResultBase
    {
        [System.Runtime.Serialization.DataMemberAttribute()] public SearchResults[] searchResults;
        [System.Runtime.Serialization.DataMemberAttribute()] public int currentPage;
        [System.Runtime.Serialization.DataMemberAttribute()] public int returnCount;
        [System.Runtime.Serialization.DataMemberAttribute()] public int pageSize;
        [System.Runtime.Serialization.DataMemberAttribute()] public int totalCount;
        [System.Runtime.Serialization.DataMemberAttribute()] public string sortKey;
        [System.Runtime.Serialization.DataMemberAttribute()] public string sortDirection;
        [System.Runtime.Serialization.DataMemberAttribute()] public object nextPageUrl;
        [System.Runtime.Serialization.DataMemberAttribute()] public object prevPageUrl;
    }

    // Type created for JSON at <<root>> --> searchResults
    [System.Runtime.Serialization.DataContractAttribute(Name = "searchResults")]
    public class SearchResults
    {
        [System.Runtime.Serialization.DataMemberAttribute()] public string id;
        [System.Runtime.Serialization.DataMemberAttribute()] public string firstName;
        [System.Runtime.Serialization.DataMemberAttribute()] public string lastName;
        [System.Runtime.Serialization.DataMemberAttribute()] public string email;
        [System.Runtime.Serialization.DataMemberAttribute()] public string username;
        [System.Runtime.Serialization.DataMemberAttribute()] public string phone;
        [System.Runtime.Serialization.DataMemberAttribute()] public object delete;
        [System.Runtime.Serialization.DataMemberAttribute()] public object changeEventExportError;
        [System.Runtime.Serialization.DataMemberAttribute()] public RoleAssociations[] roleAssociations;
        [System.Runtime.Serialization.DataMemberAttribute()] public string alternateKey;
        [System.Runtime.Serialization.DataMemberAttribute()] public string formatType;
        [System.Runtime.Serialization.DataMemberAttribute()] public string action; 
        [System.Runtime.Serialization.DataMemberAttribute()] public string url;
    }

    // Type created for JSON at <<root>> --> roleAssociations
    [System.Runtime.Serialization.DataContractAttribute(Name = "roleAssociations")]
    public class RoleAssociations
    {

        [System.Runtime.Serialization.DataMemberAttribute()] public string associatedEntityMongoId;
        [System.Runtime.Serialization.DataMemberAttribute()] public string associatedEntityId;
        [System.Runtime.Serialization.DataMemberAttribute()] public string role;
        [System.Runtime.Serialization.DataMemberAttribute()] public object stateAbbreviation;
        [System.Runtime.Serialization.DataMemberAttribute()] public string level;
    }

    [System.Runtime.Serialization.DataContractAttribute()]
    public class EntityResultSet : ResultBase
    {
        public EntityResult[] EntityResults;
    }

    [System.Runtime.Serialization.DataContractAttribute()]
    public class EntityResult
    {
        [System.Runtime.Serialization.DataMemberAttribute()] public string id;
        [System.Runtime.Serialization.DataMemberAttribute()] public string entityId;
        [System.Runtime.Serialization.DataMemberAttribute()] public string entityName;
        [System.Runtime.Serialization.DataMemberAttribute()] public string parentEntityId;
        [System.Runtime.Serialization.DataMemberAttribute()] public string stateAbbreviation;
        [System.Runtime.Serialization.DataMemberAttribute()] public string nationwideIdentifier;
        [System.Runtime.Serialization.DataMemberAttribute()] public string parentId;
        [System.Runtime.Serialization.DataMemberAttribute()] public string formatType;
        [System.Runtime.Serialization.DataMemberAttribute()] public string tenantType;
        [System.Runtime.Serialization.DataMemberAttribute()] public string entityType;
        [System.Runtime.Serialization.DataMemberAttribute()] public string alternateKey;
        [System.Runtime.Serialization.DataMemberAttribute()] public string action;
        [System.Runtime.Serialization.DataMemberAttribute()] public string parentEntityType;
        //[System.Runtime.Serialization.DataMemberAttribute()] public string url;
    }


}

