using System;
using System.Security.Cryptography;
using System.Text;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public class Encrypter : IEncrypter
    {
        public string GetSalt(string value)
        {
            const int minSaltSize = 20;
            const int maxSaltSize = 40;

            var random = new Random();
            var saltSize = random.Next(minSaltSize, maxSaltSize);

            var saltBytes = new byte[saltSize];

            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        public string GetHash(string value, string salt)
        {
            using (var sha512 = SHA512.Create())
            {
                var bytes = Encoding.Unicode.GetBytes(value + salt);
                var hash = sha512.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }
    }
}