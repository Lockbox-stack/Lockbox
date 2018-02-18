using System.Threading.Tasks;

namespace Lockbox.Client
{
    public class LockboxAccountClient : LockboxClientBase, ILockboxAccountClient
    {
        public LockboxAccountClient(string apiUrl, string apiKey)
            : base(apiUrl, apiKey)
        {
        }

        public async Task<string> CreateApiKeyAsync()
        {
            var response = await PostAsync<dynamic>("api-keys", new {});

            return response.apiKey as string;
        }

        public async Task DeleteApiKeyAsync(string key)
            => await DeleteAsync($"api-keys/{key}");
    }
}