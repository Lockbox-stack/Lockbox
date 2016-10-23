using System;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IJwtTokenHandler
    {
        string Create(string username, TimeSpan? expiry = default(TimeSpan?));
        string GetFromAuthorizationHeader(string authorizationHeader);
        JwtToken Decode(string token);
        bool IsValid(JwtToken token);
    }
}