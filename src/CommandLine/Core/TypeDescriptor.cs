// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using CSharpx;

namespace CommandLine.Core
{
    struct TypeDescriptor
    {
        private readonly TargetType targetType;
        private readonly Maybe<int> maxItems;
        private readonly Maybe<TypeDescriptor> nextValue;

        private TypeDescriptor(TargetType targetType, Maybe<int> maxItems, Maybe<TypeDescriptor> nextValue = null)
        {
            this.targetType = targetType;
            this.maxItems = maxItems;
            this.nextValue = nextValue;
        }

        public TargetType TargetType
        {
            get { return targetType; }
        }

        public Maybe<int> MaxItems
        {
            get { return maxItems; }
        }

        public Maybe<TypeDescriptor> NextValue
        {
            get { return this.nextValue; }
        }

        public static TypeDescriptor Create(TargetType tag, Maybe<int> maximumItems, TypeDescriptor next = default(TypeDescriptor))
        {
            if (maximumItems == null) throw new ArgumentNullException("maximumItems");

            return new TypeDescriptor(tag, maximumItems, next.ToMaybe());
        }
    }

    static class TypeDescriptorExtensions
    {
        public static TypeDescriptor WithNextValue(this TypeDescriptor descriptor, Maybe<TypeDescriptor> nextValue)
        {
            return TypeDescriptor.Create(descriptor.TargetType, descriptor.MaxItems, nextValue.MapValueOrDefault(n => n, default(TypeDescriptor)));
        }
    }
}