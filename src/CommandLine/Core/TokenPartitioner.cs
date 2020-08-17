// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine.Core
{
    static class TokenPartitioner
    {
        public static
            Tuple<IEnumerable<KeyValuePair<string, IEnumerable<string>>>, IEnumerable<string>, IEnumerable<Token>> Partition(
                IEnumerable<Token> tokens,
                Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            IEqualityComparer<Token> tokenComparer = ReferenceEqualityComparer.Default;

            var tokenList = tokens.Memoize();
            var partitioned = PartitionTokensByType(tokenList, typeLookup);
            var switches = partitioned.Item1;
            var scalars = partitioned.Item2;
            var sequences = partitioned.Item3;
            var nonOptions = partitioned.Item4;
            var valuesAndErrors = nonOptions.PartitionByPredicate(v => v.IsValue());
            var values = valuesAndErrors.Item1;
            var errors = valuesAndErrors.Item2;

            return Tuple.Create(
                    KeyValuePairHelper.ForSwitch(switches)
                        .Concat(KeyValuePairHelper.ForScalar(scalars))
                        .Concat(KeyValuePairHelper.ForSequence(sequences)),
                values.Select(t => t.Text),
                errors);
        }

        public static Tuple<IEnumerable<Token>, IEnumerable<Token>, IEnumerable<Token>, IEnumerable<Token>> PartitionTokensByType(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            var switchTokens = new List<Token>();
            var scalarTokens = new List<Token>();
            var sequenceTokens = new List<Token>();
            var nonOptionTokens = new List<Token>();
            var sequences = new Dictionary<Token, IList<Token>>();
            var count = new Dictionary<Token, int>();
            var max = new Dictionary<Token, Maybe<int>>();
            var state = SequenceState.TokenSearch;
            Token nameToken = null;
            foreach (var token in tokens)
            {
                if (token.IsValueForced())
                {
                    nonOptionTokens.Add(token);
                }
                else if (token.IsName())
                {
                    if (typeLookup(token.Text).MatchJust(out var info))
                    {
                        switch (info.TargetType)
                        {
                        case TargetType.Switch:
                            nameToken = null;
                            switchTokens.Add(token);
                            state = SequenceState.TokenSearch;
                            break;
                        case TargetType.Scalar:
                            nameToken = token;
                            scalarTokens.Add(nameToken);
                            state = SequenceState.ScalarTokenFound;
                            break;
                        case TargetType.Sequence:
                            nameToken = token;
                            if (! sequences.ContainsKey(nameToken))
                            {
                                sequences[nameToken] = new List<Token>();
                                count[nameToken] = 0;
                                max[nameToken] = info.MaxItems;
                            }
                            state = SequenceState.SequenceTokenFound;
                            break;
                        }
                    }
                    else
                    {
                        nameToken = null;
                        nonOptionTokens.Add(token);
                        state = SequenceState.TokenSearch;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case SequenceState.TokenSearch:
                        case SequenceState.ScalarTokenFound when nameToken == null:
                        case SequenceState.SequenceTokenFound when nameToken == null:
                            // if (nameToken == null) Console.WriteLine($"  (because there was no nameToken)");
                            nameToken = null;
                            nonOptionTokens.Add(token);
                            state = SequenceState.TokenSearch;
                            break;

                        case SequenceState.ScalarTokenFound:
                            nameToken = null;
                            scalarTokens.Add(token);
                            state = SequenceState.TokenSearch;
                            break;

                        case SequenceState.SequenceTokenFound:
                            if (sequences.TryGetValue(nameToken, out var sequence)) {
                                // if (max[nameToken].MatchJust(out int m) && count[nameToken] >= m)
                                // {
                                //     // This sequence is completed, so this and any further values are non-option values
                                //     nameToken = null;
                                //     nonOptionTokens.Add(token);
                                //     state = SequenceState.TokenSearch;
                                // }
                                // else
                                {
                                    sequence.Add(token);
                                    count[nameToken]++;
                                }
                            }
                            else
                            {
                                Console.WriteLine("***BUG!!!***");
                                throw new NullReferenceException($"Sequence for name {nameToken} doesn't exist, and it should");
                                // sequences[nameToken] = new List<Token>(new[] { token });
                            }
                            break;
                        }
                    }
                }

            foreach (var kvp in sequences)
            {
                if (kvp.Value.Empty()) {
                    nonOptionTokens.Add(kvp.Key);
                }
                else
                {
                    sequenceTokens.Add(kvp.Key);
                    sequenceTokens.AddRange(kvp.Value);
                }
            }
            return Tuple.Create(
                (IEnumerable<Token>)switchTokens,
                (IEnumerable<Token>)scalarTokens,
                (IEnumerable<Token>)sequenceTokens,
                (IEnumerable<Token>)nonOptionTokens
            );
        }

        private enum SequenceState
        {
            TokenSearch,
            SequenceTokenFound,
            ScalarTokenFound,
        }

    }
}
