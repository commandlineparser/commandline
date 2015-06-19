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
            var tokensExceptSwitches = tokenList.Where(x => !switches.Contains(x)).ToList();
            var scalars = Scalar.Partition(tokensExceptSwitches, typeLookup).ToList();
            var tokensExceptSwitchesAndScalars = (tokensExceptSwitches.Where(x => !scalars.Contains(x))).ToList();
            var sequences = Sequence.Partition(tokensExceptSwitchesAndScalars, typeLookup).ToList();
            var tokensExceptSwitchesAndScalarsAndSeq = tokensExceptSwitchesAndScalars.Where(x => !sequences.Contains(x)).ToList();
            var values = tokensExceptSwitchesAndScalarsAndSeq.Where(v => v.IsValue()).ToList();
            var errors = tokensExceptSwitchesAndScalarsAndSeq.Where(x => !values.Contains(x));

            return TokenPartitions.Create(
                    switches.Select(t => KeyValuePairHelper.Create(t.Text, "true"))
                        .Concat(scalars.Pairwise((f, s) => KeyValuePairHelper.Create(f.Text, s.Text)))
                        .Concat(KeyValuePairHelper.CreateSequence(sequences)),
                values.Select(t => t.Text),
                errors);
        }
    }
}