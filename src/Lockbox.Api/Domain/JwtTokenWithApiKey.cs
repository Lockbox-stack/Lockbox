namespace Lockbox.Api.Domain
{
    public class JwtTokenWithApiKey : JwtToken
    {
        public string ApiKey { get; set; }
    }
}