// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class ValueSpecification : Specification
    {
        private readonly int index;

        public ValueSpecification(int index, bool required, Maybe<int> min, Maybe<int> max, Maybe<object> defaultValue,
            Type conversionType, TargetType targetType)
            : base(SpecificationType.Value, required, min, max, defaultValue, conversionType, targetType)
        {
            this.index = index;
        }

        public static ValueSpecification FromAttribute(ValueAttribute attribute, Type conversionType)
        {
            return new ValueSpecification(
                attribute.Index,
                attribute.Required,
                attribute.Min == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Min),
                attribute.Max == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Max),
                attribute.DefaultValue.ToMaybe(),
                conversionType,
                conversionType.ToTargetType());
        }

        public int Index
        {
            get { return index; }
        }
    }
}