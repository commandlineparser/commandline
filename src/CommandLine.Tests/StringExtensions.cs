// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Linq;

namespace CommandLine.Tests
{
    static class StringExtensions
    {
        public static string[] ToNotEmptyLines(this string value)
        {
            return value.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] TrimStringArray(this string[] array)
        {
            return array.Select(item => item.Trim()).ToArray();
        }
    }
}
