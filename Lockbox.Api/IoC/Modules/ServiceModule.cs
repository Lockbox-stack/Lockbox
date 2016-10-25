using Autofac;
using Lockbox.Api.Domain;
using Lockbox.Api.Services;

namespace Lockbox.Api.IoC.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ApiKeyService>().As<IApiKeyService>();
            builder.RegisterType<Encrypter>().As<IEncrypter>().SingleInstance();
            builder.RegisterType<InitializationService>().As<IInitializationService>();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
            builder.RegisterType<EntryService>().As<IEntryService>();
            builder.RegisterType<EntryPermissionService>().As<IEntryPermissionService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<UserPermissionsService>().As<IUserPermissionsService>();
        }
    }
}