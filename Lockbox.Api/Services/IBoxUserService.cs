using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IBoxUserService
    {
        Task<BoxUser> GetAsync(string box, string username);
        Task<IEnumerable<string>> GetUsernamesAsync(string box);
        Task AddAsync(string box, string username, BoxRole? role = null);
        Task UpdateAsync(string box, string username, BoxRole? role = null, bool? isActive = null);
        Task ActivateAsync(string box, string username);
        Task LockAsync(string box, string username);
        Task DeleteAsync(string box, string username);
    }
}