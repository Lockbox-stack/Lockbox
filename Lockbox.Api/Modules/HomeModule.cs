using Nancy;

namespace Lockbox.Api.Modules
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("", args => "Welcome to Lockbox!");
        }
    }
}