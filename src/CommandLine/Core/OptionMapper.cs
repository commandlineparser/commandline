// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Core
{
    static class OptionMapper
    {
        public static Result<
                IEnumerable<SpecificationProperty>, Error>
            MapValues(
                IEnumerable<SpecificationProperty> propertyTuples,
                IEnumerable<KeyValuePair<string, IEnumerable<string>>> options,
                Func<IEnumerable<string>, Type, bool, Maybe<object>> converter,
                StringComparer comparer)
        {
            var sequencesAndErrors = propertyTuples
                .Select(
                    pt =>
                    {
                        var matched = options.FirstOrDefault(s =>
                            s.Key.MatchName(((OptionSpecification)pt.Specification).ShortName, ((OptionSpecification)pt.Specification).LongName, comparer)).ToMaybe();
                        return matched.IsJust()
                            ? (
                                from sequence in matched
                                from converted in
                                    converter(
                                        sequence.Value,
                                        pt.Property.PropertyType,
                                        pt.Specification.TargetType != TargetType.Sequence)
                                select Tuple.Create(
                                    pt.WithValue(Maybe.Just(converted)), Maybe.Nothing<Error>())
                               )
                                .GetValueOrDefault(
                                    Tuple.Create<SpecificationProperty, Maybe<Error>>(
                                        pt,
                                        Maybe.Just<Error>(
                                            new BadFormatConversionError(
                                                ((OptionSpecification)pt.Specification).FromOptionSpecification()))))
                            : Tuple.Create(pt, Maybe.Nothing<Error>());
                    }
                );
            return Result.Succeed(
                sequencesAndErrors.Select(se => se.Item1),
                sequencesAndErrors.Select(se => se.Item2).OfType<Just<Error>>().Select(se => se.Value));
        }
}
}
