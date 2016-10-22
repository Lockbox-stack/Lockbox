using Autofac;
using Lockbox.Core.IoC;
using Lockbox.Core.MongoDb;
using Microsoft.Extensions.Configuration;
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
            Logger.Info("Lockbox has started.");
            MongoConfigurator.Initialize();
        }

        protected override void ConfigureApplicationContainer(ILifetimeScope existingContainer)
        {
            base.ConfigureApplicationContainer(existingContainer);
            var builder = new ContainerBuilder();
            Container.Initialize(builder, _configuration);
            builder.Update(existingContainer.ComponentRegistry);
        }
    }
}