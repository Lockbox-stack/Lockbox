using System;

namespace Lockbox.Api.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly string DateTimeFormat = "yyyy-MM-dd H:mm:ss";

        public static string FormatToString(this DateTime value) => value.ToString(DateTimeFormat);

        public static long ToTimestamp(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var time = dateTime.Subtract(new TimeSpan(epoch.Ticks));

            return time.Ticks / 10000;
        }
    }
}