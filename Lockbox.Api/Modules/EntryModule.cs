using System.Collections.Generic;
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

            Post("{key}", async args =>
            {
                var key = (string) args.key;
                var value = BindRequest<object>();
                await entryPermissionService.CreateAsync(CurrentUsername,
                    (string)args.box, key, value, EncryptionKey);

                return Created($"entries/{key}");
            });

            Delete("{key}", async args =>
            {
                await entryPermissionService.DeleteAsync(CurrentUsername, (string)args.box, (string) args.key);

                return HttpStatusCode.NoContent;
            });
        }
    }
}