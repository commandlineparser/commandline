// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class InstanceBuilder
    {
        public static ParserResult<T> Build<T>(
            Maybe<Func<T>> factory,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {
            return Build(
                factory,
                (args, optionSpecs) =>
                    {
                        var tokens = Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, nameComparer));
                        var explodedTokens = Tokenizer.ExplodeOptionList(
                            tokens,
                            name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                        return explodedTokens;
                    },
                arguments,
                nameComparer,
                parsingCulture);
        }

        public static ParserResult<T> Build<T>(
            Maybe<Func<T>> factory,
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, StatePair<IEnumerable<Token>>> tokenizer,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {
            var typeInfo = factory.Return(f => f().GetType(), typeof(T));

            var specProps = typeInfo.GetSpecifications(pi => SpecificationProperty.Create(
                    Specification.FromProperty(pi), pi, Maybe.Nothing<object>()));

            var specs = from pt in specProps select pt.Specification;

            var optionSpecs = specs
                .ThrowingValidate(SpecificationGuards.Lookup)
                .OfType<OptionSpecification>();

            if (arguments.Any() && nameComparer.Equals("--help", arguments.First()))
            {
                return ParserResult.Create(
                    ParserResultType.Options,
                    factory.Return(f => f(), default(T)),
                    new[] { new HelpRequestedError() });
            }

            var tokenizerResult = tokenizer(arguments, optionSpecs);

            var tokens = tokenizerResult.Value;

            var partitions = TokenPartitioner.Partition(
                tokens,
                name => TypeLookup.FindTypeDescriptorAndSibling(name, optionSpecs, nameComparer));

            var optionSpecProps = OptionMapper.MapValues(
                (from pt in specProps where pt.Specification.IsOption() select pt),
                partitions.Options,
                (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture),
                nameComparer);

            var valueSpecProps = ValueMapper.MapValues(
                (from pt in specProps where pt.Specification.IsValue() select pt),
                    partitions.Values,
                (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture));

            var missingValueErrors = from token in partitions.Errors
                                     select new MissingValueOptionError(
                                         optionSpecs.Single(o => token.Text.MatchName(o.ShortName, o.LongName, nameComparer)).FromOptionSpecification());

            var specPropsWithValue = optionSpecProps.Value.Concat(valueSpecProps.Value);

            T instance;
            if (ReflectionHelper.IsTypeMutable(typeInfo))
            {
                instance = factory.Return(f => f(), Activator.CreateInstance<T>());
                instance = instance
                    .SetProperties(specPropsWithValue,
                        sp => sp.Value.IsJust(),
                        sp => sp.Value.FromJust())
                    .SetProperties(specPropsWithValue,
                        sp => sp.Value.IsNothing() && sp.Specification.DefaultValue.IsJust(),
                        sp => sp.Specification.DefaultValue.FromJust())
                    .SetProperties(specPropsWithValue,
                        sp => sp.Value.IsNothing()
                            && sp.Specification.TargetType == TargetType.Sequence
                            && sp.Specification.DefaultValue.MatchNothing(),
                        sp => sp.Property.PropertyType.GetGenericArguments().Single().CreateEmptyArray());
            }
            else
            {
                var t = typeof(T);
                var ctor = t.GetConstructor((from p in specProps select p.Specification.ConversionType).ToArray());
                var values = (from prms in ctor.GetParameters()
                              join sp in specPropsWithValue on prms.Name.ToLower() equals sp.Property.Name.ToLower()
                              select sp.Value.Return(v => v,
                                    sp.Specification.DefaultValue.Return(d => d,
                                        sp.Specification.ConversionType.CreateDefaultForImmutable()))).ToArray();
                instance = (T)ctor.Invoke(values);
            }

            var validationErrors = specPropsWithValue.Validate(
                SpecificationPropertyRules.Lookup(tokens));

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