// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class InstanceChooser
    {
        public static ParserResult<object> Choose(
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
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
                parsingCulture);
        }

        public static ParserResult<object> Choose(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, StatePair<IEnumerable<Token>>> tokenizer,
            IEnumerable<Type> types,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {
            var verbs = Verb.SelectFromTypes(types);

            return arguments.Empty()
                ? new NotParsed<object>(
                    new NullInstance(),
                    types,
                    new[] { new NoVerbSelectedError() })
                : nameComparer.Equals("help", arguments.First())
                   ? new NotParsed<object>(
                       new NullInstance(),
                       types, new[] { CreateHelpVerbRequestedError(
                                        verbs,
                                        arguments.Skip(1).SingleOrDefault() ?? string.Empty,
                                        nameComparer) })
                   : MatchVerb(tokenizer, verbs, arguments, nameComparer, parsingCulture);
        }

        private static ParserResult<object> MatchVerb(
            Func<IEnumerable<string>, IEnumerable<OptionSpecification>, StatePair<IEnumerable<Token>>> tokenizer,
            IEnumerable<Tuple<Verb, Type>> verbs,
            IEnumerable<string> arguments,
            StringComparer nameComparer,
            CultureInfo parsingCulture)
        {     
            return verbs.Any(a => nameComparer.Equals(a.Item1.Name, arguments.First()))
                ? InstanceBuilder.Build(
                    Maybe.Just<Func<object>>(() => verbs.Single(v => nameComparer.Equals(v.Item1.Name, arguments.First())).Item2.AutoDefault()),
                    tokenizer,
                    arguments.Skip(1),
                    nameComparer,
                    parsingCulture)
                : new NotParsed<object>(
                    new NullInstance(),
                    verbs.Select(v => v.Item2),
                    new[] { new BadVerbSelectedError(arguments.First()) });
        }

       private static HelpVerbRequestedError CreateHelpVerbRequestedError(
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
    }
}
