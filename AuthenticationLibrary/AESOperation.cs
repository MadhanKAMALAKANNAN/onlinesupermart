//Created by:   Madhan KAMALAKANNAN, Madhan.KAMALAKANNAN @outlook.com, Nov/2022
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationLibrary
{
    public class AESOperation
    {

        private static  readonly Random _random = new Random();

        public AESOperation()
        {
        }
        public static string EncryptString(TokenParameters tokenParams)
        {
            try
            {
                if (tokenParams != null)
                {
                    byte[] iv = new byte[16];
                    byte[] array;
                    StringBuilder plainText = new StringBuilder();
                    plainText.Append(tokenParams.Issuer);
                    plainText.Append(tokenParams.Token);
                    plainText.Append(tokenParams.AuthenticationType);
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = Encoding.UTF8.GetBytes(tokenParams.IssuerSigningKey);
                        aes.IV = iv;

                        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                                {
                                    streamWriter.Write(plainText);
                                }

                                array = memoryStream.ToArray();
                            }
                        }
                    }

                    return Convert.ToBase64String(array);
                }
            }
            catch (Exception ex) { }
            return "error";
        }

        public static string DecryptString(TokenParameters tokenParams)
        {
            try
            {
                string key = tokenParams.IssuerSigningKey;
                string cipherText = tokenParams.CipherToken;
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return "error";
        }

        public static string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):   
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length = 26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
    }
 }


