using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class BoxUserService : IBoxUserService
    {
        private readonly IBoxRepository _boxRepository;
        private readonly IUserRepository _userRepository;

        public BoxUserService(IBoxRepository boxRepository, IUserRepository userRepository)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
        }

        public async Task<BoxUser> GetAsync(string box, string username)
            => await GetBoxUserAsyncOrFail(box, username);

        public async Task<IEnumerable<string>> GetUsernamesAsync(string box)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);

            return boxEntry.Users.Select(x => x.Username).ToList();
        }

        public async Task AddAsync(string box, string username, BoxRole? role = null)
        {
            var user = await GetUserAsyncOrFail(username);
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser != null)
                throw new ArgumentException($"User {username} has been already added to box {box}.", nameof(username));

            boxUser = new BoxUser(user, role.GetValueOrDefault(BoxRole.User));
            if(user.IsActive)
                boxUser.Activate();

            boxEntry.AddUser(boxUser);
            await _boxRepository.UpdateAsync(boxEntry);
        }

        public async Task ActivateAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User {username} has not been found in box {box}.");

            boxUser.Activate();
            await _boxRepository.UpdateAsync(boxEntry);
        }

        public async Task LockAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User {username} has not been found in box {box}.");

            boxUser.Lock();
            await _boxRepository.UpdateAsync(boxEntry);
        }

        public async Task AddPermissionsAsync(string box, string username, params Permission[] permissions)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            var permissionsList = permissions?.ToList() ?? new List<Permission>();
            permissionsList.ForEach(boxUser.AddPermission);
            await _boxRepository.UpdateAsync(boxEntry);
        }

        public async Task DeletePermissionsAsync(string box, string username, params Permission[] permissions)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            var permissionsList = permissions?.ToList() ?? new List<Permission>();
            permissionsList.ForEach(boxUser.DeletePermission);
            await _boxRepository.UpdateAsync(boxEntry);
        }

        public async Task DeleteAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User {username} has not been found in box {box}.");

            if (boxUser.Role == BoxRole.Admin)
            {
                var adminsCount = boxEntry.Users.Count(x => x.Role == BoxRole.Admin);
                if (adminsCount == 1)
                    throw new InvalidOperationException($"Can not remove the only one admin account in box {box}.");
            }
            boxEntry.RemoveUser(username);
            await _boxRepository.UpdateAsync(boxEntry);
        }

        private async Task<Box> GetBoxAsyncOrFail(string box)
        {
            var boxEntry = await _boxRepository.GetAsync(box);
            if (boxEntry == null)
                throw new ArgumentNullException(nameof(boxEntry), $"Box {box} has not been found.");

            return boxEntry;
        }

        private async Task<BoxUser> GetBoxUserAsyncOrFail(string box, string username)
        {
            var boxEntry = await _boxRepository.GetAsync(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User {username} has not been found in box {box}.");

            return boxUser;
        }

        private async Task<User> GetUserAsyncOrFail(string username)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User {username} has not been found.");

            return user;
        }
    }
}