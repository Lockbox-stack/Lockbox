using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.Security;

namespace Lockbox.Api.Modules
{
    public class ApiKeyModule : ModuleBase
    {
        public ApiKeyModule(IApiKeyService apiKeyService) : base("api-keys")
        {
            this.RequiresAuthentication();

            Post("", async args =>
            {
                var request = BindRequest<CreateApiKey>();
                var apiKey = await apiKeyService.CreateAsync(CurrentUsername, request.Expiry);

                return new {apiKey};
            });

            Delete("{apiKey}", async args =>
            {
                await apiKeyService.DeleteAsync((string)args.apiKey);

                return HttpStatusCode.NoContent;
            });
        }
    }
}