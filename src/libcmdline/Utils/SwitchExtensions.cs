namespace CommandLine.Internal
{
    static class OptionExtensions
    {
        public static string ToOption(this string value)
        {
            return string.Concat("--", value);
        }

        public static string ToOption(this char? value)
        {
            return string.Concat("-", value);
        }

        public static bool IsDash(this string value)
        {
            return string.CompareOrdinal(value, "-") == 0;
        }

        public static bool IsShortOption(this string value)
        {
            return value[0] == '-';
        }

        public static bool IsLongOption(this string value)
        {
            return value[0] == '-' && value[1] == '-';
        }
    }
}
