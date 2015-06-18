// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal enum DescriptorType
    {
        Scalar,
        Boolean,
        Sequence
    }

    internal sealed class TypeDescriptor
    {
        private readonly DescriptorType tag;
        private readonly Maybe<int> maximumItems;

        internal TypeDescriptor(DescriptorType tag, Maybe<int> maximumItems)
        {
            if (maximumItems == null) throw new ArgumentNullException("maximumItems");

            this.tag = tag;
            this.maximumItems = maximumItems;
        }

        public DescriptorType Tag
        {
            get { return this.tag; }
        }

        public Maybe<int> MaximumItems
        {
            get { return this.maximumItems; }
        }
    }
}