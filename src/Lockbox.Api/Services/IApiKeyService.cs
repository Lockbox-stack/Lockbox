using System;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IApiKeyService
    {
        Task<User> GetUserAsync(string apiKey);
        Task<string> CreateAsync(string username, TimeSpan? expiry = null);
        bool IsValid(User user, string apiKey);
        Task DeleteAsync(string apiKey);
    }
}