using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lockbox.Client
{
    public abstract class LockboxClientBase
    {
        protected readonly HttpClient HttpClient;

        protected LockboxClientBase(string apiUrl, string authenticationType, string token)
        {
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(apiUrl)
            };
            HttpClient.DefaultRequestHeaders.Remove("Accept");
            HttpClient.DefaultRequestHeaders.Remove("Authorization");
            HttpClient.DefaultRequestHeaders.Add("Authorization", $"{authenticationType} {token}");
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        protected async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await GetAsync(endpoint);

            return await DeserializeAsync<T>(response);
        }

        protected async Task<HttpResponseMessage> GetAsync(string endpoint)
            => await HttpClient.GetAsync(endpoint);

        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var response = await PostAsync(endpoint, data);

            return await DeserializeAsync<T>(response);
        }

        protected async Task<HttpResponseMessage> PostAsync(string endpoint, object data)
        {
            var payload = JsonConvert.SerializeObject(data);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            return await HttpClient.PostAsync(endpoint, content);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string endpoint)
            => await HttpClient.DeleteAsync(endpoint);

        private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                return default(T);

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}