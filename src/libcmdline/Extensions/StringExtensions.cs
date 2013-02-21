#region License
// <copyright file="StringExtensions.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

namespace CommandLine.Extensions
{
    #region Using Directives
    using System;
    using System.Globalization;
    #endregion

    /// <summary>
    /// Utility extension methods for System.String.
    /// </summary>
    internal static class StringExtensions
    {
        public static string Spaces(this int value)
        {
            return new string(' ', value);
        }

        public static bool IsNumeric(this string value)
        {
            decimal temporary;
            return decimal.TryParse(value, out temporary);
        }

        public static string FormatInvariant(this string value, params object[] arguments)
        {
            return string.Format(CultureInfo.InvariantCulture, value, arguments);
        }

        public static string FormatLocal(this string value, params object[] arguments)
        {
            return string.Format(CultureInfo.CurrentCulture, value, arguments);
        }

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

        public static StringComparison GetStringComparison(this IParserSettings settings)
        {
            return settings.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }
    }
}