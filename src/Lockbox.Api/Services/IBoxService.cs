using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IBoxService
    {
        Task<Box> GetAsync(string name);
        Task<IEnumerable<string>> GetNamesAsync();
        Task<IEnumerable<string>> GetNamesForUserAsync(string username);
        Task<bool> HasAccessAsync(string box, string username);
        Task<bool> HasManagementAccess(string box, string username);
        Task CreateAsync(string name, string owner);
        Task DeleteAsync(string name);
    }
}