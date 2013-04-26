// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core
{
   static class NameLookup
    {
        public static bool Contains(string name, IEnumerable<OptionSpecification> specifications, StringComparer comparer)
        {
            if (name == null) throw new ArgumentNullException("name");

            return specifications.Any(a => name.MatchName(a.ShortName, a.LongName, comparer));
        }
    }
}
