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

	[XmlRoot("rubriclist")]
	public sealed class RubricListHelper
	{
		[XmlElement("rubric", Type = typeof(Rubric))]
		public Rubric[] Rubrics { get; set; }

		[XmlElement("samplelist", Type = typeof(SampleList))]
		public SampleList[] SampleLists { get; set; }

		public RubricListHelper()
		{
			Rubrics = null;
			SampleLists = null;
		}

		internal static RubricListHelper FromXmlString(string xmlString)
		{
			var reader = new StringReader(xmlString);
			var serializer = new XmlSerializer(typeof(RubricListHelper));
			var instance = (RubricListHelper)serializer.Deserialize(reader);

			return instance;
		}

		internal static List<RubricListModel> GetRubricList(string rubricListXML)
		{
			var rubricListModel = new List<RubricListModel>();
	
			//Get the rubriclist for the item
			var rubricListXml = rubricListXML;

			//Load up the RubricList
			var rubricList = RubricListHelper.FromXmlString(rubricListXml);

			for (int i = 0; i < rubricList.SampleLists.Length; i++)
			{
				rubricListModel.Add(new RubricListModel
				{
					RubricScorePoint = (rubricList.Rubrics[i].ScorePoint.Equals(string.Empty)) ? 0 : int.Parse(rubricList.Rubrics[i].ScorePoint),
					RubricName = rubricList.Rubrics[i].Name,
					RubricDescription = HttpUtility.HtmlDecode(rubricList.Rubrics[i].Val),
					MaxVal = (rubricList.SampleLists[i].MaxVal.Equals(string.Empty)) ? 0 : int.Parse(rubricList.SampleLists[i].MaxVal)
				});
			}

			
			return rubricListModel;
		}

	}

	[Serializable]
	public class Rubric
	{
		[XmlAttribute("scorepoint")]
		public string ScorePoint { get; set; }

		[XmlAttribute("index")]
		public string Index { get; set; }

		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("val")]
		public string Val { get; set; }

		public Rubric()
		{
		}
	}


	[Serializable]
	public sealed class SampleList
	{
		[XmlElement("sample", Type = typeof(Sample))]
		public Sample[] Samples { get; set; }

		public SampleList()
		{
			Samples = null;
		}

		[XmlAttribute("maxval")]
		public string MaxVal { get; set; }

		[XmlAttribute("minval")]
		public string MinVal { get; set; }

		[XmlAttribute("index")]
		public string Index { get; set; }

	}

	[Serializable]
	public class Sample
	{
		[XmlAttribute("purpose")]
		public string Purpose { get; set; }

		[XmlAttribute("scorepoint")]
		public string ScorePoint { get; set; }

		[XmlElement("name")]
		public string Name { get; set; }

		[XmlElement("annotation")]
		public string Annotation { get; set; }

		[XmlElement("samplecontent")]
		public string SampleContent { get; set; }


		public Sample()
		{
		}
	}

}