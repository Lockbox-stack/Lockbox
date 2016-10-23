namespace Lockbox.Api.Domain
{
    public class JwtToken
    {
        public string Sub { get; set; }
        public long Exp { get; set; }
    }
}