using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Domain;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class BoxUserPermissionModule : ModuleBase
    {
        public BoxUserPermissionModule(IBoxUserPermissionsService boxUserPermissionsService)
            : base("boxes/{box}/users/{username}/permissions")
        {
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var username = (string) args.username;
                if (!username.Equals(CurrentUsername, StringComparison.CurrentCultureIgnoreCase))
                    RequiresAdmin();

                var permissions = await boxUserPermissionsService.GetAllAsync((string) args.box, username);

                return permissions.Select(x => x.ToString()).ToList();
            });

            Put("", async args =>
            {
                RequiresAdmin();
                var permissions = BindRequest<IEnumerable<string>>() ?? new List<string>();
                var selectedPermissions = permissions.Select(x => Enum.Parse(typeof(Permission), x, true))
                    .Cast<Permission>()
                    .ToArray();

                await
                    boxUserPermissionsService.UpdateAsync((string) args.box, (string) args.username, selectedPermissions);

                return HttpStatusCode.NoContent;
            });

            Delete("", async args =>
            {
                RequiresAdmin();
                await boxUserPermissionsService.DeleteAllAsync((string) args.box, (string) args.username);

                return HttpStatusCode.NoContent;
            });
        }
    }
}