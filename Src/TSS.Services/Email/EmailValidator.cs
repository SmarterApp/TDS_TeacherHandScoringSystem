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
using System.Net.Mail;

namespace TSS.Services
{
    public static class EmailValidator
    {
        /// <summary>
        /// Indicates if the specified email address is valid.
        /// </summary>
        /// <param name="email">Address to validate</param>
        /// <returns></returns>
        public static bool IsValid(string email)
        {
            try
            {
                var address = new MailAddress(email);
            } catch(Exception)
            {
                return false;
            }
            return true;
        }
    }
}
