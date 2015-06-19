// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class Sequence
    {
        //public static IEnumerable<Token> Partition(
        //    IEnumerable<Token> tokens,
        //    Func<string, Maybe<TypeDescriptor>> typeLookup)
        //{
        //    if (tokens.Empty())
        //    {
        //        yield break;
        //    }
        //    var items = 0;
        //    var first = tokens.First();
        //    if (first.Tag == TokenType.Name)
        //    {
        //        TypeDescriptor info;
        //        if (typeLookup(first.Text).MatchJust(out info))
        //        {
        //            if (info.Tag == TypeDescriptorKind.Sequence
        //                && IsNextTokenAValue(tokens))
        //            {
        //                yield return first;

        //                foreach (var token in tokens.Skip(1)) //.Where(token => token.IsValue()))
        //                {
        //                    if (token.IsValue())
        //                    {
        //                        items++;
        //                        yield return token;
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    foreach (var token in Partition(tokens.Skip(1 + items), typeLookup))
        //    {
        //        yield return token;
        //    }
        //}

        //private static bool IsNextTokenAValue(IEnumerable<Token> tokens)
        //{
        //    var next = tokens.Skip(1).Take(1);

        //    return next.Any()
        //        ? next.Single().IsValue()
        //        : false;
        //}

        //public static IEnumerable<Token> Partition(
        //    IEnumerable<Token> tokens,
        //    Func<string, Maybe<TypeDescriptor>> typeLookup)
        //{
        //    return from tseq in tokens.Pairwise(
        //        (f, s) =>
        //                f.IsName() && s.IsValue()
        //                    ? typeLookup(f.Text).Return(info =>
        //                           info.Tag == TypeDescriptorKind.Sequence
        //                                ? new[] { f }.Concat(tokens.SkipWhile(t => t.Equals(f)).TakeWhile(v => v.IsValue()))
        //                                : new Token[] { }, new Token[] { })
        //                    : new Token[] { })
        //           from t in tseq
        //           select t;
        //}

        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            return from tseq in tokens.Pairwise(
                (f, s) =>
                        f.IsName() && s.IsValue()
                            ? typeLookup(f.Text).Return(info =>
                                   info.Tag == TypeDescriptorKind.Sequence
                                        ? new[] { f }.Concat(tokens.OfSequence(f))
                                        : new Token[] { }, new Token[] { })
                            : new Token[] { })
                   from t in tseq
                   select t;
        }

        private static IEnumerable<Token> OfSequence(this IEnumerable<Token> tokens, Token nameToken)
        {
            var nameIndex = tokens.IndexOf(t => t.Equals(nameToken));
            if (nameIndex >= 0)
            {
                return tokens.Skip(nameIndex + 1).TakeWhile(v => v.IsValue());
            }
            return new Token[] { };
        }
    }
}
