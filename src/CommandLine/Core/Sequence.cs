// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine.Core
{
    static class Sequence
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            var sequences = new Dictionary<Token, IList<Token>>();
            var state = SequenceState.TokenSearch;
            Token nameToken = default;
            foreach (var token in tokens)
            {
                switch (state)
                {
                    case SequenceState.TokenSearch:
                        if (token.IsName())
                        {
                            if (typeLookup(token.Text).MatchJust(out var info) && info.TargetType == TargetType.Sequence)
                            {
                                nameToken = token;
                                state = SequenceState.TokenFound;
                            }
                        }
                        break;

                    case SequenceState.TokenFound:
                        if (token.IsValue())
                        {
                            if (sequences.TryGetValue(nameToken, out var sequence))
                            {
                                sequence.Add(token);
                            }
                            else
                            {
                                sequences[nameToken] = new List<Token>(new[] { token });
                            }
                        }
                        else if (token.IsName())
                        {
                            if (typeLookup(token.Text).MatchJust(out var info) && info.TargetType == TargetType.Sequence)
                            {
                                nameToken = token;
                                state = SequenceState.TokenFound;
                            }
                            else
                            {
                                state = SequenceState.TokenSearch;
                            }
                        }
                        else
                        {
                            state = SequenceState.TokenSearch;
                        }
                        break;
                }
            }

            foreach (var kvp in sequences)
            {
                yield return kvp.Key;
                foreach (var value in kvp.Value)
                {
                    yield return value;
                }
            }

                //return from tseq in tokens.Pairwise(
                //(f, s) =>
                //        f.IsName() && s.IsValue()
                //            ? typeLookup(f.Text).MapValueOrDefault(info =>
                //                   info.TargetType == TargetType.Sequence
                //                        ? new[] { f }.Concat(tokens.OfSequence(f, info))
                //                        : new Token[] { }, new Token[] { })
                //            : new Token[] { })
                //   from t in tseq
                //   select t;
        }

        //private static IEnumerable<Token> OfSequence(this IEnumerable<Token> tokens, Token nameToken, TypeDescriptor info)
        //{
        //    var state = SequenceState.TokenSearch;
        //    var count = 0;
        //    var max = info.MaxItems.GetValueOrDefault(int.MaxValue);
        //    var values = max != int.MaxValue
        //        ? new List<Token>(max)
        //        : new List<Token>();

        //    foreach (var token in tokens)
        //    {
        //        if (count == max)
        //        {
        //            break;
        //        }

        //        switch (state)
        //        {
        //            case SequenceState.TokenSearch:
        //                if (token.IsName() && token.Text.Equals(nameToken.Text))
        //                {
        //                    state = SequenceState.TokenFound;
        //                }
        //                break;

        //            case SequenceState.TokenFound:
        //                if (token.IsValue())
        //                {
        //                    state = SequenceState.ValueFound;
        //                    count++;
        //                    values.Add(token);
        //                }
        //                else
        //                {
        //                    // Invalid to provide option without value
        //                    return Enumerable.Empty<Token>();
        //                }
        //                break;

        //            case SequenceState.ValueFound:
        //                if (token.IsValue())
        //                {
        //                    count++;
        //                    values.Add(token);
        //                }
        //                else if (token.IsName() && token.Text.Equals(nameToken.Text))
        //                {
        //                    state = SequenceState.TokenFound;
        //                }
        //                else
        //                {
        //                    state = SequenceState.TokenSearch;
        //                }
        //                break;
        //        }
        //    }

        //    return values;
        //}

        private enum SequenceState
        {
            TokenSearch,
            TokenFound,
        }
    }
}
