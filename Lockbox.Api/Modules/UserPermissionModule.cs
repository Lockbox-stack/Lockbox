using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Domain;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class UserPermissionModule : ModuleBase
    {
        private readonly IUserPermissionsService _userPermissionsService;

        public UserPermissionModule(IUserPermissionsService userPermissionsService) : base("users/{username}/permissions")
        {
            _userPermissionsService = userPermissionsService;
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var username = (string) args.username;
                if (!username.Equals(CurrentUsername, StringComparison.CurrentCultureIgnoreCase))
                    RequiresAdmin();

                var permissions = await _userPermissionsService.GetAllAsync(username);

                return permissions.Select(x => x.ToString()).ToList();
            });

            Put("", async args =>
            {
                RequiresAdmin();
                var permissions = BindRequest<IEnumerable<string>>() ?? new List<string>();
                var selectedPermissions = permissions.Select(x => Enum.Parse(typeof(Permission), x, true))
                    .Cast<Permission>()
                    .ToArray();

                await _userPermissionsService.UpdateAsync((string)args.username, selectedPermissions);

                return HttpStatusCode.NoContent;
            });

            Delete("", async args =>
            {
                RequiresAdmin();
                await _userPermissionsService.DeleteAllAsync((string) args.username);

                return HttpStatusCode.NoContent;
            });
        }
    }
}