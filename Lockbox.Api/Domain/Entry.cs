using System;
using Lockbox.Api.Extensions;

namespace Lockbox.Api.Domain
{
    public class Entry
    {
        public string Key { get; protected set; }
        public string Value { get; protected set; }
        public string Salt { get; protected set; }
        public string Author { get; protected set; }
        public DateTime Expiry { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected Entry()
        {
        }

        public Entry(string key, string value, string salt, string author, DateTime? expiry = null)
        {
            if (key.Empty())
                throw new ArgumentException("Key can not be empty.", nameof(key));
            if (value.Empty())
                throw new ArgumentException("Value can not be empty.", nameof(value));
            if (salt.Empty())
                throw new ArgumentException("Salt can not be empty.", nameof(salt));
            if (author.Empty())
                throw new ArgumentException("Author can not be empty.", nameof(author));

            Key = key.ToLowerInvariant();
            Value = value;
            Salt = salt;
            Author = author;
            Expiry = expiry.GetValueOrDefault(DateTime.UtcNow.AddYears(100));
            CreatedAt = DateTime.UtcNow;
        }
    }
}