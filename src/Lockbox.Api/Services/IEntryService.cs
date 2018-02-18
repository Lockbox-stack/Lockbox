using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryService
    {
        Task<object> GetValueAsync(string box, string key, string encryptionKey);
        Task<IEnumerable<string>> GetKeysAsync(string box);
        Task CreateAsync(string box, string key, object value, string author, string encryptionKey);
        Task DeleteAsync(string box, string key);
    }
}