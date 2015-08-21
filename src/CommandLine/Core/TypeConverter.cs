// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Core
{
    static class TypeConverter
    {
        public static Maybe<object> ChangeType(IEnumerable<string> values, Type conversionType, bool scalar, CultureInfo conversionCulture)
        {
            return scalar
                ? ChangeTypeScalar(values.Single(), conversionType, conversionCulture)
                : ChangeTypeSequence(values, conversionType, conversionCulture);
        }

        private static Maybe<object> ChangeTypeSequence(IEnumerable<string> values, Type conversionType, CultureInfo conversionCulture)
        {
            var type =
                conversionType.GetGenericArguments()
                              .SingleOrDefault()
                              .ToMaybe()
                              .FromJustOrFail(
                                  new ApplicationException("Non scalar properties should be sequence of type IEnumerable<T>."));

            var converted = values.Select(value => ChangeTypeScalar(value, type, conversionCulture));

            return converted.Any(a => a.MatchNothing())
                ? Maybe.Nothing<object>()
                : Maybe.Just(converted.Select(c => ((Just<object>)c).Value).ToUntypedArray(type));
        }

        private static Maybe<object> ChangeTypeScalar(string value, Type conversionType, CultureInfo conversionCulture)
        {
            var result = ChangeTypeScalarImpl(value, conversionType, conversionCulture);
            result.Match((_,__) => { }, e => e.First().RethrowWhenAbsentIn(
                new[] { typeof(InvalidCastException), typeof(FormatException), typeof(OverflowException) }));
            return result.ToMaybe();
        }

        private static Result<object, Exception> ChangeTypeScalarImpl(string value, Type conversionType, CultureInfo conversionCulture)
        {
            Func<object> changeType = () =>
            {
                Func<object> safeChangeType = () =>
                {
                    var isFsOption = ReflectionHelper.IsFSharpOptionType(conversionType);

                    Func<Type> getUnderlyingType =
                        () =>
                            isFsOption
                                ? FSharpOptionHelper.GetUnderlyingType(conversionType)
                                : Nullable.GetUnderlyingType(conversionType);

                    var type = getUnderlyingType() ?? conversionType;

                    Func<object> withValue =
                        () =>
                            isFsOption
                                ? FSharpOptionHelper.Some(type, Convert.ChangeType(value, type, conversionCulture))
                                : Convert.ChangeType(value, type, conversionCulture);

                    Func<object> empty = () => isFsOption ? FSharpOptionHelper.None(type) : null;

                    return (value == null) ? empty() : withValue();
                };

                return value.IsBooleanString()
                    ? value.ToBoolean() : conversionType.IsEnum
                        ? value.ToEnum(conversionType) : safeChangeType();
            };

            Func<object> makeType = () =>
            {
                try
                {
                    var ctor = conversionType.GetConstructor(new[] { typeof(string) });
                    return ctor.Invoke(new object[] { value });
                }
                catch (Exception)
                {
                    throw new FormatException("Destination conversion type must have a constructor that accepts a string.");
                }
            };

            return Result.Try(
                conversionType.IsPrimitiveEx() || ReflectionHelper.IsFSharpOptionType(conversionType)
                    ? changeType
                    : makeType);
        }

        private static object ToEnum(this string value, Type conversionType)
        {
            object parsedValue;
            try
            {
                parsedValue = Enum.Parse(conversionType, value);
            }
            catch (ArgumentException)
            {
                throw new FormatException();
            }
            if (Enum.IsDefined(conversionType, parsedValue))
            {
                return parsedValue;
            }
            throw new FormatException();
        }
    }
}