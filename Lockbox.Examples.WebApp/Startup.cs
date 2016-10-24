using Lockbox.Client.Extensions;
using Lockbox.Examples.WebApp.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lockbox.Examples.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddLockbox(apiUrl: "http://localhost:5000",
                    apiKey:
                    "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJTdWIiOiJ3ZWJhcHAiLCJFeHAiOjY2NzY4NDk0NDU5NjMyMzc5Mn0.TnJfzZ8UzMQ13LcXdMtKbbQYzkujejSSFVo36o8F36v51uTqJwuZc4tUh8oEN99z3DPhmDjDyKucrG5JRkxbIA",
                    entryKey: "config");

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var databaseSettings = new DatabaseSettings();
            var emailSettings = new EmailSettings();
            Configuration.GetSection("database").Bind(databaseSettings);
            Configuration.GetSection("email").Bind(emailSettings);
            services.AddSingleton(databaseSettings);
            services.AddSingleton(emailSettings);
            services.AddOptions();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvcWithDefaultRoute();
        }
    }
}