using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Extensions;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class UserModule : ModuleBase
    {
        public UserModule(IUserService userService, FeatureSettings featureSettings) : base("users")
        {

            Get("", async args =>
            {
                RequiresAdmin();
                var usernames = await userService.GetUsernamesAsync();

                return usernames ?? new List<string>();
            });

            Get("{username}", async args =>
            {
                this.RequiresAuthentication();
                var username = (string) args.username;
                if (!username.Equals(CurrentUsername, StringComparison.CurrentCultureIgnoreCase))
                    RequiresAdmin();

                var user = await userService.GetAsync(username);
                if (user == null)
                    return HttpStatusCode.NotFound;

                return new
                {
                    username = user.Username,
                    createdAt = user.CreatedAt.FormatToString(),
                    updatedAt = user.UpdatedAt.FormatToString(),
                    role = user.Role.ToString().ToLowerInvariant(),
                    isActive = user.IsActive,
                    apiKeys = user.ApiKeys
                };
            });

            Post("", async args =>
            {
                if (featureSettings.RequireAdminToCreateUser)
                {
                    RequiresAdmin();
                }
                var request = BindRequest<CreateUser>();
                await userService.CreateAsync(request.Username, request.Password, request.Role);
                var user = await userService.GetAsync(request.Username);

                return Created($"users/{request.Username}")
                    .WithHeader("X-API-Key", user.ApiKeys.First());
            });

            Delete("{username}", async args =>
            {
                RequiresAdmin();
                await userService.DeleteAsync((string) args.username);

                return HttpStatusCode.NoContent;
            });
        }
    }
}