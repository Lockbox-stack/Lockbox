using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using NLog;

namespace Lockbox.Api.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IUserRepository _userRepository;

        public ApiKeyService(IJwtTokenHandler jwtTokenHandler, IUserRepository userRepository)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _userRepository = userRepository;
        }

        public async Task<User> GetUserAsync(string apiKey)
            => await _userRepository.GetByApiKeyAsync(apiKey);

        public async Task<string> CreateAsync(string username, TimeSpan? expiry = null)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User '{username}' has not been found.");
            if (!user.IsActive)
                throw new AuthenticationException($"User '{username}' is not active.");

            var apiKey = _jwtTokenHandler.Create(username, expiry);
            user.AddApiKey(apiKey);
            await _userRepository.UpdateAsync(user);
            Logger.Info($"API key was created by user '{username}'.");

            return apiKey;
        }

        public bool IsValid(User user, string apiKey)
        {
            if (user == null)
                return false;

            return user.IsActive && user.ApiKeys.Contains(apiKey);
        }

        public async Task DeleteAsync(string apiKey)
        {
            var user = await _userRepository.GetByApiKeyAsync(apiKey);
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User has not been found for given API key.");
            if (!user.IsActive)
                throw new AuthenticationException($"User '{user.Username}' is not active.");
            if (user.ApiKeys.Count() == 1)
                throw new InvalidOperationException($"User '{user.Username}' must have at least one API key.");

            user.DeleteApiKey(apiKey);
            await _userRepository.UpdateAsync(user);
            Logger.Info($"API key was deleted by user '{user.Username}'.");
        }
    }
}