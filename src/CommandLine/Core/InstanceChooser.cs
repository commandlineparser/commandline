// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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
            return InstanceChooser.Choose(
                (args, optionSpecs) => Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, nameComparer)),
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
                ? ParserResult.Create<object>(
                    ParserResultType.Verbs, new NullInstance(), new[] { new NoVerbSelectedError() }, Maybe.Just(types))
                : nameComparer.Equals("help", arguments.First())
                   ? ParserResult.Create<object>(
                        ParserResultType.Verbs,
                        new NullInstance(), new[] { CreateHelpVerbRequestedError(
                            verbs,
                            arguments.Skip(1).SingleOrDefault() ?? string.Empty,
                            nameComparer) }, Maybe.Just(types))
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
                    () => Activator.CreateInstance(verbs.Single(v => nameComparer.Equals(v.Item1.Name, arguments.First())).Item2),
                    tokenizer,
                    arguments.Skip(1),
                    nameComparer,
                    parsingCulture)
                : ParserResult.Create<object>(
                    ParserResultType.Verbs,
                    new NullInstance(),
                    new[] { new BadVerbSelectedError(arguments.First()) },
                    Maybe.Just(verbs.Select(v => v.Item2)));
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
