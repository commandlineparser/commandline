// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;
using System.Text.RegularExpressions;

namespace CommandLine.Core
{
    static class Tokenizer
    {
        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup)
        {
            return Tokenizer.Tokenize(arguments, nameLookup, ignoreUnknownArguments:false, allowDashDash:true);
        }

        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup,
            bool ignoreUnknownArguments,
            bool allowDashDash)
        {
            var errors = new List<Error>();
            Action<string> onBadFormatToken = arg => errors.Add(new BadFormatTokenError(arg));
            Action<string> unknownOptionError = name => errors.Add(new UnknownOptionError(name));
            Action<string> doNothing = name => {};
            Action<string> onUnknownOption = ignoreUnknownArguments ? doNothing : unknownOptionError;

            int consumeNext = 0;
            Action<int> onConsumeNext = (n => consumeNext = consumeNext + n);

            bool isForced = false;

            var tokens = new List<Token>();

            var enumerator = arguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current) {
                    case null:
                        break;

                    case string arg when consumeNext > 0:
                        tokens.Add(new Value(arg, isForced));
                        consumeNext = consumeNext - 1;
                        break;

                    case "--" when allowDashDash:
                        consumeNext = System.Int32.MaxValue;
                        isForced = true;
                        break;

                    case "--":
                        tokens.Add(new Value("--", isForced));
                        break;

                    case "-":
                        // A single hyphen is always a value (it usually means "read from stdin" or "write to stdout")
                        tokens.Add(new Value("-", isForced));
                        break;

                    case string arg when arg.StartsWith("--"):
                        tokens.AddRange(TokenizeLongName(arg, nameLookup, onBadFormatToken, onUnknownOption, onConsumeNext));
                        break;

                    case string arg when arg.StartsWith("-"):
                        tokens.AddRange(TokenizeShortName(arg, nameLookup, onUnknownOption, onConsumeNext));
                        break;

                    case string arg:
                        // If we get this far, it's a plain value
                        tokens.Add(new Value(arg, isForced));
                        break;
                }
            }

            return Result.Succeed<IEnumerable<Token>, Error>(tokens.AsEnumerable(), errors.AsEnumerable());
        }

        public static Result<IEnumerable<Token>, Error> ExplodeOptionList(
            Result<IEnumerable<Token>, Error> tokenizerResult,
            Func<string, Maybe<char>> optionSequenceWithSeparatorLookup)
        {
            var tokens = tokenizerResult.SucceededWith().Memoize();

            var replaces = tokens.Select((t, i) =>
                optionSequenceWithSeparatorLookup(t.Text)
                    .MapValueOrDefault(sep => Tuple.Create(i + 1, sep),
                        Tuple.Create(-1, '\0'))).SkipWhile(x => x.Item1 < 0).Memoize();

            var exploded = tokens.Select((t, i) =>
                        replaces.FirstOrDefault(x => x.Item1 == i).ToMaybe()
                            .MapValueOrDefault(r => t.Text.Split(r.Item2).Select(Token.Value),
                                Enumerable.Empty<Token>().Concat(new[] { t })));

            var flattened = exploded.SelectMany(x => x);

            return Result.Succeed(flattened, tokenizerResult.SuccessMessages());
        }

        public static Func<
                    IEnumerable<string>,
                    IEnumerable<OptionSpecification>,
                    Result<IEnumerable<Token>, Error>>
            ConfigureTokenizer(
                    StringComparer nameComparer,
                    bool ignoreUnknownArguments,
                    bool enableDashDash)
        {
            return (arguments, optionSpecs) =>
                {
                    var tokens = Tokenizer.Tokenize(arguments, name => NameLookup.Contains(name, optionSpecs, nameComparer), ignoreUnknownArguments, enableDashDash);
                    var explodedTokens = Tokenizer.ExplodeOptionList(tokens, name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                    return explodedTokens;
                };
        }

        private static IEnumerable<Token> TokenizeShortName(
            string arg,
            Func<string, NameLookupResult> nameLookup,
            Action<string> onUnknownOption,
            Action<int> onConsumeNext)
        {

            // First option char that requires a value means we swallow the rest of the string as the value
            // But if there is no rest of the string, then instead we swallow the next argument
            string chars = arg.Substring(1);
            int len = chars.Length;
            if (len > 0 && Char.IsDigit(chars[0]))
            {
                // Assume it's a negative number
                yield return Token.Value(arg);
                yield break;
            }
            for (int i = 0; i < len; i++)
            {
                var s = new String(chars[i], 1);
                switch(nameLookup(s))
                {
                    case NameLookupResult.OtherOptionFound:
                        yield return Token.Name(s);

                        if (i+1 < len)
                        {
                            // Rest of this is the value (e.g. "-sfoo" where "-s" is a string-consuming arg)
                            yield return Token.Value(chars.Substring(i+1));
                            yield break;
                        }
                        else
                        {
                            // Value is in next param (e.g., "-s foo")
                            onConsumeNext(1);
                        }
                        break;

                    case NameLookupResult.NoOptionFound:
                        onUnknownOption(s);
                        break;

                    default:
                        yield return Token.Name(s);
                        break;
                }
            }
        }

        private static IEnumerable<Token> TokenizeLongName(
            string arg,
            Func<string, NameLookupResult> nameLookup,
            Action<string> onBadFormatToken,
            Action<string> onUnknownOption,
            Action<int> onConsumeNext)
        {
            string[] parts = arg.Substring(2).Split(new char[] { '=' }, 2);
            string name = parts[0];
            string value = (parts.Length > 1) ? parts[1] : null;
            // A parameter like "--stringvalue=" is acceptable, and makes stringvalue be the empty string
            if (String.IsNullOrWhiteSpace(name) || name.Contains(" "))
            {
                onBadFormatToken(arg);
                yield break;
            }
            switch(nameLookup(name))
            {
                case NameLookupResult.NoOptionFound:
                    onUnknownOption(name);
                    yield break;

                case NameLookupResult.OtherOptionFound:
                    yield return Token.Name(name);
                    if (value == null) // NOT String.IsNullOrEmpty
                    {
                        onConsumeNext(1);
                    }
                    else
                    {
                        yield return Token.Value(value);
                    }
                    break;

                default:
                    yield return Token.Name(name);
                    break;
            }
        }
    }
}
