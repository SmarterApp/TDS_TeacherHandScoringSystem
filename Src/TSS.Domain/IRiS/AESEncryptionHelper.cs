using System.IO;
using System.Security.Cryptography;

namespace TSS.Domain
{
    public class AESEncryptionHelper
    {
        public static AesCryptoServiceProvider CreateServiceProvider(byte[] key, byte[] iv)
        {
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.KeySize = key.Length * 8;
            aes.IV = iv;
            aes.Key = key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Create an AESCryptoProvider.
            using (SymmetricAlgorithm aes = CreateServiceProvider(key, iv))
            {
                // Create encryptor from the AESCryptoProvider.
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    // Create memory stream to store the encrypted data.
                    using (MemoryStream stream = new MemoryStream())
                    {
                        // Create a CryptoStream to encrypt the data.
                        using (CryptoStream cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                        {
                            // Encrypt the data.
                            cryptoStream.Write(data, 0, data.Length);
                        }

                        // return the encrypted data.
                        return stream.ToArray();
                    }
                }
            }
        }
    }
}
