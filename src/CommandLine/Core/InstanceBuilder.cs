// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class InstanceBuilder
    {
        public static ParserResult<T> Build<T>(
            Func<T> factory,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {
            return InstanceBuilder.Build(
                factory,
                (args, optionSpecs) =>
                    Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, nameComparer)),
                arguments,
                nameComparer,
                parsingCulture);
        }

        public static ParserResult<T> Build<T>(
            Func<T> factory,
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, StatePair<IEnumerable<Token>>> tokenizer,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {
            var instance = factory();

            if (arguments.Any() && nameComparer.Equals("--help", arguments.First()))
            {
                return ParserResult.Create(
                    ParserResultType.Options,
                    instance,
                    new[] { new HelpRequestedError() });
            }

            var specProps = instance.GetType().GetSpecifications(pi => SpecificationProperty.Create(
                    Specification.FromProperty(pi), pi, Maybe.Nothing<object>()));

            var optionSpecs = (from pt in specProps select pt.Specification)
                .ThrowingValidate(SpecificationGuards.Lookup)
                .OfType<OptionSpecification>();

            var tokenizerResult = tokenizer(arguments, optionSpecs);

            var tokens = tokenizerResult.Value;

            var partitions = TokenPartitioner.Partition(
                tokens,
                name => TypeLookup.GetDescriptorInfo(name, optionSpecs, nameComparer));

            var optionSpecProps = OptionMapper.MapValues(
                (from pt in specProps where pt.Specification.IsOption() select pt),
                partitions.Item1,
                (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture),
                nameComparer);

            var valueSpecProps = ValueMapper.MapValues(
                (from pt in specProps where pt.Specification.IsValue() select pt),
                    partitions.Item2,
                (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture));

            var missingValueErrors = from token in partitions.Item3
                                     select new MissingValueOptionError(
                                         NameInfo.FromOptionSpecification(optionSpecs.Single(o => token.Text.MatchName(o.ShortName, o.LongName, nameComparer))));

            var specPropsWithValue = optionSpecProps.Value.Concat(valueSpecProps.Value);

            instance = instance
                .SetProperties(specPropsWithValue,
                    sp => sp.Value.IsJust(),
                    sp => sp.Value.FromJust())
                .SetProperties(specPropsWithValue,
                    sp => sp.Value.IsNothing() && sp.Specification.DefaultValue.IsJust(),
                    sp => sp.Specification.DefaultValue.FromJust())
                .SetProperties(specPropsWithValue,
                    sp => sp.Value.IsNothing()
                        && sp.Specification.ConversionType.ToDescriptor() == DescriptorType.Sequence
                        && sp.Specification.DefaultValue.MatchNothing(),
                    sp => sp.Property.PropertyType.GetGenericArguments().Single().CreateEmptyArray());

            var validationErrors = specPropsWithValue.Validate(SpecificationPropertyRules.Lookup)
                .OfType<Just<Error>>().Select(e => e.Value);

            return ParserResult.Create(
                ParserResultType.Options,
                instance,
                tokenizerResult.Errors
                    .Concat(missingValueErrors)
                    .Concat(optionSpecProps.Errors)
                    .Concat(valueSpecProps.Errors)
                    .Concat(validationErrors));
        }
    }
}