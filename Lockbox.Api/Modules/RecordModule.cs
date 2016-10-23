using Lockbox.Api.Services;
using Nancy;

namespace Lockbox.Api.Modules
{
    public class RecordModule : NancyModule
    {
        public RecordModule(IRecordService recordService) : base("records")
        {
            Get("{name}", async args =>
            {
                var record = await recordService.GetValueAsync((string) args.name);

                return record ?? HttpStatusCode.NotFound;
            });
        }
    }
}