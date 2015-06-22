// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    static class SpecificationExtensions
    {
        public static bool IsOption(this Specification specification)
        {
            return specification.Tag == SpecificationType.Option;
        }

        public static bool IsValue(this Specification specification)
        {
            return specification.Tag == SpecificationType.Value;
        }

        public static OptionSpecification WithLongName(this OptionSpecification specification, string newLongName)
        {
            return new OptionSpecification(
                specification.ShortName,
                newLongName,
                specification.Required,
                specification.SetName,
                specification.Min,
                specification.Max,
                specification.Separator,
                specification.DefaultValue,
                specification.ConversionType,
                specification.HelpText,
                specification.MetaValue,
                specification.EnumValues);
        }

        public static IEnumerable<Specification> ThrowingValidate(this IEnumerable<Specification> specifications, IEnumerable<Tuple<Func<Specification, bool>, string>> guardsLookup)
        {
            foreach (var guard in guardsLookup)
            {
                if (specifications.Any(spec => guard.Item1(spec)))
                {
                    throw new ApplicationException(guard.Item2);
                }
            }

            return specifications;
        }

        public static Maybe<int> GetMaxValueCount(this Specification specification)
        {
            switch (specification.ConversionType.ToDescriptorKind())
            {
                case TypeDescriptorKind.Scalar:
                    return  Maybe.Just(1);
                case TypeDescriptorKind.Sequence:
                    var min = specification.Min;
                    var max = specification.Max;
                    if (min >= 0 && max >= 0)
                    {
                        return Maybe.Just(max);
                    }
                    break;
            }

            return Maybe.Nothing<int>();
        }

        public static bool IsMinNotSpecified(this Specification specification)
        {
            return specification.Min == -1;
        }

        public static bool IsMaxNotSpecified(this Specification specification)
        {
            return specification.Max == -1;
        }

        public static string GetSetName(this Specification specification)
        {
            return specification.IsOption()
                ? ((OptionSpecification)specification).SetName
                : string.Empty;
        }
    }
}
