using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CSVAssistent.Helper
{


    public static class FileHashHelper
    {
        public static string ComputeFileHash(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();

            byte[] hashBytes = sha256.ComputeHash(stream);
            return Convert.ToHexString(hashBytes); // .NET 5+; sonst BitConverter.ToString(...).Replace("-", "");
        }
    }

}
