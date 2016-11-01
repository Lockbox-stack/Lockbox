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
        private readonly IBoxRepository _boxRepository;

        public EntryPermissionService(IEntryService entryService, IUserRepository userRepository,
            IBoxRepository boxRepository)
        {
            _entryService = entryService;
            _userRepository = userRepository;
            _boxRepository = boxRepository;
        }

        public async Task<object> GetValueAsync(string username, string box, string key, string encryptionKey)
        {
            await ValidatePermission(username, box, Permission.ReadEntry);

            return await _entryService.GetValueAsync(box, key, encryptionKey);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string username, string box)
        {
            await ValidatePermission(username, box, Permission.ReadEntryKeys);

            return await _entryService.GetKeysAsync(box);
        }

        public async Task CreateAsync(string username, string box, string key, object value, string encryptionKey)
        {
            await ValidatePermission(username, box, Permission.CreateEntry);
            await _entryService.CreateAsync(box, key, value, username, encryptionKey);
        }

        public async Task DeleteAsync(string username, string box, string key)
        {
            await ValidatePermission(username, box, Permission.DeleteEntry);
            await _entryService.DeleteAsync(box, key);
        }

        private async Task ValidatePermission(string username, string box, Permission permission)
        {
            var entryBox = await _boxRepository.GetAsync(box);
            if (entryBox == null)
                throw new ArgumentException($"Box {box} has not been found.");
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found.");
            if (!user.IsActive)
                throw new AuthenticationException($"User {username} is not active.");
            if (user.Role == Role.Admin)
                return;

            var boxUser = entryBox.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found in box {box}.");
            if (boxUser.Permissions.Contains(permission))
                return;

            throw new AuthenticationException($"User {username} does not have permission {permission} in box {box}.");
        }
    }
}