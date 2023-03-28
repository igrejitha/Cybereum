using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Cybereum.Models
{
    public class EncryptDecrypt
    {
        //public string Encrypt(string password)
        //{
        //    // byte array representation of that string
        //    byte[] encodedPassword = new UTF8Encoding().GetBytes(password);

        //    // need MD5 to calculate the hash
        //    byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

        //    // string representation (similar to UNIX format)
        //    string encoded = BitConverter.ToString(hash)
        //       .Replace("-", string.Empty)
        //       // make lowercase
        //       .ToLower();
        //    return encoded;
        //}

        //public string Decrypt(string password)
        //{
        //    string source = "01cfcd4f6b8770febfb40cb906715822";// "Hello World!";
        //    MD5 md5 = System.Security.Cryptography.MD5.Create();
        //    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
        //    byte[] hash = md5.ComputeHash(inputBytes);

        //    // step 2, convert byte array to hex string
        //    StringBuilder sb = new StringBuilder();
        //    for (int i = 0; i < hash.Length; i++)
        //    {
        //        sb.Append(hash[i].ToString("X2"));
        //    }
        //    return sb.ToString();
        //}

        //static string GetMd5Hash(MD5 md5Hash, string input)
        //{

        //    // Convert the input string to a byte array and compute the hash.
        //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        //    // Create a new Stringbuilder to collect the bytes
        //    // and create a string.
        //    StringBuilder sBuilder = new StringBuilder();

        //    // Loop through each byte of the hashed data 
        //    // and format each one as a hexadecimal string.
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        sBuilder.Append(data[i].ToString("x2"));
        //    }

        //    // Return the hexadecimal string.
        //    return sBuilder.ToString();
        //}

        public string Encrypt(string clearText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}