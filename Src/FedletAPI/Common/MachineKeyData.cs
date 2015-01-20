using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace SAML.Handler
{
    /// <summary>
    /// Helper for encrypting or decrypting data using the machine key.
    /// </summary>
    /// <remarks>
    /// Review this pattern:
    /// https://github.com/SignalR/SignalR/blob/bc9412bcab0f5ef097c7dc919e3ea1b37fc8718c/src/Microsoft.AspNet.SignalR.Core/Infrastructure/DefaultProtectedData.cs
    /// </remarks>
    public class MachineKeyData
    {
        /// <summary>
        /// Encrypt bytes into hex string.
        /// </summary>
        /// <returns>
        /// The bytes encrypted as a hex string.
        /// </returns>
        private static string Encode(byte[] bytes)
        {
            return MachineKey.Encode(bytes, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Encrypt plain text string into base64.
        /// </summary>
        public static string Protect(string plainText, bool urlSafe = true)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            string encryptedText = Encode(bytes);
            return HexToBase64(encryptedText, urlSafe);
        }

        /// <summary>
        /// Descrypt hex string into bytes.
        /// </summary>
        /// <returns>
        /// The decrypted text in a byte array.
        /// </returns>
        private static byte[] Decode(string hex)
        {
            return MachineKey.Decode(hex, MachineKeyProtection.Encryption);
        }

        /// <summary>
        /// Decrypt base64 into plain text string. 
        /// </summary>
        public static string Unprotect(string base64, bool urlSafe = true)
        {
            string hex = Base64ToHex(base64, urlSafe);
            byte[] decryptedBytes = Decode(hex);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

        // String transformation helpers

        private static char HexDigit(int value)
        {
            return (char)(value > 9 ? value + '7' : value + '0');
        }

        private static int HexValue(char digit)
        {
            return digit > '9' ? digit - '7' : digit - '0';
        }

        private static string Base64ToHex(string base64, bool urlSafe)
        {
            StringBuilder builder = new StringBuilder(base64.Length * 4);
            byte[] bytes;

            if (urlSafe)
            {
                bytes = HttpServerUtility.UrlTokenDecode(base64);
            }
            else
            {
                bytes = Convert.FromBase64String(base64);
            }

            if (bytes == null)
            {
                throw new ArgumentException("Cannot convert base64 to byte array.");
            }

            foreach (byte b in bytes)
            {
                builder.Append(HexDigit(b >> 4));
                builder.Append(HexDigit(b & 0x0F));
            }
            string result = builder.ToString();
            return result;
        }

        private static byte[] HexToBytes(string hex)
        {
            int size = hex.Length / 2;
            byte[] bytes = new byte[size];
            for (int idx = 0; idx < size; idx++)
            {
                bytes[idx] = (byte)((HexValue(hex[idx * 2]) << 4) + HexValue(hex[idx * 2 + 1]));
            }
            return bytes;
        }

        private static string HexToBase64(string hex, bool urlSafe)
        {
            byte[] bytes = HexToBytes(hex);

            if (urlSafe)
            {
                return HttpServerUtility.UrlTokenEncode(bytes);
            }
            else
            {
                return Convert.ToBase64String(bytes);
            }
        }


        //private static byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };

        ///// <summary>
        ///// Function to encrypt the querystring
        ///// </summary>
        ///// <param name="stringToEncrypt"></param>
        ///// <param name="sEncryptionKey"></param>
        ///// <returns></returns>
        //public static string Protect(string pStringToEncrypt, bool urlSafe = true)
        //{
        //    try
        //    {
        //        string pEncryptionKey = "ASCMDJDL!%$S";
        //        byte[] key = System.Text.Encoding.UTF8.GetBytes(pEncryptionKey.PadRight(8, '0').Substring(0, 8));
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //        byte[] inputByteArray = Encoding.UTF8.GetBytes(pStringToEncrypt);
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        return Convert.ToBase64String(ms.ToArray());
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        ///// <summary>
        ///// Decrypts string using encryptionkey as key and returns original text
        ///// </summary>
        ///// <param name="stringToDecrypt">string to decrypt</param>
        ///// <param name="sEncryptionKey">key for decryption</param>
        ///// <returns>original string text</returns>
        //public static string Unprotect(string pStringToDecrypt, bool urlSafe = true)
        //{
        //    byte[] inputByteArray = new byte[pStringToDecrypt.Length];
        //    try
        //    {
        //        string pEncryptionKey = "ASCMDJDL!%$S";
        //        byte[] key = Encoding.UTF8.GetBytes(pEncryptionKey.PadRight(8, '0').Substring(0, 8));
        //        DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        //        inputByteArray = Convert.FromBase64String(pStringToDecrypt.Replace(" ", "+"));
        //        MemoryStream ms = new MemoryStream();
        //        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
        //        cs.Write(inputByteArray, 0, inputByteArray.Length);
        //        cs.FlushFinalBlock();
        //        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //        return encoding.GetString(ms.ToArray());
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

    }
}
