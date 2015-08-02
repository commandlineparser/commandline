// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    static class NameLookup
    {
        public static bool Contains(string name, IEnumerable<OptionSpecification> specifications, StringComparer comparer)
        {
            return specifications.Any(a => name.MatchName(a.ShortName, a.LongName, comparer));
        }

        public static Maybe<char> HavingSeparator(string name, IEnumerable<OptionSpecification> specifications,
            StringComparer comparer)
        {
            return specifications.SingleOrDefault(
                a => name.MatchName(a.ShortName, a.LongName, comparer) && a.Separator != '\0')
                .ToMaybe()
                .Return(spec => Maybe.Just(spec.Separator), Maybe.Nothing<char>());
        }

    }
}
