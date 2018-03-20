using System;
using System.Text;
using Jose;
using Lockbox.Api.Domain;
using Lockbox.Api.Extensions;
using Serilog;

namespace Lockbox.Api.Services
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private static readonly ILogger Logger = Log.Logger;
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

        public JwtTokenWithApiKey GetFromAuthorizationHeader(string authorizationHeader)
        {
            var authorizationTypeAndToken = authorizationHeader.ParseAuthorzationHeader();

            return authorizationTypeAndToken.Key != "bearer"
                ? null
                : Decode(authorizationTypeAndToken.Value);
        }

        private JwtTokenWithApiKey Decode(string token)
        {
            try
            {
                var jwtToken = JWT.Decode<JwtToken>(token, _jwtSecretKey, JwsAlgorithm.HS512);

                return new JwtTokenWithApiKey
                {
                    Sub = jwtToken.Sub,
                    Exp = jwtToken.Exp,
                    ApiKey = token
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "JWT Token generation error.");

                throw;
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