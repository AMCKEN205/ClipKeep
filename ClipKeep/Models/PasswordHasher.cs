using System;
using System.Security.Cryptography;
using System.Text;

namespace ClipKeep.Models
{
    /// <summary>
    /// Used to provide password hash string retrieval
    /// </summary>
    public static class PasswordHasher
    {

        /// <summary>
        /// Generate a password hash for password storage or password validation
        /// </summary>
        /// <returns> The password has for password storage/validation in/from the database </returns>
        public static string GetPassHashString(string password, string hashSalt)
        {
            var passWithSalt = password + hashSalt;
            var passData = Encoding.UTF8.GetBytes(passWithSalt);
            using (var hashAlg = new SHA256Managed())
            {
                var passHash = hashAlg.ComputeHash(passData);
                var hashString = BitConverter.ToString(passHash).Replace("-", string.Empty);
                return hashString;
            }
        }
    }
}