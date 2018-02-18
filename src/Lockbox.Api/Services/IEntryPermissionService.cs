using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryPermissionService
    {
        Task<object> GetValueAsync(string username, string box,  string key, string encryptionKey);
        Task<IEnumerable<string>> GetKeysAsync(string username, string box);
        Task CreateAsync(string username, string box, string key, object value, string encryptionKey);
        Task DeleteAsync(string username, string box, string key);
    }
}