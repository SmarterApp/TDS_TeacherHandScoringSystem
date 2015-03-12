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
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace TSS.MVC.Helpers
{
    public class SchemaHelper 
    {
        /// <summary>
        /// validate xmldocument against the specified xsd schema.  
        /// </summary>
        /// <param name="xsdPath"></param>
        /// <param name="doc"></param>
        /// <returns>empty string if file passes validation otherwise returns error message</returns>
        public static string Validate(string xsdPath, XmlDocument doc)
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            using (XmlReader r = XmlReader.Create(xsdPath, null))
            {
                schemaSet.Add(null, r);
            }

            StringBuilder error = new StringBuilder();
            try
            {
                doc.Schemas.Add(schemaSet);
                doc.Validate(
                    delegate(object sender, ValidationEventArgs args)
                    {
                        error.AppendLine(String.Format("{0}: {1}", args.Severity, args.Message));
                    }
                );
            }
            catch (Exception ex)
            {
                error.Append(ex.Message);
                error.AppendLine();
            }

            return error.ToString();
        }
    }
}

