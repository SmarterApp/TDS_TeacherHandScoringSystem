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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using TSS.Data.Sql;
using TSS.Domain;
using TSS.Domain.DataModel;
using TSS.Data.DataDistribution;

namespace TSS.Data
{
    public class ItemConfigRepository : BaseRepository,IItemConfigRepository
    {

        public ItemConfigRepository()
        {
        }

        public List<ItemType> GetItemConfiguraitons()
        {
            List<ItemType> itemTypes = new List<ItemType>();
            List<ConditionCode> conditionCodes = new List<ConditionCode>();
            List<Dimension> dimensions = new List<Dimension>();
            SqlCommand command = CreateCommand(CommandType.StoredProcedure, "[dbo].[sp_GetItemConfigurations]");

            ExecuteReader(command, delegate(IColumnReader reader)
                                       {
                                           reader.FixNulls = true;
                                           //load conditon codes
                                           while (reader.Read())
                                           {
                                               
                                               ConditionCode conditionCode = new ConditionCode()
                                                                                  {
                                                                                      ConditionCodeId = reader.GetInt32("ConditionCodeId"),
                                                                                      DimensionId = reader.GetInt32("DimensionId"),
                                                                                      FullName = reader.GetString("FullName"),
                                                                                      ShortName = reader.GetString("ShortName")
                                                                                  };

                                               conditionCodes.Add(conditionCode);
                                           }
                                           if (reader.NextResult())
                                           {
                                               
                                               while (reader.Read())
                                               {
                                                   Dimension dimension = new Dimension()
                                                                              {
                                                                                  DimensionId = reader.GetInt32("DimensionId"),
                                                                                  BankId = reader.GetInt32("BankKey"),
                                                                                  ItemKey = reader.GetInt32("ItemKey"),
                                                                                  Name = reader.GetString("Name"),
                                                                                  Max = reader.GetInt32("Max"),
                                                                                  Min = reader.GetInt32("Min"),
                                                                                  ConditionCodes = new List<ConditionCode>()
                                                                              };
                                                   if (conditionCodes.Any(c => c.DimensionId == dimension.DimensionId))
                                                       dimension.ConditionCodes = conditionCodes.Where(c => c.DimensionId == dimension.DimensionId).ToList();
                                                   dimensions.Add(dimension);
                                               }
                                           }
                                           if (reader.NextResult())
                                           {
                                               while (reader.Read())
                                               {
                                                   
                                                   ItemType itemType = new ItemType()
                                                                            {
                                                                                BankKey = reader.GetInt32("BankKey"),
                                                                                ItemKey = reader.GetInt32("ItemKey"),
                                                                                Passage = reader.GetInt32("Passage"),
                                                                                // If HandScored is NULL, it must have been imported
                                                                                // before, so treat it as true
                                                                                HandScored = 
                                                                                    reader.IsDBNull("HandScored") || 
                                                                                    reader.GetBoolean("HandScored"),

                                                                                Description = reader.GetString("Description"),
                                                                                ExemplarURL = reader.GetString("ExemplarURL"),
                                                                                Grade = reader.GetString("Grade"),
                                                                                Subject = reader.GetString("Subject"),
                                                                                Layout = reader.GetString("Layout"),
                                                                                RubricListXML = reader.GetString("RubricListXML"),
                                                                                TrainingGuideURL = reader.GetString("TrainingGuideURL"),
                                                                                Dimensions = new List<Dimension>()
                                                                            };

                                                   if (dimensions.Any(d => d.ItemKey == itemType.ItemKey && d.BankId == itemType.BankKey))
                                                       itemType.Dimensions = dimensions.Where(d => d.ItemKey == itemType.ItemKey && d.BankId == itemType.BankKey).ToList();
                                                   itemTypes.Add(itemType);
                                               }
                                           }
                                       });

            return itemTypes;
        }

        public void UpdateSql<T>(List<T> aList, string sproc)
        {
            Encoding encoding = new UnicodeEncoding();
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (List<T>));
                using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, encoding))
                {

                    xmlSerializer.Serialize(xmlWriter, aList);
                    string xmlString = encoding.GetString(ms.ToArray());
                    // District code is back in.
                    List<string> districts = DataConnections.DistrictLookUp.Keys.ToList();
                    // If the default is not associated with a district, put a dummy
                    // value in that defaults to the default.
                    districts.Add("Default");
                    List<string> dbSent = new List<string>();
                    foreach (string district in districts)
                    {
                        SqlCommand command = CreateCommand(CommandType.StoredProcedure,
                                                           sproc,district);
                        if (dbSent.Contains(command.Connection.ConnectionString))
                        {
                            // Don't send to the same db more than once.
                            continue;
                        }
                        dbSent.Add(command.Connection.ConnectionString);
                        command.Parameters.AddWithValue("@xmlUpdates", xmlString);
                        ExecuteNonQuery(command);
                        if (HttpContext.Current.IsDebuggingEnabled)
                        {
                            LoggerRepository.SaveLog(new Log
                                                                  {
                                                                      Category = LogCategory.Application,
                                                                      Level = LogLevel.Warning,
                                                                      Message = string.Format("/api/item/submit"),
                                                                      Details = xmlString
                                                                  });
                        }
                    }
                }
            }
        }

        public void UpdateItemTypes(List<ItemType> types)
        {
            List<ItemType> modified = new List<ItemType>();
            List<Dimension> dimsModified = new List<Dimension>();
            List<ConditionCodeSql> codesModified = new List<ConditionCodeSql>();
            foreach (ItemType type in types)
            {
                if (type.Modified)
                {
                    modified.Add(type);
                    foreach (Dimension dim in type.Dimensions)
                    {
                        dimsModified.Add(dim);
                        foreach (ConditionCode code in dim.ConditionCodes)
                        {
                            // Hack around the backwards way condition codes are modelled.
                            ConditionCodeSql code1 = new ConditionCodeSql(code,dim,type);
                            codesModified.Add(code1);
                        }
                    }
                }
            }
            UpdateSql(modified, "[dbo].[sp_UpdateItems]");
            UpdateSql(dimsModified, "[dbo].[sp_UpdateDimensions]");
            UpdateSql(codesModified, "[dbo].[sp_UpdateCodes]");
        }
    }
}

