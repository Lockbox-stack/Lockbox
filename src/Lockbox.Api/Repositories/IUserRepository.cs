using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string username);
        Task<int> CountUsersWithRoleAsync(Role role);
        Task<IEnumerable<string>> GetUsernamesAsync();
        Task AddAsync(User user);
        Task DeleteAsync(string username);
        Task UpdateAsync(User user);
        Task<User> GetByApiKeyAsync(string apiKey);
        Task<bool> AnyAsync();
    }
}