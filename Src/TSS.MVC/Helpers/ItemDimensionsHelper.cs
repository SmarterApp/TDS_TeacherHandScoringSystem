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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Newtonsoft.Json;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Models;
using TSS.Services;

namespace TSS.MVC.Helpers
{
	public class ItemDimensionsHelper
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

			[JsonProperty("maxpoints")]
			public int maxpoints { get; set; }

		}



		public class RootObject
		{

			[JsonProperty("dimensions")]
			public List<Dimension> dimensions { get; set; }

		}

		internal static List<RootObject> FromJSonString(string jsonString)
		{
			var reader = new StringReader(jsonString);
			var rootObject = JsonConvert.DeserializeObject<List<RootObject>>(jsonString);

			return rootObject;
		}

        internal static List<ItemDimensionModel> GetItemDimensionList(string scoreData, ItemType itemType)
		{
			var itemDimensionModel = new List<ItemDimensionModel>();

			var assignmentScores = ScoringHelper.GetScore(scoreData);

            if (itemType != null)
            {
                var itemDimensions = itemType.Dimensions; // rootObject[0];

                for (int i = 0; i < itemDimensions.Count; i++)
                {
                    var score = string.Empty;
                    var conditioncode = string.Empty;
                    if (assignmentScores != null)
                    {
                        score = assignmentScores[i].Score;
                        conditioncode = assignmentScores[i].ConditionCode;
                    }
                    var newModel = new ItemDimensionModel
                                       {
                                           Dimension = itemDimensions[i].Name,
                                           Points = itemDimensions[i].Max,
                                           Score = score,
                                           ConditionCode = conditioncode
                                       };

                    newModel.ConditionCodes = new List<SelectListItem>();
                
                    for (int j = 0; j < itemDimensions[i].ConditionCodes.Count; j++)
                    {
                        newModel.ConditionCodes.Add(new SelectListItem
                                                        {
                                                            Value = itemDimensions[i].ConditionCodes[j].ShortName,
                                                            Text = itemDimensions[i].ConditionCodes[j].FullName
                                                        });
                    }

                    itemDimensionModel.Add(newModel);
                }
            }

            return itemDimensionModel;
		}



	}
}