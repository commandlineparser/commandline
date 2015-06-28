// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal sealed class TypeDescriptor
    {
        private readonly TargetType tag;
        private readonly Maybe<int> maxItems;

        private TypeDescriptor(TargetType tag, Maybe<int> maxItems)
        {
            this.tag = tag;
            this.maxItems = maxItems;
        }

        public TargetType Tag
        {
            get { return tag; }
        }

        public Maybe<int> MaxItems
        {
            get { return this.maxItems; }
        }

        public static TypeDescriptor Create(TargetType tag, Maybe<int> maximumItems)
        {
            if (maximumItems == null) throw new ArgumentNullException("maximumItems");

            return new TypeDescriptor(tag, maximumItems);
        }
    }
}