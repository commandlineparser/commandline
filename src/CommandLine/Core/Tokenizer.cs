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
            return Tokenizer.Tokenize(arguments, nameLookup, tokens => tokens);
        }

        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, NameLookupResult> nameLookup,
            Func<IEnumerable<Token>, IEnumerable<Token>> normalize)
        {
            var errors = new List<Error>();
            Action<Error> onError = errors.Add;

            var tokens = (from arg in arguments
                          from token in !arg.StartsWith("-", StringComparison.Ordinal)
                               ? new[] { Token.Value(arg) }
                               : arg.StartsWith("--", StringComparison.Ordinal)
                                     ? TokenizeLongName(arg, onError)
                                     : TokenizeShortName(arg, nameLookup)
                          select token)
                            .Memorize();

            var normalized = normalize(tokens);

            var unkTokens = (from t in normalized where t.IsName() && nameLookup(t.Text) == NameLookupResult.NoOptionFound select t).Memorize();

            return Result.Succeed(normalized.Where(x => !unkTokens.Contains(x)), errors.Concat(from t in unkTokens select new UnknownOptionError(t.Text)));
        }

        public static Result<IEnumerable<Token>, Error> PreprocessDashDash(
            IEnumerable<string> arguments,
            Func<IEnumerable<string>, Result<IEnumerable<Token>, Error>> tokenizer)
        {
            if (arguments.Any(arg => arg.EqualsOrdinal("--")))
            {
                var tokenizerResult = tokenizer(arguments.TakeWhile(arg => !arg.EqualsOrdinal("--")));
                var values = arguments.SkipWhile(arg => !arg.EqualsOrdinal("--")).Skip(1).Select(Token.Value);
                return tokenizerResult.Map(tokens => tokens.Concat(values));
            }
            return tokenizer(arguments);
        }

        public static Result<IEnumerable<Token>, Error> ExplodeOptionList(
            Result<IEnumerable<Token>, Error> tokenizerResult,
            Func<string, Maybe<char>> optionSequenceWithSeparatorLookup)
        {
            var tokens = tokenizerResult.SucceededWith();

            var replaces = tokens.Select((t, i) =>
                optionSequenceWithSeparatorLookup(t.Text)
                    .MapValueOrDefault(sep => Tuple.Create(i + 1, sep),
                        Tuple.Create(-1, '\0'))).SkipWhile(x => x.Item1 < 0);

            var exploded = tokens.Select((t, i) =>
                        replaces.FirstOrDefault(x => x.Item1 == i).ToMaybe()
                            .MapValueOrDefault(r => t.Text.Split(r.Item2).Select(Token.Value),
                                Enumerable.Empty<Token>().Concat(new[] { t })));

            var flattened = exploded.SelectMany(x => x);

            return Result.Succeed(flattened, tokenizerResult.SuccessfulMessages());
        }

        public static IEnumerable<Token> Normalize(
            IEnumerable<Token> tokens, Func<string, bool> nameLookup)
        {
            var indexes =
                from i in
                    tokens.Select(
                        (t, i) =>
                        {
                            var prev = tokens.ElementAtOrDefault(i - 1).ToMaybe();
                            return t.IsValue() && ((Value)t).ExplicitlyAssigned
                                   && prev.MapValueOrDefault(p => p.IsName() && !nameLookup(p.Text), false)
                                ? Maybe.Just(i)
                                : Maybe.Nothing<int>();
                        }).Where(i => i.IsJust())
                select i.FromJustOrFail();

            var toExclude =
                from t in
                    tokens.Select((t, i) => indexes.Contains(i) ? Maybe.Just(t) : Maybe.Nothing<Token>())
                        .Where(t => t.IsJust())
                select t.FromJustOrFail();

            var normalized = tokens.Where(t => toExclude.Contains(t) == false);

            return normalized;
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
                    var normalize = ignoreUnknownArguments
                        ? toks => Tokenizer.Normalize(toks,
                            name => NameLookup.Contains(name, optionSpecs, nameComparer) != NameLookupResult.NoOptionFound)
                        : new Func<IEnumerable<Token>, IEnumerable<Token>>(toks => toks);

                    var tokens = enableDashDash
                        ? Tokenizer.PreprocessDashDash(
                                arguments,
                                args =>
                                    Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, nameComparer), normalize))
                        : Tokenizer.Tokenize(arguments, name => NameLookup.Contains(name, optionSpecs, nameComparer), normalize);
                    var explodedTokens = Tokenizer.ExplodeOptionList(tokens, name => NameLookup.HavingSeparator(name, optionSpecs, nameComparer));
                    return explodedTokens;
                };
        }

        private static IEnumerable<Token> TokenizeShortName(
            string value,
            Func<string, NameLookupResult> nameLookup)
        {
            if (value.Length > 1 && value[0] == '-' && value[1] != '-')
            {
                var text = value.Substring(1);

                if (char.IsDigit(text[0]))
                {
                    yield return Token.Value(value);
                    yield break;
                }

                if (value.Length == 2)
                {
                    yield return Token.Name(text);
                    yield break;
                }

                var i = 0;
                foreach (var c in text)
                {
                    var n = new string(c, 1);
                    var r = nameLookup(n);
                    // Assume first char is an option
                    if (i > 0 && r == NameLookupResult.NoOptionFound) break;
                    i++;
                    yield return Token.Name(n);
                    // If option expects a value (other than a boolean), assume following chars are that value
                    if (r == NameLookupResult.OtherOptionFound) break;
                }

                if (i < text.Length)
                {
                    yield return Token.Value(text.Substring(i));
                }
            }
        }

        private static IEnumerable<Token> TokenizeLongName(
            string value,
            Action<Error> onError)
        {
            if (value.Length > 2 && value.StartsWith("--", StringComparison.Ordinal))
            {
                var text = value.Substring(2);
                var equalIndex = text.IndexOf('=');
                if (equalIndex <= 0)
                {
                    yield return Token.Name(text);
                    yield break;
                }
                if (equalIndex == 1) // "--="
                {
                    onError(new BadFormatTokenError(value));
                    yield break;
                }

                var tokenMatch = Regex.Match(text, "^([^=]+)=([^ ].*)$");

                if (tokenMatch.Success)
                {
                    yield return Token.Name(tokenMatch.Groups[1].Value);
                    yield return Token.Value(tokenMatch.Groups[2].Value, true);
                }
                else
                {
                    onError(new BadFormatTokenError(value));
                    yield break;
                }
            }
        }
    }
}