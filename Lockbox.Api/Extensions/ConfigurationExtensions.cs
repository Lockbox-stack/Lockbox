using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Lockbox.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        private static readonly string EncryptionKeyEnvironmentVariable = "LOCKBOX_ENCRYPTION_KEY";
        private static readonly string SecretKeyEnvironmentVariable = "LOCKBOX_SECRET_KEY";

        public static T GetSettings<T>(this IConfiguration configuration, string section = "") where T : class, new()
        => GetSettings(configuration, typeof(T), section) as T;

        public static object GetSettings(this IConfiguration configuration, Type type, string section = "")
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                section = type.Name.Replace("Settings", string.Empty).Replace("Configuration", string.Empty);
            }

            var configurationValue = Activator.CreateInstance(type);
            configuration.GetSection(section).Bind(configurationValue);

            return type == typeof(LockboxSettings)
                ? InitializeLockboxSettings(configurationValue as LockboxSettings, configuration)
                : configurationValue;
        }

        private static LockboxSettings InitializeLockboxSettings(LockboxSettings settings, IConfiguration configuration)
        {
            if (settings.EncryptionKey.Empty())
            {
                settings.EncryptionKey = configuration.GetChildren()
                    .FirstOrDefault(x => x.Key == EncryptionKeyEnvironmentVariable)?.Value;
            }
            if (settings.SecretKey.Empty())
            {
                settings.SecretKey = configuration.GetChildren()
                    .FirstOrDefault(x => x.Key == SecretKeyEnvironmentVariable)?.Value;
            }

            if (settings.EncryptionKey.Empty())
                throw new ArgumentException("Lockbox encryption key can not be empty!", nameof(settings.EncryptionKey));
            if (settings.SecretKey.Empty())
                throw new ArgumentException("Lockbox secret key can not be empty!", nameof(settings.SecretKey));


            return settings;
        }
    }
}