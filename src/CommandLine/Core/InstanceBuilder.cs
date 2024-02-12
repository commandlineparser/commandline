// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;
using System.Reflection;

namespace CommandLine.Core
{
    internal static class InstanceBuilder
    {
        public static ParserResult<T> Build<
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                    DynamicallyAccessedMemberTypes.Interfaces |
                    DynamicallyAccessedMemberTypes.PublicConstructors |
                    DynamicallyAccessedMemberTypes.PublicMethods |
                    DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
                T>(
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
            return Build(
                factory,
                tokenizer,
                arguments,
                nameComparer,
                ignoreValueCase,
                parsingCulture,
                autoHelp,
                autoVersion,
                false,
                nonFatalErrors);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing type annotation", "IL2067")]
        [UnconditionalSuppressMessage("Missing type annotation", "IL2072")]
#endif
        public static ParserResult<T> Build<
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(
                    DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                    DynamicallyAccessedMemberTypes.Interfaces |
                    DynamicallyAccessedMemberTypes.PublicConstructors |
                    DynamicallyAccessedMemberTypes.PublicMethods |
                    DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
                T>(
            Maybe<Func<T>> factory,
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            bool allowMultiInstance,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            var typeInfo = GetTypeInfo();

            var specProps = typeInfo.GetSpecifications(pi => SpecificationProperty.Create(
                    Specification.FromProperty(pi), pi, Maybe.Nothing<object>()))
                .Memoize();

            var specificationProperties = specProps as SpecificationProperty[] ?? specProps.ToArray();
            var specs = from pt in specificationProperties select pt.Specification;

            var optionSpecs = specs
                .ThrowingValidate(SpecificationGuards.Lookup)
                .OfType<OptionSpecification>()
                .Memoize();

            Func<IEnumerable<Error>, ParserResult<T>> notParsed =
                errs => new NotParsed<T>(MakeDefault().GetType().ToTypeInfo(), errs);

            var enumerable = arguments as string[] ?? arguments.ToArray();
            var argumentsList = enumerable.Memoize().ToArray();

            var preprocessorErrors = (
                argumentsList.Length != 0
                    ? enumerable.Preprocess(PreprocessorGuards.Lookup(nameComparer, autoHelp, autoVersion))
                    : Enumerable.Empty<Error>()
            ).Memoize();

            var errors = preprocessorErrors as Error[] ?? preprocessorErrors.ToArray();
            var result = argumentsList.Length != 0
                ? errors.Length != 0
                    ? notParsed(errors)
                    : BuildUp()
                : BuildUp();

            return result;

            ParserResult<T> BuildUp()
            {
                var optionSpecifications = optionSpecs as OptionSpecification[] ?? optionSpecs.ToArray();
                var tokenizerResult = tokenizer(argumentsList, optionSpecifications);

                var tokens = tokenizerResult.SucceededWith().Memoize();

                var partitions = TokenPartitioner.Partition(tokens,
                    name => TypeLookup.FindTypeDescriptorAndSibling(name, optionSpecifications, nameComparer));
                var optionsPartition = partitions.Item1.Memoize();
                var valuesPartition = partitions.Item2.Memoize();
                var errorsPartition = partitions.Item3.Memoize();

                var optionSpecPropsResult = OptionMapper.MapValues(
                    (from pt in specificationProperties where pt.Specification.IsOption() select pt), optionsPartition,
                    (vals, type, isScalar, isFlag) =>
                        TypeConverter.ChangeType(vals, type, isScalar, isFlag, parsingCulture, ignoreValueCase),
                    nameComparer);

                var valueSpecPropsResult = ValueMapper.MapValues(
                    (from pt in specificationProperties
                     where pt.Specification.IsValue()
                     orderby ((ValueSpecification)pt.Specification).Index
                     select pt), valuesPartition,
                    (vals, type, isScalar) =>
                        TypeConverter.ChangeType(vals, type, isScalar, false, parsingCulture, ignoreValueCase));

                var missingValueErrors = from token in errorsPartition
                                         select new MissingValueOptionError(optionSpecifications
                                             .Single(o => token.Text.MatchName(o.ShortName, o.LongName, nameComparer))
                                             .FromOptionSpecification());

                var specPropsWithValue = optionSpecPropsResult.SucceededWith()
                    .Concat(valueSpecPropsResult.SucceededWith()).Memoize();

                var setPropertyErrors = new List<Error>();

                //build the instance, determining if the type is mutable or not.
                var propsWithValue = specPropsWithValue as SpecificationProperty[] ?? specPropsWithValue.ToArray();
                var instance = typeInfo.IsMutable() == true
                    ? BuildMutable(factory, propsWithValue, setPropertyErrors)
                    : BuildImmutable(typeInfo, factory, specificationProperties, propsWithValue, setPropertyErrors);

                var validationErrors =
                    propsWithValue.Validate(SpecificationPropertyRules.Lookup(tokens, allowMultiInstance));

                var allErrors = tokenizerResult.SuccessMessages()
                    .Concat(missingValueErrors)
                    .Concat(optionSpecPropsResult.SuccessMessages())
                    .Concat(valueSpecPropsResult.SuccessMessages())
                    .Concat(validationErrors)
                    .Concat(setPropertyErrors)
                    .Memoize();

                var warnings = from e in allErrors where nonFatalErrors.Contains(e.Tag) select e;

                return allErrors.Except(warnings).ToParserResult(instance);
            }

            T MakeDefault() =>
                typeof(T).IsMutable()
                    ? factory.MapValueOrDefault(f => f(), () => Activator.CreateInstance<T>())
                    : ReflectionHelper.CreateDefaultImmutableInstance<T>(
                        (from p in specificationProperties select p.Specification.ConversionType).ToArray());

#if NET8_0_OR_GREATER
            [UnconditionalSuppressMessage("Missing type annotation", "IL2111")]
            [UnconditionalSuppressMessage("Missing type annotation", "IL2026")]
            [UnconditionalSuppressMessage("Missing type annotation", "IL2073")]
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            Type GetTypeInfo()
            {
                return factory.MapValueOrDefault(f => f().GetType(), typeof(T));
            }
        }

        private static T BuildMutable<
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
                T>(
            Maybe<Func<T>> factory,
            IEnumerable<SpecificationProperty> specPropsWithValue,
            List<Error> setPropertyErrors)
        {
            var mutable = factory.MapValueOrDefault(f => f(), Activator.CreateInstance<T>);

            var specificationProperties = specPropsWithValue as SpecificationProperty[] ?? specPropsWithValue.ToArray();
            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specificationProperties,
                    sp => sp.Value.IsJust(),
                    sp => sp.Value.FromJustOrFail()
                )
            );

            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specificationProperties,
                    sp => sp.Value.IsNothing() && sp.Specification.DefaultValue.IsJust(),
                    sp => sp.Specification.DefaultValue.FromJustOrFail()
                )
            );

