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
            builder.RegisterType<AuthenticationService>().As<IAuthenticationService>();
            builder.RegisterType<BoxService>().As<IBoxService>();
            builder.RegisterType<BoxUserPermissionsService>().As<IBoxUserPermissionsService>();
            builder.RegisterType<BoxUserService>().As<IBoxUserService>();
            builder.RegisterType<Encrypter>().As<IEncrypter>().SingleInstance();
            builder.RegisterType<EntryService>().As<IEntryService>();
            builder.RegisterType<EntryPermissionService>().As<IEntryPermissionService>();
            builder.RegisterType<InitializationService>().As<IInitializationService>();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
            builder.RegisterType<UserService>().As<IUserService>();
        }
    }
}