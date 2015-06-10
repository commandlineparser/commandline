// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class ValueSpecification : Specification
    {
        private readonly int index;

        public ValueSpecification(int index, bool required, int min, int max, string separator, Maybe<object> defaultValue, System.Type conversionType)
            : base(SpecificationType.Value, required, min, max, separator, defaultValue, conversionType)
        {
            this.index = index;
        }

        public static ValueSpecification FromAttribute(ValueAttribute attribute, System.Type conversionType)
        {
            return new ValueSpecification(
                attribute.Index,
                attribute.Required,
                attribute.Min,
                attribute.Max,
                attribute.Separator,
                attribute.DefaultValue.ToMaybe(),
                conversionType);
        }

        public int Index
        {
            get { return this.index; }
        }
    }
}