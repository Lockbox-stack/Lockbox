using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Lockbox.Api.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy.Owin;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;

namespace Lockbox.Api
{
    public class LockboxStartup
    {
        protected string EnvironmentName { get; set; }
        protected IConfiguration Configuration { get; set; }
        protected IContainer ApplicationContainer { get; set; }

        public LockboxStartup(IHostingEnvironment env)
        {
            EnvironmentName = env.EnvironmentName.ToLowerInvariant();
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .SetBasePath(env.ContentRootPath);

            Configuration = builder.Build();
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddWebEncoders();
            services.AddCors();
            ApplicationContainer = GetServiceContainer(services);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            var logLevel = LogEventLevel.Debug;
            var configuration = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Is(logLevel);
            configuration.WriteTo.Console(new JsonFormatter(), logLevel);
            Log.Logger = configuration.CreateLogger();
            app.UseCors(builder => builder.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowAnyOrigin()
               .AllowCredentials());
            app.UseOwin().UseNancy(x => x.Bootstrapper = new Bootstrapper(Configuration));
        }

        protected static IContainer GetServiceContainer(IEnumerable<ServiceDescriptor> services)
        {
            var builder = new ContainerBuilder();
            builder.Populate(services);

            return builder.Build();
        }
    }
}