using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
