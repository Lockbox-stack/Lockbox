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
                ? InitializeLockboxSettings(configurationValue as LockboxSettings)
                : configurationValue;
        }

        private static LockboxSettings InitializeLockboxSettings(LockboxSettings settings)
        {
            settings.EncryptionKey = GetParameterOrFail(settings.EncryptionKey,
                EncryptionKeyEnvironmentVariable, "encryption key");
            settings.SecretKey = GetParameterOrFail(settings.SecretKey,
                SecretKeyEnvironmentVariable, "secret key");

            return settings;
        }

        private static string GetParameterOrFail(string parameter, string environmentVariable, string parameterName)
        {
            parameter = parameter.Empty() ? Environment.GetEnvironmentVariable(environmentVariable) : parameter;
            if (parameter.NotEmpty())
                return parameter;

            throw new ArgumentException($"Lockbox {parameterName} can not be empty!", nameof(parameter));
        }
    }
}