// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class Switch
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            return from t in tokens
                   where typeLookup(t.Text).MapValueOrDefault(info => t.IsName() && info.TargetType == TargetType.Switch, false)
                   select t;
        }
    }
}
