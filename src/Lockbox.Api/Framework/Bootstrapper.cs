using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using Autofac;
using Lockbox.Api.Domain;
using Lockbox.Api.Extensions;
using Lockbox.Api.IoC;
using Lockbox.Api.MongoDb;
using Lockbox.Api.Services;
using Microsoft.Extensions.Configuration;
using Nancy;
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
            pipelines.AfterRequest += (ctx) =>
            {
                AddCorsHeaders(ctx.Response);
            };
            Logger.Info("Lockbox API has started.");
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            base.ConfigureApplicationContainer(existingContainer);
            var builder = new ContainerBuilder();
            Container.Initialize(builder, _configuration);
            builder.Update(existingContainer.ComponentRegistry);
        }

        protected override void RequestStartup(ILifetimeScope container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);
            pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
            {
                ctx.Response = ErrorResponse.FromException(ex, context.Environment);
                AddCorsHeaders(ctx.Response);

                return ctx.Response;
            });
        }

        protected void SetupTokenAuthentication(ILifetimeScope container, IPipelines pipelines)
        {
            var jwtTokenHandler = container.Resolve<IJwtTokenHandler>();
            var apiKeyService = container.Resolve<IApiKeyService>();
            var userService = container.Resolve<IUserService>();
            var configuration = new StatelessAuthenticationConfiguration(ctx =>
            {
                var token = jwtTokenHandler.GetFromAuthorizationHeader(ctx.Request.Headers.Authorization);
                if (token == null)
                    return null;
                
                //TODO: Cache users.
                User user = null;
                var isValid = jwtTokenHandler.IsValid(token);
                if (isValid)
                {
                    user = userService.GetAsync(token.Sub).Result;
                    if (user == null || !user.IsActive)
                        return null;

                    return GetIdentity(token.Sub, user.Role);
                }

                user = apiKeyService.GetUserAsync(token.ApiKey).Result;
                isValid = apiKeyService.IsValid(user, token.ApiKey);
                if (!isValid)
                    return null;

                return GetIdentity(token.Sub, user.Role);
            });
            StatelessAuthentication.Enable(pipelines, configuration);
        }

        private static void AddCorsHeaders(Response response)
        {
            response.WithHeader("Access-Control-Allow-Origin", "*")
                .WithHeader("Access-Control-Allow-Methods", "POST,PUT,GET,OPTIONS,DELETE")
                .WithHeader("Access-Control-Allow-Headers", "Authorization,Accept,Origin," +
                                                            "Connection,Content-Type,User-Agent,X-Requested-With,X-API-Key")
                .WithHeader("Access-Control-Expose-Headers", "X-API-Key");
        }

        private static ClaimsPrincipal GetIdentity(string username, Role role)
        {
                return new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role.ToString().ToLowerInvariant())
                }, "Bearer", ClaimTypes.Name, ClaimTypes.Role));
        }
    }
}