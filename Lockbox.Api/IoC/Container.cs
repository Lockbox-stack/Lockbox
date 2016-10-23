using System.Linq;
using System.Reflection;
using Autofac;
using Lockbox.Api.Extensions;
using Lockbox.Api.IoC.Modules;
using Lockbox.Api.Services;
using Microsoft.Extensions.Configuration;

namespace Lockbox.Api.IoC
{
    public static class Container
    {
        public static void Initialize(ContainerBuilder builder, IConfiguration configuration)
        {
            RegisterSettings(builder, configuration);
            builder.RegisterModule<MongoDbModule>();
            builder.RegisterType<RecordService>().As<IRecordService>();
        }

        private static void RegisterSettings(ContainerBuilder builder, IConfiguration configuration)
        {
            var assembly = Assembly.Load(new AssemblyName("Lockbox.Api"));
            var settingsTypes = assembly.GetTypes()
                .Where(x => x.Name.EndsWith("Settings") || x.Name.EndsWith("Configuration"));
            foreach (var settingType in settingsTypes)
            {
                builder.RegisterInstance(configuration.GetSettings(settingType)).As(settingType).SingleInstance();
            }
        }
    }
}