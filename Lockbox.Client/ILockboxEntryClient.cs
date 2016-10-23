using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Client
{
    public interface ILockboxEntryClient
    {
        Task<IEnumerable<string>> GetEntryKeysAsync();
        Task<dynamic> GetEntryAsync(string key);
        Task<T> GetEntryAsync<T>(string key);
        Task<IDictionary<string, object>> GetEntryAsDictionaryAsync(string key);
        Task CreateEntryAsync(string key, object value, TimeSpan? expiry = null);
        Task DeleteEntryAsync(string key);
    }
}