using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace CSVAssistent.Helper
{
    public static class FileHashHelper
    {
        /// <summary>
        /// Computes the SHA256 hash of a file at the given filepath.
        /// </summary>
        /// 
        /// <param name="filePath"></param>
        /// <returns>string</returns>
        public static string? ComputeFileHash(string? filePath)
        {
            if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return null;

            using var stream = File.OpenRead(filePath);
            using var sha256 = SHA256.Create();

            byte[] hashBytes = sha256.ComputeHash(stream);
            return Convert.ToHexString(hashBytes);
        }

        /// <summary>
        /// Compares the hash of the file at the given filepath with the provided hash string.
        /// </summary>
        /// 
        /// <param name="filePath"></param>
        /// <param name="hash"></param>
        /// <returns>bool</returns>
        public static bool Compare(string? filepath, string? hash)
        {
            if (String.IsNullOrEmpty(filepath) || String.IsNullOrEmpty(hash))
                return false;
            string? fileHash = ComputeFileHash(filepath);
            if (fileHash == null) return false;
            return string.Equals(fileHash, hash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
