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
using Newtonsoft.Json;

namespace TSS.Domain
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ContentRequest
    {
        [JsonProperty(PropertyName = "passage")]
        public object Passage { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<ContentRequestItem> Items { get; set; }

        [JsonProperty(PropertyName = "layout")]
        public string Layout { get; set; }
    }

    public abstract class ContentRequestEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        // open source system does not believe in label property

    }

    public class ContentRequestPassage : ContentRequestEntity
    {
        public override string ToString()
        {
            return Id;
        }
    }

    public class ContentRequestItem : ContentRequestEntity
    {
        [JsonProperty(PropertyName = "response")]
        public string Response { get; set; }

    }
}
