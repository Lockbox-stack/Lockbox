using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Newtonsoft.Json.Linq;

namespace Lockbox.Client.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        private static readonly string ApiUrlEnvironmentVariable = "LOCKBOX_API_URL";
        private static readonly string ApiKeyEnvironmentVariable = "LOCKBOX_API_KEY";
        private static readonly string EntryKeyEnvironmentVariable = "LOCKBOX_ENTRY_KEY";

        public static IConfigurationBuilder AddLockbox(this IConfigurationBuilder builder,
            string apiUrl = null, string apiKey = null, string entryKey = null)
        {
            apiUrl = GetParameterOrFail(apiKey, ApiUrlEnvironmentVariable, "API key");
            apiKey = GetParameterOrFail(apiKey, ApiKeyEnvironmentVariable, "API url");
            entryKey = GetParameterOrFail(apiKey, EntryKeyEnvironmentVariable, "entry key");
            var lockboxClient = new LockboxEntryClient(apiUrl, apiKey);
            var entryDictionary = lockboxClient.GetEntryAsDictionaryAsync(entryKey).Result;
            if (entryDictionary == null)
            {
                throw new ArgumentException($"Lockbox entry has not been found for key: '{entryKey}'.", nameof(entryKey));
            }

            var data = from entryPair in entryDictionary
                from entry in entryPair.Value as IEnumerable<object>
                let property = (JProperty) entry
                select new KeyValuePair<string, string>($"{entryPair.Key}:{property.Name}", property.Value.ToString());

            var source = new MemoryConfigurationSource {InitialData = data};
            builder.Add(source);

            return builder;
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