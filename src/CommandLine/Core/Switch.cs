// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class Switch
    {
        public static IEnumerable<Token> Partition(
            IEnumerable<Token> tokens,
            Func<string, Maybe<TypeDescriptor>> typeLookup)
        {
            return from t in tokens
                   where typeLookup(t.Text).Return(info => t.IsName() && info.TargetType == TargetType.Boolean, false)
                   select t;
        }
    }
}
