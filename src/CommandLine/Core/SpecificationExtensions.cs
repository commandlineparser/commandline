// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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
            if (specification == null) throw new ArgumentNullException("specification");

            return specification.Tag == SpecificationType.Option;
        }

        public static bool IsValue(this Specification specification)
        {
            if (specification == null) throw new ArgumentNullException("specification");

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
                specification.DefaultValue,
                specification.ConversionType,
                specification.HelpText,
                specification.MetaValue);
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
            if (specification == null) throw new ArgumentNullException("specification");

            switch (specification.ConversionType.ToDescriptor())
            {
                case DescriptorType.Scalar:
                    return  Maybe.Just(1);
                case DescriptorType.Sequence:
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
    }
}
