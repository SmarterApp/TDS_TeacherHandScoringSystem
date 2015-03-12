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
    public partial class UserPermissions : ResultBase
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public object message;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string status;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public RoleSet[] value;

        public RoleSet[] roleSet
        {
            get { return this.value; }
        }



    }


    [System.Runtime.Serialization.DataContractAttribute(Name = "roleSet")]
    public partial class RoleSet
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public AllowableEntities[] allowableEntities;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string role;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Mappings[] mappings;
    }

    
    [System.Runtime.Serialization.DataContractAttribute(Name = "allowableEntities")]
    public partial class AllowableEntities
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string entity;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string description;
    }

   
    [System.Runtime.Serialization.DataContractAttribute(Name = "mappings")]
    public partial class Mappings
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Permissions[] permissions;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string component;
    }

   
    [System.Runtime.Serialization.DataContractAttribute(Name = "permissions")]
    public partial class Permissions
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string name;
    }


}

