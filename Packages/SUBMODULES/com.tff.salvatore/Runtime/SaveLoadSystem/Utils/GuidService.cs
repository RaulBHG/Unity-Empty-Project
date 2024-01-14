using System;
using System.Security.Cryptography;
using System.Text;

namespace TFF.Salvatore.SaveLoadSystem.Utils
{
    public class GuidService
    {
        /**
         * Method to generate secured Guid based on string
         */
        public static Guid GenerateGuidFromString(string inputString)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }

    }
}