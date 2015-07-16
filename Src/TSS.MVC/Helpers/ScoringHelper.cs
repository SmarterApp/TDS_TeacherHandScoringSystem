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
using System.Xml.Serialization;
using TSS.Data;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.MVC.Models;

namespace TSS.MVC.Helpers
{
	[XmlRoot("score")]
	public sealed class ScoringHelper
	{
		[XmlAttribute("type")]
		public string Type { get; set; }

		[XmlElement("dimension", Type = typeof(ScoreDimension))]
		public List<ScoreDimension> Dimensions { get; set; }

		public ScoringHelper()
		{
			Dimensions = new List<ScoreDimension>();
			Type = string.Empty;
		}

		internal static ScoringHelper FromXmlString(string xmlString)
		{
			var reader = new StringReader(xmlString);
			var serializer = new XmlSerializer(typeof(ScoringHelper));
			var instance = (ScoringHelper)serializer.Deserialize(reader);

			return instance;
		}

		internal static string ToXmlString(ScoringHelper scores)
		{
			var serializer = new XmlSerializer(typeof(ScoringHelper));

			TextWriter writer = new StringWriter();

			serializer.Serialize(writer, scores);

			return writer.ToString();
		}

        internal static List<ScoringCriteriaModel> GetScore(string scoreXml)
		{
			var scoreListModels = new List<ScoringCriteriaModel>();
            
			//If no score yet, then bail out.
			if (string.IsNullOrEmpty(scoreXml))
			{
				return null;
			}

			//Load up the RubricList
			var scores = ScoringHelper.FromXmlString(scoreXml);

			for (int i = 0; i < scores.Dimensions.Count; i++)
			{
                string conditionCode = scores.Dimensions[i].ConditionCode;
                if (conditionCode == "NA") conditionCode = "";
				scoreListModels.Add(new ScoringCriteriaModel
				{
					ConditionCode = conditionCode,
					Score = scores.Dimensions[i].Score,
					ScoringCriteria = scores.Dimensions[i].Name
				});
			}

			return scoreListModels;
		}

	}

	[Serializable]
	public class ScoreDimension
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlElement("score")]
		public string Score { get; set; }

		[XmlElement("conditioncode")]
		public string ConditionCode { get; set; }

		public ScoreDimension()
		{
		}
	}



}