            setPropertyErrors.AddRange(
                mutable.SetProperties(
                    specificationProperties,
                    sp => sp.Value.IsNothing()
                     && sp.Specification.TargetType == TargetType.Sequence
                     && sp.Specification.DefaultValue.MatchNothing(),
                    sp => sp.Property.PropertyType.GetTypeInfo().GetGenericArguments().Single().CreateEmptyArray()
                )
            );

            return mutable;
        }

        private static T BuildImmutable<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
            Type typeInfo,
            Maybe<Func<T>> factory,
            IEnumerable<SpecificationProperty> specProps,
            IEnumerable<SpecificationProperty> specPropsWithValue,
            List<Error> setPropertyErrors)
        {
            var specificationProperties = specProps as SpecificationProperty[] ?? specProps.ToArray();
            var ctor = typeInfo.GetTypeInfo().GetConstructor(
                specificationProperties.Select(sp => sp.Property.PropertyType).ToArray()
            );

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Type {typeInfo.FullName} appears to be immutable, but no constructor found to accept values.");
            }

            var propsWithValue = specPropsWithValue as SpecificationProperty[] ?? specPropsWithValue.ToArray();
            try
            {
                var values =
                    (from prms in ctor.GetParameters()
                     join sp in propsWithValue on prms.Name?.ToLower() equals sp.Property.Name.ToLower() into spv
                     from sp in spv.DefaultIfEmpty()
                     select ExtractValue(sp, prms)).ToArray();

                var immutable = (T)ctor.Invoke(values);

                return immutable;
            }
            catch (Exception)
            {
                var ctorArgs = propsWithValue
                    .Select(x => x.Property.Name.ToLowerInvariant()).ToArray();
                throw GetException(ctorArgs);
            }

#if NET8_0_OR_GREATER
            [UnconditionalSuppressMessage("Missing type annotation", "IL2072")]
#endif
            object ExtractValue(SpecificationProperty sp, ParameterInfo prms)
            {
                return sp == null
                    ? specificationProperties.First(s =>
                            string.Equals(s.Property.Name, prms.Name,
                                StringComparison.CurrentCultureIgnoreCase))
                        .Property.PropertyType.GetDefaultValue()
                    : sp.Value.GetValueOrDefault(
                        sp.Specification.DefaultValue.GetValueOrDefault(
                            sp.Specification.ConversionType.CreateDefaultForImmutable()));
            }

            Exception GetException(string[] s)
            {
                var ctorSyntax = s != null
                    ? " Constructor Parameters can be ordered as: " + $"'({string.Join(", ", s)})'"
                    : string.Empty;
                var msg =
                    $"Type {typeInfo.FullName} appears to be Immutable with invalid constructor. Check that constructor arguments have the same name and order of their underlying Type. {ctorSyntax}";
                InvalidOperationException invalidOperationException = new InvalidOperationException(msg);
                return invalidOperationException;
            }
        }
    }
}
