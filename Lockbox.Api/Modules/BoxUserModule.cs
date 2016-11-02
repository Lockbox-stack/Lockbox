using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class BoxUserModule : ModuleBase
    {
        public BoxUserModule(IBoxUserService boxUserService) : base("boxes/{box}/users")
        {
            this.RequiresAuthentication();

            Get("", async args =>
            {
                RequiresAdmin();
                var usernames = await boxUserService.GetUsernamesAsync((string)args.box);

                return usernames ?? new List<string>();
            });

            Get("{username}", async args =>
            {
                var username = (string) args.username;
                if (!username.Equals(CurrentUsername, StringComparison.CurrentCultureIgnoreCase))
                    RequiresAdmin();

                var user = await boxUserService.GetAsync((string)args.box, username);
                if (user == null)
                    return HttpStatusCode.NotFound;

                return new
                {
                    username = user.Username,
                    role = user.Role.ToString().ToLowerInvariant(),
                    isActive = user.IsActive,
                    permissions = user.Permissions.Select(x => x.ToString().ToLowerInvariant()).ToList()
                };
            });

            Post("", async args =>
            {
                RequiresAdmin();
                var request = BindRequest<AddUserToBox>();
                var box = (string) args.box;
                await boxUserService.AddAsync(box, request.Username, request.Role);

                return Created($"boxes/{box}/users/{request.Username}");
            });

            Put("{username}/lock", async args =>
            {
                RequiresAdmin();
                await boxUserService.LockAsync((string) args.box, (string) args.username);

                return HttpStatusCode.NoContent;
            });

            Put("{username}/activate", async args =>
            {
                RequiresAdmin();
                await boxUserService.ActivateAsync((string)args.box, (string)args.username);

                return HttpStatusCode.NoContent;
            });

            Delete("{username}", async args =>
            {
                RequiresAdmin();
                await boxUserService.DeleteAsync((string)args.box, (string) args.username);

                return HttpStatusCode.NoContent;
            });
        }
    }
}