using System.Reflection;
using Autofac;
using Lockbox.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Lockbox.Core.IoC.Modules;
using Lockbox.Core.MongoDb;
using Lockbox.Core.Repositories;
using Lockbox.Core.Services;

namespace Lockbox.Core.IoC
{
    public static class Container
    {
        public static void Initialize(ContainerBuilder builder, IConfiguration configuration)
        {
            RegisterAllSettings(builder, configuration);
            builder.RegisterModule<MongoDbModule>();
            builder.RegisterType<MongoDbRecordRepository>().As<IRecordRepository>();
            builder.RegisterType<RecordService>().As<IRecordService>();
        }

        public static void RegisterAllSettings(ContainerBuilder builder, IConfiguration configuration)
            => RegisterSettings(builder, configuration, Assembly.Load(new AssemblyName("Lockbox.Core")));

        public static void RegisterCustomSettings(ContainerBuilder builder, IConfiguration configuration,
            Assembly assembly) => RegisterSettings(builder, configuration, assembly);

        private static void RegisterSettings(ContainerBuilder builder, IConfiguration configuration, Assembly assembly)
        {
            var settingsTypes = assembly.GetTypes()
                .Where(x => x.Name.EndsWith("Settings") || x.Name.EndsWith("Configuration"));
            foreach (var settingType in settingsTypes)
            {
                builder.RegisterInstance(configuration.GetSettings(settingType)).As(settingType).SingleInstance();
            }
        }
    }
}