using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApp.Models
{
    public static class SecretHasher
    {
        private const int saltSize = 16;
        private const int keySize = 32;
        private const int iterations = 100000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
        private const char segmentDelimeter = ':';

        public static string Hash(string inputPassword)
        {
            byte[] salt  = RandomNumberGenerator.GetBytes(saltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, iterations, Algorithm, keySize);

            return string.Join(segmentDelimeter, Convert.ToHexString(hash), Convert.ToHexString(salt), iterations, Algorithm);
        }

        public static bool Verify(string inputPassword, string hashedPassword)
        {
            string[] segments = hashedPassword.Split(segmentDelimeter);

            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);

            int iterations = int.Parse(segments[2]);

            HashAlgorithmName algorithm = new HashAlgorithmName(segments[3]);

            byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, iterations, algorithm, keySize);

            return testHash.SequenceEqual(hash);
        }
        
        
    }
}
