using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using Serilog;

namespace Lockbox.Api.Services
{
    public class BoxUserPermissionsService : IBoxUserPermissionsService
    {
        private static readonly ILogger Logger = Log.Logger;
        private readonly IBoxRepository _boxRepository;

        public BoxUserPermissionsService(IBoxRepository boxRepository)
        {
            _boxRepository = boxRepository;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box '{box}'.");

            return boxUser.Permissions.ToList();
        }

        public async Task DeleteAllAsync(string box, string username)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box '{box}'.");

            boxUser.DeleteAllPermissions();
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Information($"User '{username}' permissions in box '{boxEntry.Name}' were deleted.");
        }

        public async Task UpdateAsync(string box, string username, params Permission[] permissions)
        {
            var boxEntry = await GetBoxAsyncOrFail(box);
            var boxUser = boxEntry.GetUser(username);
            if (boxUser == null)
                throw new ArgumentNullException(nameof(boxUser), $"User '{username}' has not been found in box {box}.");

            boxUser.DeleteAllPermissions();
            var selectedPermissions = permissions?.ToList() ?? new List<Permission>();
            selectedPermissions.ForEach(boxUser.AddPermission);
            await _boxRepository.UpdateAsync(boxEntry);
            Logger.Information($"User '{username}' permissions in box '{boxEntry.Name}' were updated.");
        }

        private async Task<Box> GetBoxAsyncOrFail(string box)
        {
            var boxEntry = await _boxRepository.GetAsync(box);
            if (boxEntry == null)
                throw new ArgumentNullException(nameof(boxEntry), $"Box '{box}' has not been found.");

            return boxEntry;
        }
    }
}