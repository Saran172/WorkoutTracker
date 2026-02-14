using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WorkoutTracker.API.EncryptionServices
{
    public class EncryptionService : IEnryptService
    {
        public string ToCSV(DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : "\t");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString().Replace("\n", "/n"));
                    result.Append(i == table.Columns.Count - 1 ? "\n" : "\t");
                }
            }

            return result.ToString();
        }

        public string Encryption(string EncText)
        {
            using var aesAlg = Aes.Create();

            string enkey = "42358357407472253245745740747545";
            aesAlg.Key = Encoding.UTF8.GetBytes(enkey);
            var plain = Encoding.UTF8.GetBytes(EncText);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            byte[] encrypted;
            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            {
                byte[] plainTextBytes = plain;
                encrypted = encryptor.TransformFinalBlock(plainTextBytes, 0, plainTextBytes.Length);
            }            
            return Convert.ToBase64String(encrypted);
        }

        public string Decryption(string PlainText)
        {
            using var aesAlg = Aes.Create();
          
            string enkey = "42358357407472253245745740747545";
            aesAlg.Key = Encoding.UTF8.GetBytes(enkey);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;
            byte[] encryptedBytes = Convert.FromBase64String(PlainText);
            byte[] iv = new byte[aesAlg.IV.Length];
            Array.Copy(encryptedBytes, 0, iv, 0, iv.Length);
            byte[] encryptedMessage = new byte[encryptedBytes.Length];
            Array.Copy(encryptedBytes, 0, encryptedMessage, 0, encryptedMessage.Length);

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, iv))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedMessage, 0, encryptedMessage.Length);
                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);


                try
                {
                    JObject jsonObject = JObject.Parse(decryptedText);
                    return jsonObject.ToString();
                }
                catch (JsonReaderException)
                {

                    return decryptedText;
                }
            }
        }
    }
}
