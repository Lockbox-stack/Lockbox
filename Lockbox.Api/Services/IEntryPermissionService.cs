using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryPermissionService
    {
        Task<object> GetValueAsync(string username, string key);
        Task<IEnumerable<string>> GetKeysAsync(string username);
        Task CreateAsync(string username, string key, object value);
        Task DeleteAsync(string username, string key);
    }
}