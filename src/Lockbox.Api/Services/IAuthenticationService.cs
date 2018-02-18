using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IAuthenticationService
    {
         Task<AuthToken> AuthenticateAsync(string username, string password);
    }
}