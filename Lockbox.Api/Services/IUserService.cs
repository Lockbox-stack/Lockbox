using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IUserService
    {
        Task<User> GetAsync(string username);
        Task<IEnumerable<string>> GetUsernamesAsync();
        Task CreateAsync(string username, string password, Role? role = null);
        Task ActivateAsync(string username);
        Task LockAsync(string username);
        Task DeleteAsync(string username);
    }
}