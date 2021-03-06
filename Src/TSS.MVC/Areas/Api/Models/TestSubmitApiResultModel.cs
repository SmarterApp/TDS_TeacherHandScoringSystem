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
using System.Linq;
using System.Web;

namespace TSS.MVC.Areas.Api.Models
{
    public class TestSubmitApiResultModel
    {
        public List<TestSubmitApiResultFileModel> Files = new List<TestSubmitApiResultFileModel>();
    }

    public class TestSubmitApiResultFileModel
    {
        public string FileName { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
