// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Core
{
    static class NameExtensions
    {
        public static bool MatchName(this string value, string shortName, string longName, StringComparer comparer)
        {
            return value.Length == 1
               ? comparer.Equals(value, shortName)
               : comparer.Equals(value, longName);
        }

        public static NameInfo FromOptionSpecification(this OptionSpecification specification)
        {
            return new NameInfo(
                specification.ShortName,
                specification.LongName);
        }

        public static NameInfo FromSpecification(this Specification specification)
        {
            switch (specification.Tag)
            {
                case SpecificationType.Option:
                    return FromOptionSpecification((OptionSpecification)specification);
                default:
                    return NameInfo.EmptyName;
            }
        }
    }
}
