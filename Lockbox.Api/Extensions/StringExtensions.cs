namespace Lockbox.Api.Extensions
{
    public static class StringExtensions
    {
        public static bool NotEmpty(this string value) => !value.Empty();
        public static bool Empty(this string value) => string.IsNullOrWhiteSpace(value);

    }
}