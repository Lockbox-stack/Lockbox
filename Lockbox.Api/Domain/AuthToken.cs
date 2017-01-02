using System;

namespace Lockbox.Api.Domain
{
    public class AuthToken
    {
        public string Token { get; }
        public DateTime Expiry { get; }

        protected AuthToken(string token, DateTime expiry)
        {
            Token = token;
            Expiry = expiry;
        }

        public static AuthToken Create(string token, DateTime expiry)
            => new AuthToken(token, expiry);
    }
}