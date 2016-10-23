using System;
using System.Text;
using Jose;
using Lockbox.Api.Domain;
using Lockbox.Api.Extensions;
using NLog;

namespace Lockbox.Api.Services
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly byte[] _jwtSecretKey;

        public JwtTokenHandler(LockboxSettings lockboxSettings)
        {
            _jwtSecretKey = Encoding.Unicode.GetBytes(lockboxSettings.SecretKey);
        }

        public string Create(string username, TimeSpan? expiry = null)
        {
            var expiryTicks = expiry?.Ticks ?? DateTime.MinValue.AddYears(100).Ticks;
            var customPayload = new JwtToken
            {
                Sub = username,
                Exp = DateTime.UtcNow.AddTicks(expiryTicks).Ticks
            };

            return JWT.Encode(customPayload, _jwtSecretKey, JwsAlgorithm.HS512);
        }

        public string GetFromAuthorizationHeader(string authorizationHeader)
        {
            var authorizationTypeAndToken = authorizationHeader.ParseAuthorzationHeader();

            return authorizationTypeAndToken.Key != "bearer"
                ? null
                : authorizationTypeAndToken.Value;
        }

        public JwtToken Decode(string token)
        {
            try
            {
                return JWT.Decode<JwtToken>(token, _jwtSecretKey, JwsAlgorithm.HS512);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "JWT Token generation error.");

                return null;
            }
        }

        public bool IsValid(JwtToken token)
        {
            if (token == null)
                return false;

            var expiry = DateTime.FromBinary(token.Exp);

            return expiry > DateTime.UtcNow;
        }
    }
}