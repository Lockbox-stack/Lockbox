using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class UserPermissionsService : IUserPermissionsService
    {
        private readonly IUserRepository _userRepository;

        public UserPermissionsService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(string username)
        {
            var user = await GetAsyncOrFail(username);

            return user.Permissions.ToList();
        }

        public async Task DeleteAllAsync(string username)
        {
            var user = await GetAsyncOrFail(username);
            user.DeleteAllPermissions();
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateAsync(string username, params Permission[] permissions)
        {
            var user = await GetAsyncOrFail(username);
            user.DeleteAllPermissions();
            var selectedPermissions = permissions?.ToList() ?? new List<Permission>();
            selectedPermissions.ForEach(user.AddPermission);
            await _userRepository.UpdateAsync(user);
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