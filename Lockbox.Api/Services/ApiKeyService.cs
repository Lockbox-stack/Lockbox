using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IUserRepository _userRepository;
        private readonly IEncrypter _encrypter;

        public ApiKeyService(IJwtTokenHandler jwtTokenHandler, IEncrypter encrypter, IUserRepository userRepository)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _userRepository = userRepository;
            _encrypter = encrypter;
        }

        public async Task<string> CreateAsync(string username, string password, TimeSpan? expiry = null)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found.");

            if (!user.ValidatePassword(password, _encrypter))
                throw new AuthenticationException($"Invalid credentials for user {username}.");

            var apiKey = _jwtTokenHandler.Create(username, expiry);
            user.AddApiKey(apiKey);
            await _userRepository.UpdateAsync(user);

            return apiKey;
        }

        public async Task<bool> IsValidAsync(string apiKey)
        {
            var user = await _userRepository.GetByApiKeyAsync(apiKey);
            if (user == null)
                return false;

            return user.IsActive && user.ApiKeys.Contains(apiKey);
        }

        public async Task RemoveAsync(string apiKey)
        {
            var user = await _userRepository.GetByApiKeyAsync(apiKey);
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User has not been found for given API key.");

            user.RemoveApiKey(apiKey);
            await _userRepository.UpdateAsync(user);
        }
    }
}