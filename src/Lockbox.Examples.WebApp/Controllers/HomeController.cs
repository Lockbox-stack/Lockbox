using Lockbox.Examples.WebApp.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lockbox.Examples.WebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly DatabaseSettings _databaseSettings;
        private readonly EmailSettings _emailSettings;

        public HomeController(DatabaseSettings databaseSettings, EmailSettings emailSettings)
        {
            _databaseSettings = databaseSettings;
            _emailSettings = emailSettings;
        }

        [Route("")]
        public IActionResult Get()
        {
            var settings = new
            {
                database = _databaseSettings,
                email = _emailSettings
            };

            return new JsonResult(new {message = "Successfully initialized app settings using Lockbox.", settings});
        }
    }
}