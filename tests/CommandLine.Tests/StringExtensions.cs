// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Tests
{
    static class StringExtensions
    {
        public static string[] ToNotEmptyLines(this string value)
        {
            return value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] TrimStringArray(this IEnumerable<string> array)
        {
            return array.Select(item => item.Trim()).ToArray();
        }
    }
}
