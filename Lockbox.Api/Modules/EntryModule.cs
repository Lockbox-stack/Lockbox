using System.Collections.Generic;
using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class EntryModule : ModuleBase
    {
        public EntryModule(IEntryPermissionService entryPermissionService) : base("boxes/{box}/entries")
        {
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var keys = await entryPermissionService.GetKeysAsync((string)args.box, CurrentUsername);

                return keys ?? new List<string>();
            });

            Get("{key}", async args =>
            {
                var entry = await entryPermissionService.GetValueAsync(CurrentUsername,
                    (string)args.box, (string) args.key, EncryptionKey);

                return entry ?? HttpStatusCode.NotFound;
            });

            Post("", async args =>
            {
                var request = BindRequest<CreateEntry>();
                await entryPermissionService.CreateAsync(CurrentUsername,
                    (string) args.box, request.Key, request.Value, EncryptionKey);

                return Created($"entries/{request.Value}");
            });

            Delete("{key}", async args =>
            {
                await entryPermissionService.DeleteAsync(CurrentUsername, (string)args.box, (string) args.key);

                return HttpStatusCode.NoContent;
            });
        }
    }
}