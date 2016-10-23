using Lockbox.Api.Services;
using Nancy;
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
        }
    }
}