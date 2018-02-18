using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lockbox.Api.Framework.Serialization
{
    public sealed class CustomJsonSerializer : JsonSerializer
    {
        private const string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        public CustomJsonSerializer()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Formatting = Formatting.Indented;
            DateFormatString = DateTimeFormat;
            Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter
            {
                AllowIntegerValues = true,
                CamelCaseText = true
            });
        }
    }
}