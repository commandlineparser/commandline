// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class OptionMapper
    {
        public static StatePair<
            IEnumerable<SpecificationProperty>>
                MapValues(
                    IEnumerable<SpecificationProperty> propertyTuples,
                    IEnumerable<KeyValuePair<string, IEnumerable<string>>> options,
                    Func<IEnumerable<string>, System.Type, bool, Maybe<object>> converter,
                    StringComparer comparer)
        {
            var sequencesAndErrors = propertyTuples
                .Select(pt =>
                    options.SingleOrDefault(
                            s =>
                            s.Key.MatchName(((OptionSpecification)pt.Specification).ShortName, ((OptionSpecification)pt.Specification).LongName, comparer))
                               .ToMaybe()
                               .Return(sequence =>
                                    converter(sequence.Value, pt.Property.PropertyType, pt.Specification.ConversionType.IsScalar())
                                    .Return(converted =>
                                            Tuple.Create(
                                                pt.WithValue(Maybe.Just(converted)),
                                                Maybe.Nothing<Error>()),
                                            Tuple.Create<SpecificationProperty, Maybe<Error>>(
                                                pt,
                                                Maybe.Just<Error>(new BadFormatConversionError(NameInfo.FromOptionSpecification((OptionSpecification)pt.Specification))))),
                                Tuple.Create(pt, Maybe.Nothing<Error>()))
                );
            return StatePair.Create(
                sequencesAndErrors.Select(se => se.Item1),
                sequencesAndErrors.Select(se => se.Item2).OfType<Just<Error>>().Select(se => se.Value));
        }
    }
}
