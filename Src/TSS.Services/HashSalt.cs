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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TSS.Services
{
    public class HashSalt
    {
        private const int HashSaltLength = 8;

        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="plainTextPassword">The plain text password.</param>
        /// <param name="saltBytes">The salt bytes.</param>
        /// <returns></returns>
        public static byte[] HashPassword(string plainTextPassword, byte[] saltBytes)
        {
            if (String.IsNullOrEmpty(plainTextPassword)) return null;

            byte[] plainTextBytes = Encoding.ASCII.GetBytes(plainTextPassword);

            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes =
              new byte[plainTextPassword.Length + saltBytes.Length];

            for (int i = 0; i < plainTextPassword.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainTextBytes[i];
            }
            for (int i = 0; i < saltBytes.Length; i++)
            {
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Generates the salt.
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateSalt()
        {
            var saltBytes = new byte[HashSaltLength];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);
            return saltBytes;
        }
    }
}

