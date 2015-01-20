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
    [System.Runtime.Serialization.DataContractAttribute(Name = "oAuthToken")]
    public partial class oAuthToken
    {
       [System.Runtime.Serialization.DataMemberAttribute()]
        public string scope;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int expires_in;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string token_type;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string refresh_token;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string access_token;
    }
}
