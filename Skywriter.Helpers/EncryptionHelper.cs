using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Skywriter.Helpers
{
    public class EncryptionHelper
    {
        public static String Hash(String inputString, String salt)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider cryptographer = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] data = System.Text.Encoding.ASCII.GetBytes(inputString + salt);
            data = cryptographer.ComputeHash(data);

            return BitConverter.ToString(data);
        }

        public static string Encrypt(string toEncrypt, string securityKey, bool useHashing)
        {
            string retVal = string.Empty;

            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

                // If hashing use get hashcode regards to your key

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));

                    // Always release the resources and flush data
                    // of the Cryptographic service provide. Best Practice

                    hashmd5.Clear();
                }
                else
                {
                    keyArray = UTF8Encoding.UTF8.GetBytes(securityKey);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                // Set the secret key for the tripleDES algorithm

                tdes.Key = keyArray;

                // Mode of operation. there are other 4 modes
                // We choose ECB (Electronic code Book)

                tdes.Mode = CipherMode.ECB;

                // Padding mode (if any extra byte added)                
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateEncryptor();

                // Transform the specified region of bytes array to resultArray
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                // Release resources held by TripleDes Encryptor
                tdes.Clear();

                // Return the encrypted data into unreadable string format
                retVal = BitConverter.ToString(resultArray);
            }
            catch (Exception ex)
            {
                return "";
            }

            return retVal;
        }

        public static string Decrypt(string cipherString, string securityKey, bool useHashing)
        {
            string retVal = string.Empty;

            try
            {
                String[] arr = cipherString.Split('-');

                byte[] keyArray;
                byte[] toEncryptArray = new byte[arr.Length];
                for (int i = 0; i < arr.Length; i++) toEncryptArray[i] = Convert.ToByte(arr[i], 16);

                if (useHashing)
                {
                    // If hashing was used get the hash code with regards to your key
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(securityKey));

                    // Release any resource held by the MD5CryptoServiceProvider
                    hashmd5.Clear();
                }
                else
                {
                    // If hashing was not implemented get the byte code of the key
                    keyArray = UTF8Encoding.UTF8.GetBytes(securityKey);
                }

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                // Set the secret key for the tripleDES algorithm
                tdes.Key = keyArray;

                // Mode of operation. there are other 4 modes
                // We choose ECB(Electronic code Book)                
                tdes.Mode = CipherMode.ECB;

                // Padding mode(if any extra byte added)
                tdes.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                // Release resources held by TripleDes Encryptor
                tdes.Clear();

                // Return the Clear decrypted TEXT
                retVal = UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                return "";
            }

            return retVal;
        }
    }
}
