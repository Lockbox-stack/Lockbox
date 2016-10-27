using System;
using Lockbox.Api.MongoDb;
using Microsoft.Extensions.Configuration;

namespace Lockbox.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        private static readonly string SecretKeyEnvironmentVariable = "LOCKBOX_SECRET_KEY";
        private static readonly string MongoConnectionStringEnvironmentVariable = "LOCKBOX_MONGO_CONNECTION_STRING";
        private static readonly string MongoDatabaseEnvironmentVariable = "LOCKBOX_MONGO_DATABASE";

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

            if (type == typeof(LockboxSettings))
                return InitializeLockboxSettings(configurationValue as LockboxSettings);
            if (type == typeof(MongoDbSettings))
                return InitializeMongoDbSettings(configurationValue as MongoDbSettings);

            return configurationValue;
        }

        private static LockboxSettings InitializeLockboxSettings(LockboxSettings settings)
        {
            settings.SecretKey = GetParameterOrFail(settings.SecretKey,
                SecretKeyEnvironmentVariable, "secret key");

            return settings;
        }

        private static MongoDbSettings InitializeMongoDbSettings(MongoDbSettings settings)
        {
            settings.ConnectionString = GetParameterOrFail(settings.ConnectionString,
                MongoConnectionStringEnvironmentVariable, "connection string");
            settings.Database = GetParameterOrFail(settings.Database,
                MongoDatabaseEnvironmentVariable, "database");

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