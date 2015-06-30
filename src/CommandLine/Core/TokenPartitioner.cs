// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class TokenPartitioner
    {
        public static
            TokenPartitions Partition(
                IEnumerable<Token> tokens,
                Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            var tokenList = tokens.ToList();
            var switches = Switch.Partition(tokenList, typeLookup).ToList();
            var scalars = Scalar.Partition(tokenList, typeLookup).ToList();
            var sequences = Sequence.Partition(tokenList, typeLookup).ToList();
            var nonOptions = tokenList
                .Where(t => !switches.Contains(t))
                .Where(t => !scalars.Contains(t))
                .Where(t => !sequences.Contains(t)).ToList();
            var values = nonOptions.Where(v => v.IsValue()).ToList();
            var errors = nonOptions.Except(values).ToList();

            return TokenPartitions.Create(
                    KeyValuePairHelper.ForSwitch(switches)
                        .Concat(KeyValuePairHelper.ForScalar(scalars))
                        .Concat(KeyValuePairHelper.ForSequence(sequences)),
                values.Select(t => t.Text),
                errors);
        }
    }
}