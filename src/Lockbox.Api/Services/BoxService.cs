using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using Lockbox.Api.Extensions;
using NLog;

namespace Lockbox.Api.Services
{
    public class BoxService : IBoxService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IBoxRepository _boxRepository;
        private readonly IUserRepository _userRepository;
        private readonly FeatureSettings _featureSettings;

        public BoxService(IBoxRepository boxRepository, IUserRepository userRepository, FeatureSettings featureSettings)
        {
            _boxRepository = boxRepository;
            _userRepository = userRepository;
            _featureSettings = featureSettings;
        }

        public async Task<Box> GetAsync(string name)
            => await _boxRepository.GetAsync(name);

        public async Task<IEnumerable<string>> GetNamesAsync()
            => await _boxRepository.GetNamesAsync();

        public async Task<IEnumerable<string>> GetNamesForUserAsync(string username)
            => await _boxRepository.GetNamesForUserAsync(username);

        public async Task<bool> HasAccessAsync(string box, string username)
            => await HasAccess(box, username, management: false);

        public async Task<bool> HasManagementAccess(string box, string username)
            => await HasAccess(box, username, management: true);

        private async Task<bool> HasAccess(string box, string username, bool management = false)
        {
            var boxAndUser = await GetBoxAndUserAsyncOrFail(box, username);
            var entryBox = boxAndUser.Item1;
            var user = boxAndUser.Item2;
            if (!user.IsActive)
                return false;

            return user.Role == Role.Admin ||
                   (management ? entryBox.HasManagementAccess(username) : entryBox.HasAccess(username));
        }

        public async Task CreateAsync(string name, string owner)
        {
            if (name.Empty())
                throw new ArgumentException("Box name can not be empty.", nameof(name));
            if (owner.Empty())
                throw new ArgumentException("Box owner has not been provided.", nameof(owner));

            var user = await _userRepository.GetAsync(owner);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User '{owner}' has not been found.");
            if (!user.IsActive)
                throw new AuthenticationException($"User '{owner}' is not active.");

            var boxes = await GetNamesForUserAsync(owner);
            var boxesLimit = _featureSettings.BoxesPerUserLimit;
            if (boxes.Count() >= boxesLimit && user.Role == Role.User)
                throw new AuthenticationException($"User '{owner}' can not create more than {boxesLimit} box(es).");

            var entryBox = await _boxRepository.GetAsync(name);
            if (entryBox != null)
                throw new ArgumentException($"Box '{name}' already exists.", nameof(name));

            var box = new Box(name, user);
            await _boxRepository.AddAsync(box);
            Logger.Info($"Box '{box.Name}' was created by user '{owner}'.");
        }

        public async Task DeleteAsync(string name)
        {
            var entryBox = await _boxRepository.GetAsync(name);
            if (entryBox == null)
                throw new ArgumentNullException($"Box '{name}' has not been found.");

            await _boxRepository.DeleteAsync(name);
            Logger.Info($"Box '{entryBox.Name}' was deleted.");
        }

        private async Task<Tuple<Box, User>> GetBoxAndUserAsyncOrFail(string box, string username)
        {
            var entryBox = await _boxRepository.GetAsync(box);
            if (entryBox == null)
                throw new ArgumentNullException($"Box '{box}' has not been found.");
            var user = await _userRepository.GetAsync(username);
            if (user == null)
                throw new ArgumentNullException(nameof(user), $"User '{username}' has not been found.");

            return new Tuple<Box, User>(entryBox, user);
        }
    }
}