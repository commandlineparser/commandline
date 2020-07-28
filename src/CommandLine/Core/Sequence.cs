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
                        //IsValueForced are tokens after --
                        if (token.IsValue() && !token.IsValueForced())
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
        }

        
        private enum SequenceState
        {
            TokenSearch,
            TokenFound,
        }
    }
}
