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
            var switches = new HashSet<Token>(Switch.Partition(tokenList, typeLookup), tokenComparer);
            var scalars = new HashSet<Token>(Scalar.Partition(tokenList, typeLookup), tokenComparer);
            var sequences = new HashSet<Token>(Sequence.Partition(tokenList, typeLookup), tokenComparer);
            var nonOptions = tokenList
                .Where(t => !switches.Contains(t))
                .Where(t => !scalars.Contains(t))
                .Where(t => !sequences.Contains(t)).Memoize();
            var values = nonOptions.Where(v => v.IsValue()).Memoize();
            var errors = nonOptions.Except(values, (IEqualityComparer<Token>)ReferenceEqualityComparer.Default).Memoize();

            return Tuple.Create(
                    KeyValuePairHelper.ForSwitch(switches)
                        .Concat(KeyValuePairHelper.ForScalar(scalars))
                        .Concat(KeyValuePairHelper.ForSequence(sequences)),
                values.Select(t => t.Text),
                errors);
        }
    }
}