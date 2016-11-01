using System;
using Autofac;
using Lockbox.Api.MongoDb;
using Lockbox.Api.Repositories;
using MongoDB.Driver;

namespace Lockbox.Api.IoC.Modules
{
    public class MongoDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
            {
                var settings = c.Resolve<MongoDbSettings>();

                return new MongoClient(settings.ConnectionString);
            }).SingleInstance();

            builder.Register((c, p) =>
            {
                var mongoClient = c.Resolve<MongoClient>();
                var settings = c.Resolve<MongoDbSettings>();
                var database = mongoClient.GetDatabase(settings.Database);
                return database;
            }).As<IMongoDatabase>();

            builder.RegisterType<BoxRepository>().As<IBoxRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
        }
    }
}