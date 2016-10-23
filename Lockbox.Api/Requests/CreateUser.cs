using Lockbox.Api.Domain;

namespace Lockbox.Api.Requests
{
    public class CreateUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role? Role { get; set; }
    }
}