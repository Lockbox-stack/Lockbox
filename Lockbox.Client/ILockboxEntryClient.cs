using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Client
{
    public interface ILockboxEntryClient
    {
        Task<IEnumerable<string>> GetEntryKeysAsync(string box);
        Task<dynamic> GetEntryAsync(string box, string key);
        Task<IDictionary<string, object>> GetEntryAsDictionaryAsync(string box, string key);
        Task<T> GetEntryAsync<T>(string box, string key);
        Task CreateEntryAsync(string box, string key, object value);
        Task DeleteEntryAsync(string box, string key);
    }
}