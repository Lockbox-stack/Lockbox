using System;
using Lockbox.Client.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Newtonsoft.Json.Linq;

namespace Lockbox.Client.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        private static readonly JsonParser JsonParser = new JsonParser();
        private static readonly string EncryptionKeyEnvironmentVariable = "LOCKBOX_ENCRYPTION_KEY";
        private static readonly string ApiUrlEnvironmentVariable = "LOCKBOX_API_URL";
        private static readonly string ApiKeyEnvironmentVariable = "LOCKBOX_API_KEY";
        private static readonly string BoxNameEnvironmentVariable = "LOCKBOX_BOX_NAME";
        private static readonly string EntryKeyEnvironmentVariable = "LOCKBOX_ENTRY_KEY";

        /// <summary>
        /// Integrates Lockbox as a configuration source for the application settings.
        /// Specified configuration will be fetched via HTTP request and added to the builder pipeline.
        /// </summary>
        /// <param name="builder">Instance of IConfigurationBuilder</param>
        /// <param name="encryptionKey">Encryption key used for encrypting values. If omitted then "LOCKBOX_ENCRYPTION_KEY" environment variable will be used instead.</param>
        /// <param name="apiUrl">URL of the Lockbox API. If omitted then "LOCKBOX_API_URL" environment variable will be used instead.</param>
        /// <param name="apiKey">API key that will be used for authenticating HTTP requests to the Lockbox API. If omitted then "LOCKBOX_API_KEY" environment variable will be used instead.</param>
        /// <param name="boxName">Name of the box that contains specified entry defined below. If omitted then "LOCKBOX_BOX_NAME" environment variable will be used instead.</param>
        /// <param name="entryKey">Name of the entry within a box that will be used for getting configuration values. If omitted then "LOCKBOX_ENTRY_KEY" environment variable will be used instead.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
            var entryDictionary = lockboxClient.GetEntryAsync(boxName, entryKey).Result;
            if (entryDictionary == null)
            {
                throw new ArgumentException($"Lockbox entry has not been found for key: '{entryKey}'.", nameof(entryKey));
            }

            var data = JsonParser.Parse((JObject)entryDictionary);
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
