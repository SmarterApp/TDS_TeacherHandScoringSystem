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
using System.Configuration;
using System.Linq;
using System.Web;

namespace TSS.MVC.Models
{
    public class ItemDetailModel
    {
        public Guid AssignmentId;



        public string IRiSRootURL
        {
            get
            {
                return ConfigurationManager.AppSettings["IRIS_ROOT_URL"].ToString();
            }
        }

        public string IriSBlackboxRootURL
        {
            get
            {
                return ConfigurationManager.AppSettings["IRISBlackbox_ROOT_URL"].ToString();
            }
        }

    }
}
