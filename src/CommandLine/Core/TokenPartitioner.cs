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
            Tuple<
                IEnumerable<KeyValuePair<string, IEnumerable<string>>>, // options
                IEnumerable<string>,                                    // values
                IEnumerable<Token>                                      // errors
            > Partition(
                IEnumerable<Token> tokens,
                Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            IEqualityComparer<Token> tokenComparer = ReferenceEqualityComparer.Default;

            var tokenList = tokens.Memorize();
            var switches = Switch.Partition(tokenList, typeLookup).ToSet(tokenComparer);
            var scalars = Scalar.Partition(tokenList, typeLookup).ToSet(tokenComparer);
            var sequences = Sequence.Partition(tokenList, typeLookup).ToSet(tokenComparer);
            var nonOptions = tokenList
                .Where(t => !switches.Contains(t))
                .Where(t => !scalars.Contains(t))
                .Where(t => !sequences.Contains(t)).Memorize();
            var values = nonOptions.Where(v => v.IsValue()).Memorize();
            var errors = nonOptions.Except(values, (IEqualityComparer<Token>)ReferenceEqualityComparer.Default).Memorize();

            return Tuple.Create(
                    KeyValuePairHelper.ForSwitch(switches)
                        .Concat(KeyValuePairHelper.ForScalar(scalars))
                        .Concat(KeyValuePairHelper.ForSequence(sequences)),
                values.Select(t => t.Text),
                errors);
        }
    }
}