using System;

namespace Lockbox.Api.Framework.Serialization
{
    internal static class Helpers
    {
        /// <summary>
        /// Attempts to detect if the content type is JSON.
        /// Supports:
        ///   application/json
        ///   text/json
        ///   application/vnd[something]+json
        /// Matches are case insentitive to try and be as "accepting" as possible.
        /// </summary>
        /// <param name="contentType">Request content type</param>
        /// <returns>True if content type is JSON, false otherwise</returns>
        public static bool IsJsonType(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals("application/json", StringComparison.CurrentCultureIgnoreCase) ||
                   contentMimeType.Equals("text/json", StringComparison.CurrentCultureIgnoreCase) ||
                  (contentMimeType.StartsWith("application/vnd", StringComparison.CurrentCultureIgnoreCase) &&
                   contentMimeType.EndsWith("+json", StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
