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
            string recordKey)
        {
            var lockboxClient = new LockboxClient(apiUrl, apiKey);
            var record = lockboxClient.GetComplexRecordAsync(recordKey).Result;
            if (record == null)
            {
                throw new ArgumentException($"Lockbox record has not been found for key: '{recordKey}'.", nameof(recordKey));
            }

            var data = from recordEntry in record
                from entry in recordEntry.Value as IEnumerable<object>
                let property = (JProperty) entry
                select new KeyValuePair<string, string>($"{recordEntry.Key}:{property.Name}", property.Value.ToString());

            var source = new MemoryConfigurationSource {InitialData = data};
            builder.Add(source);

            return builder;
        }
    }
}