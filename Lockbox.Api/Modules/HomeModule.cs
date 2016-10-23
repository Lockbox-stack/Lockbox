using Nancy;

namespace Lockbox.Api.Modules
{
    public class HomeModule : ModuleBase
    {
        public HomeModule()
        {
            Get("", args => "Welcome to Lockbox!");
        }
    }
}