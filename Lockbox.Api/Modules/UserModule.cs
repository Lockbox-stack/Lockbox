using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;

namespace Lockbox.Api.Modules
{
    public class UserModule : ModuleBase
    {
        public UserModule(IUserService userService) : base("users")
        {
            RequiresAdmin();

            Get("", async args =>
            {
                var usernames = await userService.GetUsernamesAsync();

                return usernames ?? new List<string>();
            });

            Get("{username}", async args =>
            {
                var user = await userService.GetAsync((string) args.username);
                if (user == null)
                    return HttpStatusCode.NotFound;

                return new
                {
                    username = user.Username,
                    role = user.Role,
                    isActive = user.IsActive,
                    apiKeys = user.ApiKeys,
                    permissions = user.Permissions.Select(x => x.ToString()).ToList()
                };
            });

            Post("", async args =>
            {
                var request = BindRequest<CreateUser>();
                await userService.CreateAsync(request.Username, request.Password, request.Role);

                return Created($"users/{request.Username}");
            });

            Delete("{username}", async args =>
            {
                await userService.DeleteAsync((string) args.username);

                return HttpStatusCode.NoContent;
            });
        }
    }
}