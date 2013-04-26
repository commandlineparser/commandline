// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine.Core
{
    static class SpecificationGuards
    {
        public static readonly IEnumerable<Tuple<Func<Specification, bool>, string>> Lookup = new List<Tuple<Func<Specification, bool>, string>>
            {
                Tuple.Create(GuardAgainstScalarWithRange(), "Scalar option specifications do not support range specification."),
                Tuple.Create(GuardAgainstSequenceWithWrongRange(), "Bad range in sequence option specifications."),
                Tuple.Create(GuardAgainstOneCharLongName(), "Long name should be longest than one character.")
            };

        private static Func<Specification, bool> GuardAgainstScalarWithRange()
        {
            return spec => spec.ConversionType.ToDescriptor() == DescriptorType.Scalar && (spec.Min > 0 || spec.Max > 0);
        }

        private static Func<Specification, bool> GuardAgainstSequenceWithWrongRange()
        {
            return spec => spec.ConversionType.ToDescriptor() == DescriptorType.Sequence && spec.Min > spec.Max;
        }

        private static Func<Specification, bool> GuardAgainstOneCharLongName()
        {
            return spec => spec.IsOption() && ((OptionSpecification)spec).LongName.Length == 1;
        }
    }
}
