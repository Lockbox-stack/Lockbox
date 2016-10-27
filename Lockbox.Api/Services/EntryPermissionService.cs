using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class EntryPermissionService : IEntryPermissionService
    {
        private readonly IEntryService _entryService;
        private readonly IUserRepository _userRepository;

        public EntryPermissionService(IEntryService entryService, IUserRepository userRepository)
        {
            _entryService = entryService;
            _userRepository = userRepository;
        }

        public async Task<object> GetValueAsync(string username, string key, string encryptionKey)
        {
            await ValidatePermission(username, Permission.ReadEntry);

            return await _entryService.GetValueAsync(key, encryptionKey);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string username)
        {
            await ValidatePermission(username, Permission.ReadEntryKeys);

            return await _entryService.GetKeysAsync();
        }

        public async Task CreateAsync(string username, string key, object value, string encryptionKey)
        {
            await ValidatePermission(username, Permission.CreateEntry);
            await _entryService.CreateAsync(key, value, username, encryptionKey);
        }

        public async Task DeleteAsync(string username, string key)
        {
            await ValidatePermission(username, Permission.DeleteEntry);
            await _entryService.DeleteAsync(key);
        }

        private async Task ValidatePermission(string username, Permission permission)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found.");
            if (!user.IsActive)
                throw new AuthenticationException($"User {username} is not active.");
            if(user.Role == Role.Admin)
                return;
            if(user.Permissions.Contains(permission))
                return;

            throw new AuthenticationException($"User {username} does not have permission {permission}.");
        }
    }
}