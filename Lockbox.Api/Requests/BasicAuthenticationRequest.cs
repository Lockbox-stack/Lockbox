namespace Lockbox.Api.Requests
{
    public abstract class BasicAuthenticationRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}