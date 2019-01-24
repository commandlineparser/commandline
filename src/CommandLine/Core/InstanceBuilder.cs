// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;
using System.Reflection;

namespace CommandLine.Core
{
    static class InstanceBuilder
    {
        public static ParserResult<T> Build<T>(
            Maybe<Func<T>> factory,
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            var typeInfo = factory.MapValueOrDefault(f => f().GetType(), typeof(T));

            var specProps = typeInfo.GetSpecifications(pi => SpecificationProperty.Create(
                    Specification.FromProperty(pi), pi, Maybe.Nothing<object>()))
                .Memorize();

            var specs = from pt in specProps select pt.Specification;

            var optionSpecs = specs
                .ThrowingValidate(SpecificationGuards.Lookup)
                .OfType<OptionSpecification>()
                .Memorize();

            Func<T> makeDefault = () =>
                typeof(T).IsMutable()
                    ? factory.MapValueOrDefault(f => f(), Activator.CreateInstance<T>())
                    : ReflectionHelper.CreateDefaultImmutableInstance<T>(
                        (from p in specProps select p.Specification.ConversionType).ToArray());

            Func<IEnumerable<Error>, ParserResult<T>> notParsed =
                errs => new NotParsed<T>(makeDefault().GetType().ToTypeInfo(), errs);

            var argumentsList = arguments.Memorize();
            Func<ParserResult<T>> buildUp = () =>
            {
                var tokenizerResult = tokenizer(argumentsList, optionSpecs);

                var tokens = tokenizerResult.SucceededWith().Memorize();

                var partitions = TokenPartitioner.Partition(
                    tokens,
                    name => TypeLookup.FindTypeDescriptorAndSibling(name, optionSpecs, nameComparer));
                var optionsPartition = partitions.Item1.Memorize();
                var valuesPartition = partitions.Item2.Memorize();
                var errorsPartition = partitions.Item3.Memorize();

                var optionSpecPropsResult =
                    OptionMapper.MapValues(
                        (from pt in specProps where pt.Specification.IsOption() select pt),
                        optionsPartition,
                        (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture, ignoreValueCase),
                        nameComparer);

                var valueSpecPropsResult =
                    ValueMapper.MapValues(
                        (from pt in specProps where pt.Specification.IsValue() orderby ((ValueSpecification)pt.Specification).Index select pt),
                        valuesPartition,    
                        (vals, type, isScalar) => TypeConverter.ChangeType(vals, type, isScalar, parsingCulture, ignoreValueCase));

                var missingValueErrors = from token in errorsPartition
                                         select
                        new MissingValueOptionError(
                            optionSpecs.Single(o => token.Text.MatchName(o.ShortName, o.LongName, nameComparer))
                                .FromOptionSpecification());

                var specPropsWithValue =
                    optionSpecPropsResult.SucceededWith().Concat(valueSpecPropsResult.SucceededWith()).Memorize();

                var setPropertyErrors = new List<Error>();                

                Func <T> buildMutable = () =>
                {
                    var mutable = factory.MapValueOrDefault(f => f(), Activator.CreateInstance<T>());
                    setPropertyErrors.AddRange(mutable.SetProperties(specPropsWithValue, sp => sp.Value.IsJust(), sp => sp.Value.FromJustOrFail()));
                    setPropertyErrors.AddRange(mutable.SetProperties(
                                                 specPropsWithValue,
                                                 sp => sp.Value.IsNothing() && sp.Specification.DefaultValue.IsJust(),
                                                 sp => sp.Specification.DefaultValue.FromJustOrFail()));
                    setPropertyErrors.AddRange(mutable.SetProperties(
                                                 specPropsWithValue,
                                                 sp =>
                                                     sp.Value.IsNothing() && sp.Specification.TargetType == TargetType.Sequence
                                                     && sp.Specification.DefaultValue.MatchNothing(),
                                                 sp => sp.Property.PropertyType.GetTypeInfo().GetGenericArguments().Single().CreateEmptyArray()));
                     return mutable;
                };

                Func<T> buildImmutable = () =>
                {
                    var ctor = typeInfo.GetTypeInfo().GetConstructor((from sp in specProps select sp.Property.PropertyType).ToArray());
                    var values = (from prms in ctor.GetParameters()
                        join sp in specPropsWithValue on prms.Name.ToLower() equals sp.Property.Name.ToLower() into spv
                        from sp in spv.DefaultIfEmpty()
                        select
                          sp == null
                            ? specProps.First(s => String.Equals(s.Property.Name, prms.Name, StringComparison.CurrentCultureIgnoreCase))
                              .Property.PropertyType.GetDefaultValue()
                            : sp.Value.GetValueOrDefault(
                                sp.Specification.DefaultValue.GetValueOrDefault(
                                    sp.Specification.ConversionType.CreateDefaultForImmutable()))).ToArray();
                    var immutable = (T)ctor.Invoke(values);
                    return immutable;
                };

                var instance = typeInfo.IsMutable() ? buildMutable() : buildImmutable();
                
                var validationErrors = specPropsWithValue.Validate(SpecificationPropertyRules.Lookup(tokens));

                var allErrors =
                    tokenizerResult.SuccessfulMessages()
                        .Concat(missingValueErrors)
                        .Concat(optionSpecPropsResult.SuccessfulMessages())
                        .Concat(valueSpecPropsResult.SuccessfulMessages())
                        .Concat(validationErrors)
                        .Concat(setPropertyErrors)
                        .Memorize();

                var warnings = from e in allErrors where nonFatalErrors.Contains(e.Tag) select e;

                return allErrors.Except(warnings).ToParserResult(instance);
            };

            var preprocessorErrors = (
                    argumentsList.Any()
                    ? arguments.Preprocess(PreprocessorGuards.Lookup(nameComparer, autoHelp, autoVersion))
                    : Enumerable.Empty<Error>()
                ).Memorize();

            var result = argumentsList.Any()
                ? preprocessorErrors.Any()
                    ? notParsed(preprocessorErrors)
                    : buildUp()
                : buildUp();

            return result;
        }
    }
}