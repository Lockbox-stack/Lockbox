using System;

namespace Lockbox.Api.Requests
{
    public class CreateApiKey
    {
        public TimeSpan? Expiry { get; set; }
    }
}