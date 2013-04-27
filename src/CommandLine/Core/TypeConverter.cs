// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class TypeConverter
    {
        public static Maybe<object> ChangeType(IEnumerable<string> values, Type conversionType, bool scalar, CultureInfo conversionCulture)
        {
            if (values == null) throw new ArgumentNullException("values");
            if (conversionType == null) throw new ArgumentNullException("conversionType");
            if (conversionCulture == null) throw new ArgumentNullException("conversionCulture");

            return scalar
                ? ChangeType(values.Single(), conversionType, conversionCulture)
                : ChangeType(values, conversionType, conversionCulture);
        }

        private static Maybe<object> ChangeType(IEnumerable<string> values, System.Type conversionType, CultureInfo conversionCulture)
        {
            var type =
                conversionType.GetGenericArguments()
                              .SingleOrDefault()
                              .ToMaybe()
                              .FromJust(
                                  new ApplicationException("Non scalar properties should be sequence of type IEnumerable<T>."));

            var converted = values.Select(value => ChangeType(value, type, conversionCulture));

            return converted.Any(a => a.MatchNothing())
                ? Maybe.Nothing<object>()
                : Maybe.Just(converted.Select(c => ((Just<object>)c).Value).ToArray(type));
        }

        private static Maybe<object> ChangeType(string value, Type conversionType, CultureInfo conversionCulture)
        {
            try
            {
                return Maybe.Just(
                    MatchBoolString(value)
                        ? ConvertBoolString(value)
                        : conversionType.IsEnum
                            ? Enum.Parse(conversionType, value)
                            : Convert.ChangeType(value, conversionType, conversionCulture));
            }
            catch (InvalidCastException)
            {
                return Maybe.Nothing<object>();
            }
            catch (FormatException)
            {
                return Maybe.Nothing<object>();
            }
            catch (OverflowException)
            {
                return Maybe.Nothing<object>();
            }
        }

        private static bool MatchBoolString(string value)
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase)
                   || value.Equals("false", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ConvertBoolString(string value)
        {
            return value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}