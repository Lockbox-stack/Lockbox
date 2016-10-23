using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Repositories
{
    public interface IEntryRepository
    {
        Task<Entry> GetAsync(string key);
        Task<IEnumerable<string>> GetKeysAsync();
        Task AddAsync(Entry entry);
        Task DeleteAsync(string key);
    }
}