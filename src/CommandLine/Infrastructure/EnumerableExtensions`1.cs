// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Infrastructure
{
    static class EnumerableExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var index = -1;
            foreach (var item in source)
            {
                index++;
                if (predicate(item))
                {
                    break;
                }
            }
            return index;
        }

        public static object ToUntypedArray(this IEnumerable<object> value, Type type)
        {
            var array = Array.CreateInstance(type, value.Count());
            value.ToArray().CopyTo(array, 0);
            return array;
        }

        public static bool Empty<TSource>(this IEnumerable<TSource> source)
        {
            return !source.Any();
        }
    }
}