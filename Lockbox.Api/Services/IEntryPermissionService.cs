using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryPermissionService
    {
        Task<object> GetValueAsync(string username, string key, string encryptionKey);
        Task<IEnumerable<string>> GetKeysAsync(string username);
        Task CreateAsync(string username, string key, object value, string encryptionKey);
        Task DeleteAsync(string username, string key);
    }
}