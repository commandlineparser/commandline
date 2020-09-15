// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class ScalarSwitch
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            return tokens.Zip(tokens.Skip(1).Concat(new Token[] { new Value("true") }),
                (f, s) =>
                    f.IsName() && s.IsValue()
                        ? typeLookup(f.Text).MapValueOrDefault(info =>
                                info.TargetType == TargetType.ScalarSwitch ? new[] { f, s } : new Token[] { }, new Token[] { })
                                : f.IsName()
                                    ? typeLookup(f.Text).MapValueOrDefault(info => new Token[] { f, new Value(f.IsName() && info.TargetType == TargetType.ScalarSwitch ? "true" : "false") }, new Token[] { })
                                    : new Token[] { }
            )
            .SelectMany(t => t);
        }
    }
}
