using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string username);
        Task AddAsync(User user);
        Task DeleteAsync(string username);
        Task UpdateAsync(User user);
        Task<User> GetByApiKeyAsync(string apiKey);
        Task<bool> AnyAsync();
    }
}