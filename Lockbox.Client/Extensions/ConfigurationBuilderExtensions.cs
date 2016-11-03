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
        private static readonly string EncryptionKeyEnvironmentVariable = "LOCKBOX_ENCRYPTION_KEY";
        private static readonly string ApiUrlEnvironmentVariable = "LOCKBOX_API_URL";
        private static readonly string ApiKeyEnvironmentVariable = "LOCKBOX_API_KEY";
        private static readonly string BoxNameEnvironmentVariable = "LOCKBOX_BOX_NAME";
        private static readonly string EntryKeyEnvironmentVariable = "LOCKBOX_ENTRY_KEY";

        public static IConfigurationBuilder AddLockbox(this IConfigurationBuilder builder,
            string encryptionKey = null, 
            string apiUrl = null,
            string apiKey = null,
            string boxName = null,
            string entryKey = null)
        {
            encryptionKey = GetParameterOrFail(encryptionKey, EncryptionKeyEnvironmentVariable, "encryption key");
            apiUrl = GetParameterOrFail(apiUrl, ApiUrlEnvironmentVariable, "API key");
            apiKey = GetParameterOrFail(apiKey, ApiKeyEnvironmentVariable, "API url");
            boxName = GetParameterOrFail(boxName, BoxNameEnvironmentVariable, "box name");
            entryKey = GetParameterOrFail(entryKey, EntryKeyEnvironmentVariable, "entry key");

            var lockboxClient = new LockboxEntryClient(encryptionKey, apiUrl, apiKey);
            var entryDictionary = lockboxClient.GetEntryAsDictionaryAsync(boxName, entryKey).Result;
            if (entryDictionary == null)
            {
                throw new ArgumentException($"Lockbox entry has not been found for key: '{entryKey}'.", nameof(entryKey));
            }

            var data = ParseobjectMappings(entryDictionary);
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

        private static IEnumerable<KeyValuePair<string, string>> ParseobjectMappings(
            IDictionary<string, object> entryDictionary)
        {
            IEnumerable<KeyValuePair<string, string>> data = null;
            try
            {
                data = GetComplexObjectMappings(entryDictionary);
            }
            catch
            {
            }

            return data ?? GetFlatObjectMappings(entryDictionary);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetComplexObjectMappings(
                IDictionary<string, object> entryDictionary)
            => from entryPair in entryDictionary
            from entry in entryPair.Value as IEnumerable<object>
            let property = (JProperty) entry
            select new KeyValuePair<string, string>($"{entryPair.Key}:{property.Name}", property.Value.ToString());

        private static IEnumerable<KeyValuePair<string, string>> GetFlatObjectMappings(
                IDictionary<string, object> entryDictionary)
            => from entryPair in entryDictionary
            select new KeyValuePair<string, string>($"{entryPair.Key}", $"{entryPair.Value}");
    }
}
