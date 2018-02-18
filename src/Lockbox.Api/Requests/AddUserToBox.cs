using Lockbox.Api.Domain;

namespace Lockbox.Api.Requests
{
    public class AddUserToBox
    {
        public string Username { get; set; }
        public BoxRole? Role { get; set; }
    }
}