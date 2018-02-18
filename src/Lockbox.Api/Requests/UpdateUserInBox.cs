using Lockbox.Api.Domain;

namespace Lockbox.Api.Requests
{
    public class UpdateUserInBox
    {
        public string Username { get; set; }
        public bool? IsActive { get; set; }
        public BoxRole? Role { get; set; }
    }
}