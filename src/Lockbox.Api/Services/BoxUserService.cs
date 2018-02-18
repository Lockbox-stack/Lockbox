using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using NLog;

namespace Lockbox.Api.Services
{
    public class BoxUserService : IBoxUserService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IBoxRepository _boxRepository;
        private readonly IUserRepository _userRepository;
        private readonly FeatureSettings _featureSettings;

        public BoxUserService(IBoxRepository boxRepository, IUserRepository userRepository, FeatureSettings featureSettings)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            _featureSettings = featureSettings;
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
                throw new ArgumentException($"User '{username}' has been already added to box '{box}'.", nameof(username));

            if (boxEntry.Users.Count() >= _featureSettings.UsersPerBoxLimit)
            {
                throw new InvalidOperationException($"Box: '{box}' already contains " +
                                                    $"{_featureSettings.UsersPerBoxLimit} users.");
            }

            boxUser = new BoxUser(user, role.GetValueOrDefault(BoxRole.BoxUser));
            if(user.IsActive)
                boxUser.Activate();

            boxUser.AddPermission(Permission.ReadEntryKeys);
            boxUser.AddPermission(Permission.ReadEntry);
            boxEntry.AddUser(boxUser);
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Info($"User '{username}' was added to the box '{boxEntry.Name}'.");
        }

        public async Task UpdateAsync(string box, string username, BoxRole? role = null, bool? isActive = null)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box '{box}'.");

            if (role.HasValue)
                boxUser.SetRole(role.Value);
            if (isActive.HasValue)
            {
                if(isActive.Value)
                    boxUser.Activate();
                else
                    boxUser.Lock();
            }
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Info($"User '{username}' was added updated in the box '{boxEntry.Name}'.");
        }

        public async Task ActivateAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box {box}.");

            boxUser.Activate();
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Info($"User '{username}' was activated in the box '{boxEntry.Name}'.");
        }

        public async Task LockAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box {box}.");

            boxUser.Lock();
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Info($"User '{username}' was locked in the box '{boxEntry.Name}'.");
        }

        public async Task DeleteAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box {box}.");

            if (boxEntry.Owner.Equals(boxUser.Username))
            {
                throw new InvalidOperationException($"Box '{box}' owner '{boxUser.Username}' can not be deleted.");
            }
            if (boxUser.Role == BoxRole.BoxAdmin)
            {
                var adminsCount = boxEntry.Users.Count(x => x.Role == BoxRole.BoxAdmin);
                if (adminsCount == 1)
                    throw new InvalidOperationException($"Can not delete the only one admin account in box {box}.");
            }
            boxEntry.DeleteUser(username);
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Info($"User '{username}' was deleted from the box '{boxEntry.Name}'.");
        }

        private async Task<Box> GetBoxAsyncOrFail(string box)
        {
            var boxEntry = await _boxRepository.GetAsync(box);
            if (boxEntry == null)
                throw new ArgumentNullException(nameof(boxEntry), $"Box '{box}' has not been found.");

            return boxEntry;
        }

        private async Task<BoxUser> GetBoxUserAsyncOrFail(string box, string username)
        {
            var boxEntry = await _boxRepository.GetAsync(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box {box}.");

            return boxUser;
        }

        private async Task<User> GetUserAsyncOrFail(string username)
        {
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User '{username} has not been found.");

            return user;
        }
    }
}