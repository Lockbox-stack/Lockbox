using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.ModelBinding;

namespace Lockbox.Api.Modules
{
    public class ApiKeyModule : NancyModule
    {
        public ApiKeyModule(IApiKeyService apiKeyService) : base("api-keys")
        {
            Post("", async args =>
            {
                var request = this.Bind<CreateApiKey>();
                var apiKey = await apiKeyService.CreateAsync(request.Username, request.Password);

                return new {apiKey};
            });

            Delete("{apiKey}", async args =>
            {
                await apiKeyService.RemoveAsync((string)args.apiKey);

                return HttpStatusCode.NoContent;
            });
        }
    }
}