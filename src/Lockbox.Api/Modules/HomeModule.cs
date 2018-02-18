namespace Lockbox.Api.Modules
{
    public class HomeModule : ModuleBase
    {
        public HomeModule()
        {
            Get("", args => "Welcome to Lockbox API!\nRead the docs at: http://docs.lockbox.apiary.io");
        }
    }
}