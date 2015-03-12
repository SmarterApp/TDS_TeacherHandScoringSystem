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
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TSS.Domain
{
    public class ItemTypeJson
    {

    	public class Condition
		{

			[JsonProperty("code")]
			public string code { get; set; }

			[JsonProperty("description")]
			public string description { get; set; }

		}



		public class Dimension
		{

			[JsonProperty("conditions")]
			public List<Condition> conditions { get; set; }

			[JsonProperty("description")]
            public string description { get; set; }

            [JsonProperty("minpoints")]
            public int minpoints { get; set; }

            [JsonProperty("maxpoints")]
            public int maxpoints { get; set; }

		}



		public class RootObject
		{

            [JsonProperty("baseUrl")]
            public string baseUrl { get; set; }

            [JsonProperty("itemId")]
            public int itemId { get; set; }

            [JsonProperty("bankKey")]
            public int bankKey { get; set; }

            [JsonProperty("passage")]
            public string passage { get; set; }

            [JsonProperty("handScored")]
            public string handScored { get; set; }

            [JsonProperty("subject")]
            public string subject { get; set; }

            [JsonProperty("grade")]
            public string grade { get; set; }

            [JsonProperty("description")]
            public string description { get; set; }

            [JsonProperty("scoreType")]
            public string scoreType { get; set; }

            [JsonProperty("exemplar")]
            public string exemplar { get; set; }

            [JsonProperty("trainingGuide")]
            public string trainingGuide { get; set; }

            [JsonProperty("rubricList")]
            public string rubricList { get; set; }

			[JsonProperty("dimensions")]
            public List<Dimension> dimensions { get; set; }

            [JsonProperty("layout", NullValueHandling = NullValueHandling.Ignore)]
            public string Layout { get; set; }

		}
    }
}

