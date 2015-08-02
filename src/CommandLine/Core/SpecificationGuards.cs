// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using CSharpx;

namespace CommandLine.Core
{
    static class SpecificationGuards
    {
        public static readonly IEnumerable<Tuple<Func<Specification, bool>, string>> Lookup = new List<Tuple<Func<Specification, bool>, string>>
            {
                Tuple.Create(GuardAgainstScalarWithRange(), "Scalar option specifications do not support range specification."),
                Tuple.Create(GuardAgainstSequenceWithWrongRange(), "Bad range in sequence option specifications."),
                Tuple.Create(GuardAgainstSequenceWithZeroRange(), "Zero is not allowed in range of sequence option specifications."),
                Tuple.Create(GuardAgainstOneCharLongName(), "Long name should be longest than one character.")
            };

        private static Func<Specification, bool> GuardAgainstScalarWithRange()
        {
            return spec => spec.TargetType == TargetType.Scalar
                && (spec.Min.IsJust() || spec.Max.IsJust());
        }

        private static Func<Specification, bool> GuardAgainstSequenceWithWrongRange()
        {
            return spec => spec.TargetType == TargetType.Sequence
                && spec.HavingRange((min, max) => min > max);
        }

        private static Func<Specification, bool> GuardAgainstOneCharLongName()
        {
            return spec => spec.IsOption() && ((OptionSpecification)spec).LongName.Length == 1;
        }

        private static Func<Specification, bool> GuardAgainstSequenceWithZeroRange()
        {
            return spec => spec.TargetType == TargetType.Sequence
                && (spec.HavingMin(min => min == 0)
                || spec.HavingMax(max => max == 0));
        }
    }
}
