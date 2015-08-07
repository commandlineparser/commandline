// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine.Infrastructure
{
    static class PopsicleSetter
    {
        public static void Set<T>(bool consumed, ref T field, T value)
        {
            if (consumed)
            {
                throw new InvalidOperationException();
            }

            field = value;
        }
    }
}