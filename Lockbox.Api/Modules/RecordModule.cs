using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class RecordModule : NancyModule
    {
        public RecordModule(IRecordService recordService) : base("records")
        {
            this.RequiresAuthentication();

            Get("{name}", async args =>
            {
                var record = await recordService.GetValueAsync((string) args.name);

                return record ?? HttpStatusCode.NotFound;
            });

            Post("", async args =>
            {
                var request = this.Bind<CreateRecord>();
                await recordService.CreateAsync(request.Key, request.Value);

                return Negotiate.WithHeader("Location", $"/records/{request.Key.ToLowerInvariant()}")
                    .WithStatusCode(HttpStatusCode.Created);
            });
        }
    }
}