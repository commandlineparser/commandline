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
    internal static class InstanceChooser
    {
        public static ParserResult<object> Choose(
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            return Choose(
                (args, optionSpecs) =>
                    {
                        var tokens = Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, nameComparer));
                        var explodedTokens = Tokenizer.ExplodeOptionList(tokens, name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                        return explodedTokens;
                    },
                types,
                arguments,
                nameComparer,
                parsingCulture,
                nonFatalErrors);
        }

        public static ParserResult<object> Choose(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            if (arguments.Empty())
            {
                return MakeNotParsed(types, new NoVerbSelectedError());
            }

            var firstArg = arguments.First();

            Func<string, bool> preprocCompare = command =>
                    nameComparer.Equals(command, firstArg) ||
                    nameComparer.Equals(string.Concat("--", command), firstArg);

            var verbs = Verb.SelectFromTypes(types);

            return preprocCompare("help")
                ? MakeNotParsed(types,
                    MakeHelpVerbRequestedError(verbs,
                        arguments.Skip(1).SingleOrDefault() ?? string.Empty, nameComparer))
                : preprocCompare("version")
                    ? MakeNotParsed(types, new VersionRequestedError())
                    : MatchVerb(tokenizer, verbs, arguments, nameComparer, parsingCulture, nonFatalErrors);
        }

        private static ParserResult<object> MatchVerb(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, Result<IEnumerable<Token>, Error>> tokenizer,
            IEnumerable<Tuple<Verb, Type>> verbs,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture,
            IEnumerable<ErrorType> nonFatalErrors)
        {
            return verbs.Any(a => nameComparer.Equals(a.Item1.Name, arguments.First()))
                ? InstanceBuilder.Build(
                    Maybe.Just<Func<object>>(
                        () =>
                            verbs.Single(v => nameComparer.Equals(v.Item1.Name, arguments.First())).Item2.AutoDefault()),
                    tokenizer,
                    arguments.Skip(1),
                    nameComparer,
                    parsingCulture,
                    nonFatalErrors)
                : MakeNotParsed(verbs.Select(v => v.Item2), new BadVerbSelectedError(arguments.First()));
        }

        private static HelpVerbRequestedError MakeHelpVerbRequestedError(
            IEnumerable<Tuple<Verb, Type>> verbs,
            string verb,
            StringComparer nameComparer)
        {
            return verb.Length > 0
                ? verbs.SingleOrDefault(v => nameComparer.Equals(v.Item1.Name, verb))
                        .ToMaybe()
                        .Return(
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
