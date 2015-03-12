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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.Services;

namespace TSS.Services
{
    public class ExportService : IExportService
    {
        //private readonly IItemService _itemService;

        public ExportService() {
            
        }

        public void SendScoreReport(StudentResponseAssignment assignment)
        {
            XDocument scoreReport = GetScoreReport(assignment);
            string data = scoreReport.ToString();

            string url = assignment.CallbackUrl;

            WebRequest req = System.Net.WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);
            req.ContentLength = bytes.Length;
            using (System.IO.Stream os = req.GetRequestStream())
            {
                os.Write(bytes, 0, bytes.Length);
                os.Close();
            }

            var resp = (HttpWebResponse)req.GetResponse();
            if (resp == null)
            {
                throw new Exception("No response from Score Report URL " + url);
            }

            return;
        }

        public XDocument GetScoreReport(StudentResponseAssignment assignment)
        {
            var item = assignment.StudentResponse;
            var status = StudentResponseAssignment.ScoreStatusCode.Scored.ToString();
            string scorerId = assignment.Teacher.TeacherID.ToString();

            var scoreXml = XDocument.Parse(assignment.ScoreData); //XDocument.Load("Samples/score-rubric.xml");
            int score = GetTotalScore(scoreXml);
            var dimensions = scoreXml.Element("score").Elements("dimension");
            int numDimensions = dimensions.Count();
            XElement scoreInfo = null;

            scoreInfo = CreateScoreInfo(scorerId, score, "Overall", status, numDimensions == 1 ? dimensions.First() : null);
            AddDimensions(scorerId, status, scoreInfo, dimensions);

            // Context token
            var context = new XElement("ContextToken");
            JObject j = new JObject();
            j.Add("clientName", assignment.ClientName);
            j.Add("oppKey", assignment.OpportunityKey.ToString());
            //j.Add("oppID", assignment.OpportunityId);
            j.Add("itemBank", assignment.StudentResponse.BankKey);
            j.Add("itemID", assignment.StudentResponse.ItemKey);
            j.Add("itemType", assignment.StudentResponse.Format);
            context.Add(new XCData(JsonConvert.SerializeObject(j)));

            var scoreElement = new XElement("Score", scoreInfo);
            scoreElement.Add(context);
            return new XDocument(new XElement("ItemScoreResponse", scoreElement));
        }

        #region Private Helpers

        private XElement CreateScoreInfo(string scorerId, int? point, string dimension, string status, XElement dimNode = null) {
            var scoreInfo = new XElement("ScoreInfo");

            string pointString = point.HasValue ? point.Value.ToString() : "0";
            scoreInfo.SetAttributeValue("scorePoint", pointString);
            scoreInfo.SetAttributeValue("scoreDimension", dimension);
            scoreInfo.SetAttributeValue("scoreStatus", status);

            if (dimension == "Initial" || dimNode != null)
            {
                string conditionCode = string.Empty;
                if (dimNode != null)
                {
                    conditionCode = dimNode.Element("conditioncode").Value;
                }
                if (!string.IsNullOrEmpty(conditionCode) || dimension == "Initial")
                    scoreInfo.Add(CreateRationale(scorerId, conditionCode, dimension));
            }
            return scoreInfo;
        }

        private XElement CreateRationale(string scorerID, string conditionCode, string dimensionName)
        {
            if (conditionCode == "NA") conditionCode = "";
            var rationale = new XElement("ScoreRationale");
            var m = new XElement("Message");
            var cdata = new XCData("Message");
            JObject j = new JObject();
            if (dimensionName == "Initial") j.Add("scorerID", scorerID);
            if (!String.IsNullOrEmpty(conditionCode)) j.Add("conditionCode", conditionCode);
            cdata.Value = JsonConvert.SerializeObject(j);
            m.Add(cdata);
            rationale.Add(m);
            return rationale;
        }

        private void AddDimensions(string scorerId, string status, XElement scoreInfo, IEnumerable<XElement> dimensions)
        {
            var subScoreList = new XElement("SubScoreList");
            foreach(var dim in dimensions)
            {
                int? score = null; 
                if (dim.Element("score") != null && !String.IsNullOrEmpty(dim.Element("score").Value))
                    score = Convert.ToInt32(dim.Element("score").Value);
                // for cases with one dimension score response, it could have attribute name as correct response. 
                string name = (dim.Attribute("name") != null && dimensions.Count() > 1) ? dim.Attribute("name").Value : "Initial";
                string conditionCode = dim.Element("conditioncode").Value;
                if (conditionCode == "NA") conditionCode = "";
                XElement rationaleNode = null;
                if (!String.IsNullOrEmpty(conditionCode)) rationaleNode = dim;

                XElement child = CreateScoreInfo(scorerId, score, name, status, rationaleNode);

                if (name != "Initial" )
                {
                    var grandChild = new XElement("SubScoreList");
                    XElement grandChildScoreInfo = CreateScoreInfo(scorerId, score, "Initial", status, dim);
                    grandChildScoreInfo.Add(new XElement("SubScoreList"));
                    grandChild.Add(grandChildScoreInfo);
                    child.Add(grandChild);
                }
                else if (rationaleNode != null && dimensions.Count() >1 )
                {
                    child.Add(CreateRationale(scorerId, conditionCode, name));
                }

                subScoreList.Add(child);
            }
            scoreInfo.Add(subScoreList);
        }

        private int GetTotalScore(XDocument scoreDoc)
        {
            int totalScore = 0;
            foreach (var dim in scoreDoc.Element("score").Elements("dimension"))
            {
                if (dim.Element("score") == null || String.IsNullOrEmpty(dim.Element("score").Value)) continue;
                totalScore += Convert.ToInt32(dim.Element("score").Value);
            }
            return totalScore;
        }

        #endregion

    }
}

