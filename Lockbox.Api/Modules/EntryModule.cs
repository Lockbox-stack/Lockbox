using System.Collections.Generic;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class EntryModule : ModuleBase
    {
        public EntryModule(IEntryPermissionService entryPermissionService) : base("entries")
        {
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var keys = await entryPermissionService.GetKeysAsync(CurrentUsername);

                return keys ?? new List<string>();
            });

            Get("{key}", async args =>
            {
                var entry = await entryPermissionService.GetValueAsync(CurrentUsername, (string) args.key);

                return entry ?? HttpStatusCode.NotFound;
            });

            Post("", async args =>
            {
                var request = BindRequest<CreateEntry>();
                await entryPermissionService.CreateAsync(CurrentUsername, request.Key, request.Value);

                return Created($"entries/{request.Key}");
            });

            Delete("{key}", async args =>
            {
                await entryPermissionService.DeleteAsync(CurrentUsername, (string)args.key);

                return HttpStatusCode.NoContent;
            });
        }
    }
}