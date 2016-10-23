using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Client
{
    public class LockboxEntryClient : LockboxClientBase, ILockboxEntryClient
    {
        public LockboxEntryClient(string apiUrl, string apiKey)
            : base(apiUrl, "Bearer", apiKey)
        {
        }

        public async Task<IEnumerable<string>> GetEntryKeysAsync()
            => await GetAsync<IEnumerable<string>>("keys");

        public async Task<dynamic> GetEntryAsync(string key)
            => await GetEntryAsync<dynamic>(key);

        public async Task<T> GetEntryAsync<T>(string key)
            => await GetAsync<T>($"entries/{key}");

        public async Task<IDictionary<string, object>> GetEntryAsDictionaryAsync(string key)
            => await GetEntryAsync<IDictionary<string, object>>(key);

        public async Task CreateEntryAsync(string key, object value, TimeSpan? expiry = null)
        {
            var request = new
            {
                key,
                value,
                expiry
            };
            await PostAsync("entries", request);
        }

        public async Task DeleteEntryAsync(string key)
            => await DeleteAsync($"entries/{key}");
    }
}