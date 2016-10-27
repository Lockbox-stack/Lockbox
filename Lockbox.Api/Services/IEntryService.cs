using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryService
    {
        Task<object> GetValueAsync(string key, string encryptionKey);
        Task<IEnumerable<string>> GetKeysAsync();
        Task CreateAsync(string key, object value, string author, string encryptionKey);
        Task DeleteAsync(string key);
    }
}