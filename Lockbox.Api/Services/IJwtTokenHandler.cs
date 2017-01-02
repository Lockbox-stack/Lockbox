using System;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IJwtTokenHandler
    {
        string Create(string username, TimeSpan? expiry = default(TimeSpan?));
        JwtTokenWithApiKey GetFromAuthorizationHeader(string authorizationHeader);
        bool IsValid(JwtToken token);
    }
}