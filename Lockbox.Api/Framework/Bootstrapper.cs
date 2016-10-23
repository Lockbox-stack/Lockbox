using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using Lockbox.Api.IoC;
using Lockbox.Api.MongoDb;
using Lockbox.Api.Services;
using Microsoft.Extensions.Configuration;
using Nancy.Authentication.Stateless;
using Nancy.Bootstrapper;
using NLog;

namespace Lockbox.Api.Framework
{
    public class Bootstrapper : AutofacNancyBootstrapper
    {
        private readonly IConfiguration _configuration;
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        public Bootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ApplicationStartup(ILifetimeScope container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            MongoConfigurator.Initialize();
            SetupTokenAuthentication(container, pipelines);
            Logger.Info("Lockbox API has started.");
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            base.ConfigureApplicationContainer(existingContainer);
            var builder = new ContainerBuilder();
            Container.Initialize(builder, _configuration);
            builder.Update(existingContainer.ComponentRegistry);
        }

        protected void SetupTokenAuthentication(ILifetimeScope container, IPipelines pipelines)
        {
            var jwtTokenHandler = container.Resolve<IJwtTokenHandler>();
            var apiKeyService = container.Resolve<IApiKeyService>();
            var configuration = new StatelessAuthenticationConfiguration(ctx =>
            {
                var apikey = jwtTokenHandler.GetFromAuthorizationHeader(ctx.Request.Headers.Authorization);
                var token = jwtTokenHandler.Decode(apikey);
                var isValid = apiKeyService.IsValidAsync(apikey).Result;

                return isValid ? new ClaimsPrincipal(new GenericIdentity(token.Sub)) : null;
            });
            StatelessAuthentication.Enable(pipelines, configuration);
        }
    }
}