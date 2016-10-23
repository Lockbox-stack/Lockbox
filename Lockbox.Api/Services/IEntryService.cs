using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IEntryService
    {
        Task<object> GetValueAsync(string key);
        Task<IEnumerable<string>> GetKeysAsync();
        Task CreateAsync(string key, object value);
        Task DeleteAsync(string key);
    }
}