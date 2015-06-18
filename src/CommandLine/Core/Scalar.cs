// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class Scalar
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<Tuple<DescriptorType, Maybe<int>>>> typeLookup)
        {
            return from tseq in tokens.Pairwise(
                (f, s) =>
                        f.IsName() && s.IsValue()
                            ? typeLookup(f.Text).Return(info =>
                                    info.Item1 == DescriptorType.Scalar ? new[] { f, s } : new Token[] { }, new Token[] { })
                                    : new Token[] { })
                   from t in tseq
                   select t;
        }
    }
}
