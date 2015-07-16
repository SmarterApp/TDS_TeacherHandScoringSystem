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
using System.Text;
using System.Threading.Tasks;

namespace TSS.Services
{
    public class ValidationDictionary : IValidationDictionary
    {
        #region Fields

        #endregion

        #region Constructor

        public ValidationDictionary()
        {
        }

        #endregion
        
        #region Overrides of IValidationDictionary

        public override void AddError(string key, string errorMessage)
        {
            ErrorDictionary.Add(new ValidationError(key, errorMessage));
        }

        public override bool IsValid
        {
            get { return ErrorDictionary.Count == 0; }
        }

        public override IList<string> Errors
        {
            get { return ErrorDictionary.Select(e => e.Message).ToList(); }
        }

        public override void Remove(string key)
        {
            var item = ErrorDictionary.FirstOrDefault(e => e.PropertyName == key);
            if (item != null)
            {
                ErrorDictionary.Remove(item);
            }
        }

        public override void Merge(IValidationDictionary vd)
        {
            foreach (var err in vd.ErrorDictionary)
            {
                AddError(err.PropertyName, err.Message);
            }
        }

        #endregion
    }
}
