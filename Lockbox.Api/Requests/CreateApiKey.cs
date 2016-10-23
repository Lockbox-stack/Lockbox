using System;

namespace Lockbox.Api.Requests
{
    public class CreateApiKey : BasicAuthenticationRequest
    {
        public TimeSpan? Expiry { get; set; }
    }
}