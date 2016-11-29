using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lockbox.Client
{
    public class LockboxEntryClient : LockboxClientBase, ILockboxEntryClient
    {
        public LockboxEntryClient(string encryptionKey, string apiUrl, string apiKey)
            : base(apiUrl, apiKey)
        {
            HttpClient.DefaultRequestHeaders.Add("X-Encryption-Key", encryptionKey);
        }

        public async Task<IEnumerable<string>> GetEntryKeysAsync(string box)
            => await GetAsync<IEnumerable<string>>($"boxes/{box}/entries");

        public async Task<dynamic> GetEntryAsync(string box, string key)
            => await GetEntryAsync<dynamic>(box, key);

        public async Task<T> GetEntryAsync<T>(string box, string key)
            => await GetAsync<T>($"boxes/{box}/entries/{key}");

        public async Task CreateEntryAsync(string box, string key, object value)
        {
            var request = new
            {
                key,
                value
            };
            await PostAsync($"boxes/{box}/entries", request);
        }

        public async Task DeleteEntryAsync(string box, string key)
            => await DeleteAsync($"boxes/{box}/entries/{key}");
    }
}