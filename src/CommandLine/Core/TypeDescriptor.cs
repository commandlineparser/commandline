// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    sealed class TypeDescriptor
    {
        private readonly TargetType targetType;
        private readonly Maybe<int> maxItems;
        private readonly Maybe<TypeDescriptor> next;

        private TypeDescriptor(TargetType targetType, Maybe<int> maxItems, Maybe<TypeDescriptor> next = null)
        {
            this.targetType = targetType;
            this.maxItems = maxItems;
            this.next = next;
        }

        public TargetType TargetType
        {
            get { return targetType; }
        }

        public Maybe<int> MaxItems
        {
            get { return maxItems; }
        }

        public Maybe<TypeDescriptor> Next
        {
            get { return next; }
        }

        public static TypeDescriptor Create(TargetType tag, Maybe<int> maximumItems, TypeDescriptor next = null)
        {
            if (maximumItems == null) throw new ArgumentNullException("maximumItems");

            return new TypeDescriptor(tag, maximumItems, next.ToMaybe());
        }
    }

    static class TypeDescriptorExtensions
    {
        public static TypeDescriptor WithNext(this TypeDescriptor descriptor, Maybe<TypeDescriptor> next)
        {
            return TypeDescriptor.Create(descriptor.TargetType, descriptor.MaxItems, next.Return(n => n, null));
        }
    }
}