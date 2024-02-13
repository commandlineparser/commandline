// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Core
{
    static class InstanceChooser
    {
        public static ParserResult<object> Choose(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            return Choose(
                tokenizer,
                types,
                arguments,
                nameComparer,
                ignoreValueCase,
                parsingCulture,
                autoHelp,
                autoVersion,
                false,
                nonFatalErrors);
        }

        public static ParserResult<object> Choose(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            bool allowMultiInstance,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            var types1 = types as Type[] ?? types.ToArray();
            var verbs = Verb.SelectFromTypes(types1).ToArray();
            var defaultVerbs = verbs.Where(t => t.Item1.IsDefault);

            var enumerable = defaultVerbs as Tuple<Verb, Type>[] ?? defaultVerbs.ToArray();
            int defaultVerbCount = enumerable.Length;
            if (defaultVerbCount > 1)
                return MakeNotParsed(types1, new MultipleDefaultVerbsError());

            var defaultVerb = defaultVerbCount == 1 ? enumerable.First() : null;

            var arguments1 = arguments as string[] ?? arguments.ToArray();

            ParserResult<object> choose()
            {
                var firstArg = arguments1.First();

                bool preprocCompare(string command) =>
                    nameComparer.Equals(command, firstArg) ||
                    nameComparer.Equals(string.Concat("--", command), firstArg);

                return autoHelp && preprocCompare("help")
                    ? MakeNotParsed(types1,
                        MakeHelpVerbRequestedError(verbs,
                            arguments1.Skip(1).FirstOrDefault() ?? string.Empty, nameComparer))
                    : (autoVersion && preprocCompare("version"))
                        ? MakeNotParsed(types1, new VersionRequestedError())
                        : MatchVerb(tokenizer, verbs, defaultVerb, arguments1, nameComparer, ignoreValueCase,
                            parsingCulture, autoHelp, autoVersion, allowMultiInstance, nonFatalErrors);
            }

            return arguments1.Length != 0
                ? choose()
                : (defaultVerbCount == 1
                    ? MatchDefaultVerb(tokenizer, verbs, defaultVerb, arguments1, nameComparer, ignoreValueCase,
                        parsingCulture, autoHelp, autoVersion, nonFatalErrors)
                    : MakeNotParsed(types1, new NoVerbSelectedError()));
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on type", "IL2072")]
#endif
        private static ParserResult<object> MatchDefaultVerb(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Tuple<Verb, Type>> verbs,
            Tuple<Verb, Type> defaultVerb,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            return defaultVerb is not null
                ? InstanceBuilder.Build(
                    Maybe.Just<Func<object>>(() => defaultVerb.Item2.AutoDefault()),
                    tokenizer,
                    arguments,
                    nameComparer,
                    ignoreValueCase,
                    parsingCulture,
                    autoHelp,
                    autoVersion,
                    nonFatalErrors)
                : MakeNotParsed(verbs.Select(v => v.Item2), new BadVerbSelectedError(arguments.First()));
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on type", "IL2072")]
#endif
        private static ParserResult<object> MatchVerb(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Tuple<Verb, Type>> verbs,
            Tuple<Verb, Type> defaultVerb,
            string[] arguments,
            StringComparer nameComparer,
            bool ignoreValueCase,
            CultureInfo parsingCulture,
            bool autoHelp,
            bool autoVersion,
            bool allowMultiInstance,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            string firstArg = arguments[0];

            var verbUsed = verbs.FirstOrDefault(vt =>
                nameComparer.Equals(vt.Item1.Name, firstArg)
             || vt.Item1.Aliases.Any(alias => nameComparer.Equals(alias, firstArg))
            );

            if (verbUsed == default)
            {
                return MatchDefaultVerb(tokenizer, verbs, defaultVerb, arguments, nameComparer, ignoreValueCase,
                    parsingCulture, autoHelp, autoVersion, nonFatalErrors);
            }

            return InstanceBuilder.Build(
                Maybe.Just(
                    () => verbUsed.Item2.AutoDefault()),
                tokenizer,
                arguments.Skip(1),
                nameComparer,
                ignoreValueCase,
                parsingCulture,
                autoHelp,
                autoVersion,
                allowMultiInstance,
                nonFatalErrors);
        }

        private static HelpVerbRequestedError MakeHelpVerbRequestedError(
            IEnumerable<Tuple<Verb, Type>> verbs,
            string verb,
            StringComparer nameComparer)
        {
            return verb.Length > 0
                ? verbs.SingleOrDefault(v => nameComparer.Equals(v.Item1.Name, verb))
                    .ToMaybe()
                    .MapValueOrDefault(
                        v => new HelpVerbRequestedError(v.Item1.Name, v.Item2, true),
                        new HelpVerbRequestedError(null, null, false))
                : new HelpVerbRequestedError(null, null, false);
        }

        private static NotParsed<object> MakeNotParsed(IEnumerable<Type> types, params Error[] errors)
        {
            return new NotParsed<object>(TypeInfo.Create(typeof(NullInstance), types), errors);
        }
    }
}
