using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lockbox.Client
{
    public interface ILockboxClient
    {
        Task<dynamic> GetRecordAsync(string key);
        Task<T> GetRecordAsync<T>(string key);
        Task<IDictionary<string, object>> GetComplexRecordAsync(string key);
    }

    public class LockboxClient : ILockboxClient
    {
        private readonly HttpClient _httpClient;

        public LockboxClient(string apiUrl, string apiKey)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
            _httpClient.DefaultRequestHeaders.Remove("Accept");
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<dynamic> GetRecordAsync(string key)
            => await GetRecordAsync<dynamic>(key);

        public async Task<T> GetRecordAsync<T>(string key)
        {
            var response = await GetResponseAsync($"records/{key}");
            if (!response.IsSuccessStatusCode)
                return default(T);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<IDictionary<string, object>> GetComplexRecordAsync(string key)
            => await GetRecordAsync<IDictionary<string, object>>(key);

        private async Task<HttpResponseMessage> GetResponseAsync(string endpoint)
            => await _httpClient.GetAsync(endpoint);
    }
}