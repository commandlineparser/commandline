// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

namespace CommandLine.Core
{
    internal static class NameExtensions
    {
        public static bool MatchName(this string value, string shortName, string longName, StringComparer comparer)
        {
            return value.Length == 1
               ? comparer.Equals(value, shortName)
               : comparer.Equals(value, longName);
        }
    }
}
