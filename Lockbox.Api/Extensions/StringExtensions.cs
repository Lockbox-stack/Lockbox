using System.Collections.Generic;

namespace Lockbox.Api.Extensions
{
    public static class StringExtensions
    {
        public static bool NotEmpty(this string value) => !value.Empty();
        public static bool Empty(this string value) => string.IsNullOrWhiteSpace(value);

        public static KeyValuePair<string, string> ParseAuthorzationHeader(this string authorizationHeader)
        {
            var data = authorizationHeader.Trim().Split(' ');
            if (data.Length != 2)
                return new KeyValuePair<string, string>();
            if (data[0].Empty() || data[1].Empty())
                return new KeyValuePair<string, string>();

            return new KeyValuePair<string, string>(data[0].ToLowerInvariant(), data[1]);
        }
    }
}