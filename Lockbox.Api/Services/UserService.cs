using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApiKeyService _apiKeyService;
        private readonly IEncrypter _encrypter;

        public UserService(IUserRepository userRepository, IApiKeyService apiKeyService, IEncrypter encrypter)
        {
            _userRepository = userRepository;
            _apiKeyService = apiKeyService;
            _encrypter = encrypter;
        }

        public async Task<User> GetAsync(string username)
            => await _userRepository.GetAsync(username);

        public async Task<IEnumerable<string>> GetUsernamesAsync()
            => await _userRepository.GetUsernamesAsync();

        public async Task CreateAsync(string username, string password, Role? role = null)
        {
            var user = await _userRepository.GetAsync(username);
            if (user != null)
                throw new ArgumentException($"User {username} already exists.", nameof(username));

            user = new User(username, role.GetValueOrDefault(Role.User));
            user.SetPassword(password, _encrypter);
            user.Activate();
            await _userRepository.AddAsync(user);
            await _apiKeyService.CreateAsync(username);
        }

        public async Task ActivateAsync(string username)
        {
            var user = await GetAsyncOrFail(username);
            user.Activate();
            await _userRepository.UpdateAsync(user);
        }

        public async Task LockAsync(string username)
        {
            var user = await GetAsyncOrFail(username);
            user.Lock();
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteAsync(string username)
        {
            var user = await GetAsyncOrFail(username);
            if (user.Role == Role.Admin)
            {
                var adminsCount = await _userRepository.CountUsersWithRoleAsync(Role.Admin);
                if (adminsCount == 1)
                    throw new InvalidOperationException("Can not remove the only one admin account.");
            }

            await _userRepository.DeleteAsync(username);
        }

        private async Task<User> GetAsyncOrFail(string username)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found.");

            return user;
        }
    }
}