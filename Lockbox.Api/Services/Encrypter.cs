using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public class Encrypter : IEncrypter
    {
        private static readonly int DeriveBytesIterationsCount = 10000;
        private static readonly int MinSaltSize = 20;
        private static readonly int MaxSaltSize = 40;
        private static readonly int MinSecureKeySize = 40;
        private static readonly int MaxSecureKeySize = 60;
        private static readonly Random Random = new Random();

        public string GetRandomSecureKey()
        {
            var size = Random.Next(MinSecureKeySize, MaxSecureKeySize);
            var bytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);

                return Convert.ToBase64String(bytes);
            }
        }

        public string GetSalt(string value)
        {
            var random = new Random();
            var saltSize = random.Next(MinSaltSize, MaxSaltSize);
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

        public string Encrypt(string value, string passwordKey, string salt)
        {
            var algorithm = TripleDES.Create();
            var rgb = new Rfc2898DeriveBytes(passwordKey, GetBytes(salt), DeriveBytesIterationsCount);
            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIv = rgb.GetBytes(algorithm.BlockSize >> 3);
            var transform = algorithm.CreateEncryptor(rgbKey, rgbIv);

            var buffer = new MemoryStream();
            using (var writer = new StreamWriter(new CryptoStream(buffer,
                transform, CryptoStreamMode.Write), Encoding.Unicode))
            {
                writer.Write(value);
            }

            return Convert.ToBase64String(buffer.ToArray());
        }

        public string Decrypt(string text, string passwordKey, string salt)
        {
            var algorithm = TripleDES.Create();
            var rgb = new Rfc2898DeriveBytes(passwordKey, GetBytes(salt), DeriveBytesIterationsCount);
            var rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
            var rgbIv = rgb.GetBytes(algorithm.BlockSize >> 3);
            var transform = algorithm.CreateDecryptor(rgbKey, rgbIv);

            var buffer = new MemoryStream(Convert.FromBase64String(text));
            using (var reader = new StreamReader(new CryptoStream(buffer,
                transform, CryptoStreamMode.Read), Encoding.Unicode))
            {
                return reader.ReadToEnd();
            }
        }

        private static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length*sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }
    }
}