// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CSharpx;

namespace CommandLine.Core
{
    enum NameLookupResult
    {
        NoOptionFound,
        BooleanOptionFound,
        OtherOptionFound
    }

    static class NameLookup
    {
        public static NameLookupResult Contains(string name, IEnumerable<OptionSpecification> specifications, StringComparer comparer)
        {
            var option = specifications.FirstOrDefault(a => name.MatchName(a.ShortName, a.LongName, comparer));
            if (option == null) return NameLookupResult.NoOptionFound;
            return option.ConversionType == typeof(bool)
                ? NameLookupResult.BooleanOptionFound
                : NameLookupResult.OtherOptionFound;
        }

        public static Maybe<char> HavingSeparator(string name, IEnumerable<OptionSpecification> specifications,
            StringComparer comparer)
        {
            return specifications.SingleOrDefault(
                a => name.MatchName(a.ShortName, a.LongName, comparer) && a.Separator != '\0')
                .ToMaybe()
                .MapValueOrDefault(spec => Maybe.Just(spec.Separator), Maybe.Nothing<char>());
        }

    }
}
