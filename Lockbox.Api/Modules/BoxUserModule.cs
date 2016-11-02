using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class BoxUserModule : ModuleBase
    {
        private readonly IBoxService _boxService;

        public BoxUserModule(IBoxService boxService, IBoxUserService boxUserService) : base("boxes/{box}/users")
        {
            _boxService = boxService;
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var box = (string) args.box;
                var hasAccess = await HasManagementAccessAsync(box);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                var usernames = await boxUserService.GetUsernamesAsync(box);

                return usernames ?? new List<string>();
            });

            Get("{username}", async args =>
            {
                var box = (string) args.box;
                var username = (string) args.username;
                if (!username.Equals(CurrentUsername, StringComparison.CurrentCultureIgnoreCase))
                {
                    var hasAccess = await HasManagementAccessAsync(box);
                    if (!hasAccess)
                        return HttpStatusCode.Forbidden;
                }

                var user = await boxUserService.GetAsync(box, username);
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
                var box = (string) args.box;
                var hasAccess = await HasManagementAccessAsync(box);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                var request = BindRequest<AddUserToBox>();
                await boxUserService.AddAsync(box, request.Username, request.Role);

                return Created($"boxes/{box}/users/{request.Username}");
            });

            Put("{username}", async args =>
            {
                var box = (string) args.box;
                var hasAccess = await HasManagementAccessAsync(box);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                var request = BindRequest<UpdateUserInBox>();
                await boxUserService.UpdateAsync(box, request.Username, request.Role, request.IsActive);

                return HttpStatusCode.NoContent;
            });

            Delete("{username}", async args =>
            {
                var box = (string) args.box;
                var hasAccess = await HasManagementAccessAsync(box);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                await boxUserService.DeleteAsync(box, (string) args.username);

                return HttpStatusCode.NoContent;
            });
        }

        private async Task<bool> HasManagementAccessAsync(string box)
            => await _boxService.HasManagementAccess(box, CurrentUsername);
    }
}