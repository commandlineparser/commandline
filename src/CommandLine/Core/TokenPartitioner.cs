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
            var separatorSeen = false;
            Token nameToken = null;
            foreach (var token in tokens)
            {
                if (token.IsValueForced())
                {
                    separatorSeen = false;
                    nonOptionTokens.Add(token);
                }
                else if (token.IsName())
                {
                    separatorSeen = false;
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
                            separatorSeen = false;
                            nameToken = null;
                            nonOptionTokens.Add(token);
                            state = SequenceState.TokenSearch;
                            break;

                        case SequenceState.ScalarTokenFound:
                            separatorSeen = false;
                            nameToken = null;
                            scalarTokens.Add(token);
                            state = SequenceState.TokenSearch;
                            break;

                        case SequenceState.SequenceTokenFound:
                            if (sequences.TryGetValue(nameToken, out var sequence)) {
                                if (max[nameToken].MatchJust(out int m) && count[nameToken] >= m)
                                {
                                    // This sequence is completed, so this and any further values are non-option values
                                    nameToken = null;
                                    nonOptionTokens.Add(token);
                                    state = SequenceState.TokenSearch;
                                }
                                else if (token.IsValueFromSeparator())
                                {
                                    separatorSeen = true;
                                    sequence.Add(token);
                                    count[nameToken]++;
                                }
                                else if (separatorSeen)
                                {
                                    // Previous token came from a separator but this one didn't: sequence is completed
                                    separatorSeen = false;
                                    nameToken = null;
                                    nonOptionTokens.Add(token);
                                    state = SequenceState.TokenSearch;
                                }
                                else
                                {
                                    sequence.Add(token);
                                    count[nameToken]++;
                                }
                            }
                            else
                            {
                                // Should never get here, but just in case:
                                separatorSeen = false;
                                sequences[nameToken] = new List<Token>(new[] { token });
                                count[nameToken] = 0;
                                max[nameToken] = Maybe.Nothing<int>();
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
