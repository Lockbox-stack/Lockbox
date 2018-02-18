using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lockbox.Api.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex NameRegex = new Regex("^(?![_.-])(?!.*[_.-]{2})[a-zA-Z0-9._.-]+(?<![_.-])$",
            RegexOptions.Compiled);

        public static bool NotEmpty(this string value) => !value.Empty();
        public static bool Empty(this string value) => string.IsNullOrWhiteSpace(value);
        public static bool IsValidName(this string value) => NameRegex.IsMatch(value);

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