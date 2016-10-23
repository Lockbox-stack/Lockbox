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
        public static IConfigurationBuilder AddLockbox(this IConfigurationBuilder builder, string apiUrl, string apiKey,
            string entryKey)
        {
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
    }
}