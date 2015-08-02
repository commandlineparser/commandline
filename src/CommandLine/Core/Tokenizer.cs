// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Core
{
    internal static class Tokenizer
    {
        public static Result<IEnumerable<Token>, Error> Tokenize(
            IEnumerable<string> arguments,
            Func<string, bool> nameLookup)
        {
            var errors = new List<Error>();
            Action<Error> onError = errors.Add;

            var tokens = (from arg in arguments
                          from token in !arg.StartsWith("-", StringComparison.Ordinal)
                               ? new[] { Token.Value(arg) }
                               : arg.StartsWith("--", StringComparison.Ordinal)
                                     ? TokenizeLongName(arg, onError)
                                     : TokenizeShortName(arg, nameLookup)
                          select token).Memorize();

            var unkTokens = (from t in tokens where t.IsName() && !nameLookup(t.Text) select t).Memorize();

            return Result.Succeed(tokens.Where(x => !unkTokens.Contains(x)), errors.Concat(from t in unkTokens select new UnknownOptionError(t.Text)));
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

            var replaces = tokens.Select((t,i) =>
                optionSequenceWithSeparatorLookup(t.Text)
                    .Return(sep => Tuple.Create(i + 1, sep),
                        Tuple.Create(-1, '\0'))).SkipWhile(x => x.Item1 < 0);

            var exploded = tokens.Select((t, i) =>
                        replaces.FirstOrDefault(x => x.Item1 == i).ToMaybe()
                            .Return(r => t.Text.Split(r.Item2).Select(Token.Value),
                                Enumerable.Empty<Token>().Concat(new[]{ t })));

            var flattened = exploded.SelectMany(x => x);

            return Result.Succeed(flattened, tokenizerResult.SuccessfulMessages());
        }

        private static IEnumerable<Token> TokenizeShortName(
            string value,
            Func<string, bool> nameLookup)
        {
            if (value.Length > 1 || value[0] == '-' || value[1] != '-')
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

                var first = text.Substring(0, 1);
                yield return Token.Name(first);

                var seen = new List<char> { first[0] };

                foreach (var c in text.Substring(1))
                {
                    var n = new string(c, 1);
                    if (!seen.Contains(c) && nameLookup(n))
                    {
                        seen.Add(c);
                        yield return Token.Name(n);
                    }
                    else
                    {
                        break;
                    }
                }
                if (seen.Count() < text.Length)
                {
                    yield return Token.Value(text.Substring(seen.Count()));
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
                var parts = text.Split('=');
                yield return Token.Name(parts[0]);
                yield return Token.Value(parts[1]);
            }
        }
    }
}