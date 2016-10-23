using Lockbox.Api.Requests;
using Lockbox.Api.Services;

namespace Lockbox.Api.Modules
{
    public class InitializationModule : ModuleBase
    {
        public InitializationModule(IInitializationService initializationService)
        {
            Post("init", async args =>
            {
                var request = BindRequest<InitializeLockbox>();
                var apiKey = await initializationService.InitializeAsync(request.Username, request.Password);

                return new { apiKey };
            });
        }
    }
}