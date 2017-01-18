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
            var tokenList = tokens.Memorize();
            var switches = Switch.Partition(tokenList, typeLookup).Memorize();
            var scalars = Scalar.Partition(tokenList, typeLookup).Memorize();
            var sequences = Sequence.Partition(tokenList, typeLookup).Memorize();
            var nonOptions = tokenList
                .Where(t => !switches.Contains(t, ReferenceEqualityComparer.Default))
                .Where(t => !scalars.Contains(t, ReferenceEqualityComparer.Default))
                .Where(t => !sequences.Contains(t, ReferenceEqualityComparer.Default)).Memorize();
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