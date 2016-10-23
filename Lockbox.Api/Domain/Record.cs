using System;
using Lockbox.Api.Extensions;

namespace Lockbox.Api.Domain
{
    public class Record
    {
        public string Key { get; protected set; }
        public string Value { get; protected set; }
        public string Salt { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        protected Record()
        {
        }

        public Record(string key, string value, string salt)
        {
            if (key.Empty())
                throw new ArgumentException("Key can not be empty.", nameof(key));
            if (value.Empty())
                throw new ArgumentException("Value can not be empty.", nameof(value));
            if (salt.Empty())
                throw new ArgumentException("Salt can not be empty.", nameof(salt));

            Key = key;
            Value = value;
            Salt = salt;
            CreatedAt = DateTime.UtcNow;
        }
    }
}