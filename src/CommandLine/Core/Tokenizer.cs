// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class Tokenizer
    {
        public static StatePair<IEnumerable<Token>> Tokenize(
            IEnumerable<string> arguments,
            Func<string, bool> nameLookup)
        {
            if (arguments == null) throw new ArgumentNullException("arguments");

            var errors = new List<Error>();
            Action<Error> onError = e => errors.Add(e);

            var tokens = (from arg in arguments
                          from token in !arg.StartsWith("-", StringComparison.Ordinal)
                               ? new Token[] { Token.Value(arg) }
                               : arg.StartsWith("--", StringComparison.Ordinal)
                                     ? TokenizeLongName(arg, onError)
                                     : TokenizeShortName(arg, nameLookup)
                          select token).ToList();

            var unkTokens = (from t in tokens where t.IsName() && !nameLookup(t.Text) select t).ToList();

            return StatePair.Create(tokens.Where(x=>!unkTokens.Contains(x)), errors.Concat(from t in unkTokens select new UnknownOptionError(t.Text)));
        }

        public static StatePair<IEnumerable<Token>> PreprocessDashDash(
            IEnumerable<string> arguments,
            Func<IEnumerable<string>, StatePair<IEnumerable<Token>>> tokenizer)
        {
            if (arguments == null) throw new ArgumentNullException("arguments");

            if (arguments.Any(arg => arg.EqualsOrdinal("--")))
            {
                var tokenizerResult = tokenizer(arguments.TakeWhile(arg => !arg.EqualsOrdinal("--")));
                var values = arguments.SkipWhile(arg => !arg.EqualsOrdinal("--")).Skip(1).Select(t => Token.Value(t));
                return tokenizerResult.MapValue(tokens => tokens.Concat(values));
            }
            return tokenizer(arguments);
        }

        public static StatePair<IEnumerable<Token>> ExplodeOptionList(
            StatePair<IEnumerable<Token>> tokens,
            Func<string, Maybe<string>> optionSequenceWithSeparatorLookup)
        {
            if (tokens == null) throw new ArgumentNullException("tokens");

            if (tokens.Errors.Any() || tokens.Value.Count() == 1)
            {
                return tokens;
            }

            //var expandedTokens = tokens.Value.Pairwise(
            //    (f, s) =>
            //        {
            //            string separator;
            //            if (f.IsName() && optionSequenceWithSeparatorLookup(f.Text).MatchJust(out separator))
            //            {
            //                var parts = s.Text.Split(Convert.ToChar(separator));
            //                return Enumerable.Empty<Token>()
            //                    .Concat(new[] { f }).Concat(parts.Select(str => Token.Value(str)));
            //            }
            //            return Enumerable.Empty<Token>().Concat(new[] { f });
            //        }); 
            var withContext = tokens.Value.WithContext();
            var expandedTokens = withContext.Select(ictx =>
                Mapper(ictx, optionSequenceWithSeparatorLookup));

            var flattened = expandedTokens.SelectMany(x => x);

            //if (tokens.Value.HasEvenNumberOfItems())
            //{
                return StatePair.Create(flattened, tokens.Errors);
            //}
            //return StatePair.Create(expandedTokens.SelectMany(x => x).Concat(new[] { tokens.Value.Last() }), tokens.Errors);
        }

        private static IEnumerable<Token> Mapper(ItemWithContext<Token> ictx, Func<string, Maybe<string>> optionSequenceWithSeparatorLookup)
        {
                var curr = ictx.Current;
                var prev = ictx.Previous;
                var next = ictx.Next;

                string separator;
                if (curr != null && curr.IsName() && optionSequenceWithSeparatorLookup(curr.Text).MatchJust(out separator))
                {
                    if (next != null)
                    {
                        var parts = next.Text.Split(Convert.ToChar(separator));
                        return Enumerable.Empty<Token>()
                            .Concat(new[] { curr })
                            .Concat(parts.Select(str => Token.Value(str)));
                    }
                    return Enumerable.Empty<Token>().Concat(new[] { curr });
                }
                if (prev != null && prev.IsName() && optionSequenceWithSeparatorLookup(prev.Text).IsJust())
                {
                    return Enumerable.Empty<Token>();
                }
                return Enumerable.Empty<Token>().Concat(new[] { curr });
        }

        private static IEnumerable<Token> TokenizeShortName(
            string value,
            Func<string, bool> nameLookup)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

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
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

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