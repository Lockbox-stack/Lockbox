using System;
using Autofac;
using Lockbox.Core.Settings;
using MongoDB.Driver;

namespace Lockbox.Core.IoC.Modules
{
    public class MongoDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var settings = c.Resolve<MongoDbSettings>();
                var mongoSettings = new MongoClientSettings
                {
                    Server = new MongoServerAddress(settings.Server, settings.Port),
                    MaxConnectionIdleTime = TimeSpan.FromMinutes(settings.MaxConnectionIdleTimeMinutes)
                };
                var credential = MongoCredential.CreateCredential(settings.Database, settings.Username,
                    settings.Password);
                if (credential != null)
                    mongoSettings.Credentials = new[] {credential};

                return new MongoClient(mongoSettings);
            }).SingleInstance();

            builder.Register((c, p) =>
            {
                var mongoClient = c.Resolve<MongoClient>();
                var settings = c.Resolve<MongoDbSettings>();
                var database = mongoClient.GetDatabase(settings.Database);
                return database;
            }).As<IMongoDatabase>();
        }
    }
}