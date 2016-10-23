using System;
using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IApiKeyService
    {
        Task<string> CreateAsync(string username, string password, TimeSpan? expiry = null);
        Task<bool> IsValidAsync(string apiKey);
        Task DeleteAsync(string apiKey);
    }
}