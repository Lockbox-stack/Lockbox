using System.Collections.Generic;
using Lockbox.Api.Services;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class EntryKeyModule : ModuleBase
    {
        public EntryKeyModule(IEntryPermissionService entryPermissionService) : base("keys")
        {
            this.RequiresAuthentication();

            Get("", async args =>
            {
                var keys = await entryPermissionService.GetKeysAsync(CurrentUsername);

                return keys ?? new List<string>();
            });
        }
    }
}