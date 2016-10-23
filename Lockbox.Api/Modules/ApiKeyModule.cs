using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;

namespace Lockbox.Api.Modules
{
    public class ApiKeyModule : ModuleBase
    {
        public ApiKeyModule(IApiKeyService apiKeyService) : base("api-keys")
        {
            Post("", async args =>
            {
                var request = BindBasicAuthenticationRequest<CreateApiKey>();
                var apiKey = await apiKeyService.CreateAsync(request.Username, request.Password,
                    request.Expiry);

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