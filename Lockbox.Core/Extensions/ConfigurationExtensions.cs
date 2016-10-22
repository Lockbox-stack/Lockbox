using System;
using Microsoft.Extensions.Configuration;

namespace Lockbox.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetSettings<T>(this IConfiguration configuration, string section = "") where T : new()
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                section = typeof(T).Name.Replace("Settings", string.Empty).Replace("Configuration", string.Empty);
            }

            var configurationValue = new T();
            configuration.GetSection(section).Bind(configurationValue);

            return configurationValue;
        }

        public static object GetSettings(this IConfiguration configuration, Type type, string section = "")
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                section = type.Name.Replace("Settings", string.Empty).Replace("Configuration", string.Empty);
            }

            var configurationValue = Activator.CreateInstance(type);
            configuration.GetSection(section).Bind(configurationValue);

            return configurationValue;
        }
    }
}