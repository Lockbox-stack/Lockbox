using System;

namespace Lockbox.Api.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        public static string FormatToString(this DateTime value) => value.ToString(DateTimeFormat);
    }
}