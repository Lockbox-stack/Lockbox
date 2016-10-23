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
                .AddLockbox("http://localhost:5000",
                "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJTdWIiOiJzcGV0eiIsIkV4cCI6NjY3Njg0NzcwMjEyNDIyOTc1fQ._01iOCc_pIWqxr0673wKVSIPzcNSxair7D0FKOr611laA9MT-fDlRG0kb_QfEu-uLklpW_-9_kvUR6K5AwGb7A", 
                "config");

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