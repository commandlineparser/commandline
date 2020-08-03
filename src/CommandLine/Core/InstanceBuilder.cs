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
                .Memoize();

            var specs = from pt in specProps select pt.Specification;

            var optionSpecs = specs
                .ThrowingValidate(SpecificationGuards.Lookup)
                .OfType<OptionSpecification>()
                .Memoize();

            Func<T> makeDefault = () =>
                typeof(T).IsMutable()
                    ? factory.MapValueOrDefault(f => f(), () => Activator.CreateInstance<T>())
                    : ReflectionHelper.CreateDefaultImmutableInstance<T>(
                        (from p in specProps select p.Specification.ConversionType).ToArray());

            Func<IEnumerable<Error>, ParserResult<T>> notParsed =
                errs => new NotParsed<T>(makeDefault().GetType().ToTypeInfo(), errs);

            var argumentsList = arguments.Memoize();
            Func<ParserResult<T>> buildUp = () =>
            {
                var tokenizerResult = tokenizer(argumentsList, optionSpecs);

                var tokens = tokenizerResult.SucceededWith().Memoize();

                var partitions = TokenPartitioner.Partition(
                    tokens,
                    name => TypeLookup.FindTypeDescriptorAndSibling(name, optionSpecs, nameComparer));
                var optionsPartition = partitions.Item1.Memoize();
                var valuesPartition = partitions.Item2.Memoize();
                var errorsPartition = partitions.Item3.Memoize();

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
                    optionSpecPropsResult.SucceededWith().Concat(valueSpecPropsResult.SucceededWith()).Memoize();

                var setPropertyErrors = new List<Error>();

                //build the instance, determining if the type is mutable or not.
                T instance;
                if(typeInfo.IsMutable() == true)
                {
                    instance = BuildMutable(factory, specPropsWithValue, setPropertyErrors);
                }
                else
                {
                    instance = BuildImmutable(typeInfo, factory, specProps, specPropsWithValue, setPropertyErrors);
                }

                var validationErrors = specPropsWithValue.Validate(SpecificationPropertyRules.Lookup(tokens));

                var allErrors =
                    tokenizerResult.SuccessMessages()
                        .Concat(missingValueErrors)
                        .Concat(optionSpecPropsResult.SuccessMessages())
                        .Concat(valueSpecPropsResult.SuccessMessages())
                        .Concat(validationErrors)
                        .Concat(setPropertyErrors)
                        .Memoize();

                var warnings = from e in allErrors where nonFatalErrors.Contains(e.Tag) select e;

                return allErrors.Except(warnings).ToParserResult(instance);
            };

            var preprocessorErrors = (
                    argumentsList.Any()
                    ? arguments.Preprocess(PreprocessorGuards.Lookup(nameComparer, autoHelp, autoVersion))
                    : Enumerable.Empty<Error>()
                ).Memoize();

            var result = argumentsList.Any()
                ? preprocessorErrors.Any()
                    ? notParsed(preprocessorErrors)
                    : buildUp()
                : buildUp();

            return result;
        }

        private static T BuildMutable<T>(Maybe<Func<T>> factory, IEnumerable<SpecificationProperty> specPropsWithValue, List<Error> setPropertyErrors )
        {
            var mutable = factory.MapValueOrDefault(f => f(), () => Activator.CreateInstance<T>());

            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specPropsWithValue, 
                    sp => sp.Value.IsJust(), 
                    sp => sp.Value.FromJustOrFail()
                )
            );

            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specPropsWithValue,
                    sp => sp.Value.IsNothing() && sp.Specification.DefaultValue.IsJust(),
                    sp => sp.Specification.DefaultValue.FromJustOrFail()
                )
            );

            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specPropsWithValue,
                    sp => sp.Value.IsNothing() 
                        && sp.Specification.TargetType == TargetType.Sequence 
                        && sp.Specification.DefaultValue.MatchNothing(),
                    sp => sp.Property.PropertyType.GetTypeInfo().GetGenericArguments().Single().CreateEmptyArray()
                )
            );

            return mutable;
        }

        private static T BuildImmutable<T>(Type typeInfo, Maybe<Func<T>> factory, IEnumerable<SpecificationProperty> specProps, IEnumerable<SpecificationProperty> specPropsWithValue, List<Error> setPropertyErrors)
        {
            var ctor = typeInfo.GetTypeInfo().GetConstructor(
                specProps.Select(sp => sp.Property.PropertyType).ToArray()
            );

            if(ctor == null)
            {
                throw new InvalidOperationException($"Type {typeInfo.FullName} appears to be immutable, but no constructor found to accept values.");
            }
            try
            {
                var values =
                    (from prms in ctor.GetParameters()
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
            }
            catch (Exception)
            {
                var ctorArgs = specPropsWithValue
                    .Select(x => x.Property.Name.ToLowerInvariant()).ToArray();
                throw GetException(ctorArgs);
            }
            Exception GetException(string[] s)
            {
                var ctorSyntax = s != null ? " Constructor Parameters can be ordered as: " + $"'({string.Join(", ", s)})'" : string.Empty;
                var msg =
                    $"Type {typeInfo.FullName} appears to be Immutable with invalid constructor. Check that constructor arguments have the same name and order of their underlying Type. {ctorSyntax}";
                InvalidOperationException invalidOperationException = new InvalidOperationException(msg);
                return invalidOperationException;
            }
        }

    }
}
