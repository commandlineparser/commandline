// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class Sequence
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            if (tokens.Empty())
            {
                yield break;
            }
            var items = 0;
            var first = tokens.First();
            if (first.Tag == TokenType.Name)
            {
                TypeDescriptor info;
                if (typeLookup(first.Text).MatchJust(out info))
                {
                    if (info.Tag == TypeDescriptorKind.Sequence
                        && tokens.Skip(1).Take(1).Any())
                    {
                        yield return first;

                        foreach (var token in tokens.Skip(1).Where(token => token.IsValue()))
                        {
                            items++;
                            yield return token;
                        }
                    }
                }
            }
            foreach (var token in Partition(tokens.Skip(1 + items), typeLookup))
            {
                yield return token;
            }
        }

        //public static IEnumerable<Token> Partition(
        //    IEnumerable<Token> tokens,
        //    Func<string, Maybe<Tuple<DescriptorType, Maybe<int>>>> typeLookup)
        //{
        //    return from tseq in tokens.Pairwise(
        //        (f, s) =>
        //                f.IsName() && s.IsValue()
        //                    ? typeLookup(f.Text).Return(info =>
        //                           info.Item1 == DescriptorType.Sequence
        //                                ? new[] { f }.Concat(tokens.SkipWhile(t => t.Equals(f)).TakeWhile(v => v.IsValue()))
        //                                : new Token[] { }, new Token[] { })
        //                    : new Token[] { })
        //           from t in tseq
        //           select t;
        //}
    }
}
