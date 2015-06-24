// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal enum TypeDescriptorKind
    {
        Scalar,
        Boolean,
        Sequence
    }

    internal sealed class TypeDescriptor
    {
        private readonly TypeDescriptorKind tag;
        private readonly Maybe<int> maximumItems;

        private TypeDescriptor(TypeDescriptorKind tag, Maybe<int> maximumItems)
        {
            this.tag = tag;
            this.maximumItems = maximumItems;
        }

        public TypeDescriptorKind Tag
        {
            get { return tag; }
        }

        public Maybe<int> MaximumItems
        {
            get { return maximumItems; }
        }

        public static TypeDescriptor Create(TypeDescriptorKind tag, Maybe<int> maximumItems)
        {
            if (maximumItems == null) throw new ArgumentNullException("maximumItems");

            return new TypeDescriptor(tag, maximumItems);
        }
    }
}