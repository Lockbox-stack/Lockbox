using Lockbox.Api.Requests;
using Lockbox.Api.Services;
using Nancy;
using Nancy.ModelBinding;

namespace Lockbox.Api.Modules
{
    public class InitializationModule : NancyModule
    {
        public InitializationModule(IInitializationService initializationService)
        {
            Post("init", async args =>
            {
                var request = this.Bind<InitializeLockbox>();
                await initializationService.InitializeAsync(request.Username, request.Password);

                return HttpStatusCode.NoContent;
            });
        }
    }
}