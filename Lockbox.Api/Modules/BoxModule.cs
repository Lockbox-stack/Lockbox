using System.Linq;
using Lockbox.Api.Extensions;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class BoxModule : ModuleBase
    {

        public BoxModule(IBoxService boxService) : base("boxes")
        {
            this.RequiresAuthentication();

            Get("", async args => await boxService.GetNamesForUserAsync(CurrentUsername));

            Get("{name}", async args =>
            {
                var name = (string) args.name;
                var hasAccess = await boxService.HasAccessAsync(name, CurrentUsername);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                var box = await boxService.GetAsync(name);
                if (box == null)
                    return HttpStatusCode.NotFound;

                return new
                {
                    name = box.Name,
                    owner = box.Owner,
                    createdAt = box.CreatedAt.FormatToString(),
                    updatedAt = box.UpdatedAt.FormatToString(),
                    entries = box.Entries.Select(x => x.Key).ToList(),
                    users = box.Users.Select(x => new
                    {
                        username = x.Username,
                        role = x.Role.ToString().ToLowerInvariant(),
                        isActive = x.IsActive
                    })
                };
            });

            Post("{name}", async args =>
            {
                var name = (string) args.name;
                await boxService.CreateAsync(name, CurrentUsername);

                return Created($"boxes/{name}");
            });

            Delete("{name}", async args =>
            {
                var name = (string) args.name;
                var hasAccess = await boxService.HasManagementAccess(name, CurrentUsername);
                if (!hasAccess)
                    return HttpStatusCode.Forbidden;

                await boxService.DeleteAsync(name);

                return HttpStatusCode.NoContent;
            });
        }
    }
}