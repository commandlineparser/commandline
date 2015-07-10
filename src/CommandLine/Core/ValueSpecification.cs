// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;

using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class ValueSpecification : Specification
    {
        private readonly int index;

        public ValueSpecification(int index, bool required, Maybe<int> min, Maybe<int> max, Maybe<object> defaultValue,
            string helpText, string metaValue, IEnumerable<string> enumValues,
            Type conversionType, TargetType targetType)
            : base(SpecificationType.Value, required, min, max, defaultValue, helpText, metaValue, enumValues, conversionType, targetType)
        {
            this.index = index;
        }

        public static ValueSpecification FromAttribute(ValueAttribute attribute, Type conversionType, IEnumerable<string> enumValues)
        {
            return new ValueSpecification(
                attribute.Index,
                attribute.Required,
                attribute.Min == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Min),
                attribute.Max == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Max),
                attribute.Default.ToMaybe(),
                attribute.HelpText,
                attribute.MetaValue,
                enumValues,
                conversionType,
                conversionType.ToTargetType());
        }

        public int Index
        {
            get { return index; }
        }
    }
}