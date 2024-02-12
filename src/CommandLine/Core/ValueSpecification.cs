// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CSharpx;

namespace CommandLine.Core
{
    sealed class ValueSpecification : Specification
    {
        private readonly int index;
        private readonly string metaName;

        public ValueSpecification(
            int index,
            string metaName,
            bool required,
            Maybe<int> min,
            Maybe<int> max,
            Maybe<object> defaultValue,
            string helpText,
            string metaValue,
            IEnumerable<string> enumValues,
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
            Type conversionType,
            TargetType targetType,
            bool hidden = false)
            : base(SpecificationType.Value, required, min, max, defaultValue, helpText, metaValue, enumValues,
                conversionType, targetType, hidden)
        {
            this.index = index;
            this.metaName = metaName;
        }

        public static ValueSpecification FromAttribute(
            ValueAttribute attribute,
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
            Type conversionType,
            IEnumerable<string> enumValues)
        {
            return new ValueSpecification(
                attribute.Index,
                attribute.MetaName,
                attribute.Required,
                attribute.Min == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Min),
                attribute.Max == -1 ? Maybe.Nothing<int>() : Maybe.Just(attribute.Max),
                attribute.Default.ToMaybe(),
                attribute.HelpText,
                attribute.MetaValue,
                enumValues,
                conversionType,
                conversionType.ToTargetType(),
                attribute.Hidden);
        }

        public int Index
        {
            get { return index; }
        }

        public string MetaName
        {
            get { return metaName; }
        }
    }
}
