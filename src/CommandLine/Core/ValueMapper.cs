// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Core
{
    static class ValueMapper
    {
        public static Result<
            IEnumerable<SpecificationProperty>, Error>
            MapValues(
                IEnumerable<SpecificationProperty> specProps,
                IEnumerable<string> values,
                Func<IEnumerable<string>, Type, bool, Maybe<object>> converter)
        {
            var propAndErrors = MapValuesImpl(specProps, values, converter);

            return Result.Succeed(
                propAndErrors.Select(pe => pe.Item1),
                propAndErrors.Select(pe => pe.Item2)
                    .OfType<Just<Error>>().Select(e => e.Value)
                );
        }

        private static IEnumerable<Tuple<SpecificationProperty, Maybe<Error>>> MapValuesImpl(
            IEnumerable<SpecificationProperty> specProps,
            IEnumerable<string> values,
            Func<IEnumerable<string>, Type, bool, Maybe<object>> converter)
        {
            if (specProps.Empty())
            {
                yield break;
            }
            var pt = specProps.First();
            var taken = values.Take(pt.Specification.CountOfMaxNumberOfValues().GetValueOrDefault(values.Count()));
            if (taken.Empty())
            {
                yield return
                    Tuple.Create(pt, pt.Specification.MakeErrorInCaseOfMinConstraint());
                yield break;
            }

            var next = specProps.Skip(1).FirstOrDefault(s => s.Specification.IsValue()).ToMaybe();
            if (pt.Specification.Max.IsJust()
                && next.IsNothing()
                && values.Skip(taken.Count()).Any())
            {
                yield return
                    Tuple.Create<SpecificationProperty, Maybe<Error>>(
                        pt, Maybe.Just<Error>(new SequenceOutOfRangeError(NameInfo.EmptyName)));
                yield break;
            }

            yield return
                converter(taken, pt.Property.PropertyType, pt.Specification.TargetType != TargetType.Sequence)
                    .MapValueOrDefault(
                        converted => Tuple.Create(pt.WithValue(Maybe.Just(converted)), Maybe.Nothing<Error>()),
                        Tuple.Create<SpecificationProperty, Maybe<Error>>(
                            pt, Maybe.Just<Error>(new BadFormatConversionError(NameInfo.EmptyName))));
         
            foreach (var value in MapValuesImpl(specProps.Skip(1), values.Skip(taken.Count()), converter))
            {
                yield return value;
            }
        }

        private static Maybe<int> CountOfMaxNumberOfValues(this Specification specification)
        {
            switch (specification.TargetType)
            {
                case TargetType.Scalar:
                    return Maybe.Just(1);
                case TargetType.Sequence:
                    if (specification.Max.IsJust())
                    {
                        return Maybe.Just(specification.Max.FromJustOrFail());
                    }
                    break;
            }
            return Maybe.Nothing<int>();
        }

        private static Maybe<Error> MakeErrorInCaseOfMinConstraint(this Specification specification)
        {
            return specification.Min.IsJust()
                ? Maybe.Just<Error>(new SequenceOutOfRangeError(NameInfo.EmptyName))
                : Maybe.Nothing<Error>();
        }
    }
}