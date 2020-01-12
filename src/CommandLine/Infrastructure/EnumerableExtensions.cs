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

        /// <summary>
        /// Breaks a collection into groups of a specified size.
        /// </summary>
        /// <param name="source">A collection of <typeparam name="T"/>.</param>
        /// <param name="groupSize">The number of items each group shall contain.</param>
        /// <returns>An enumeration of T[].</returns>
        /// <remarks>An incomplete group at the end of the source collection will be silently dropped.</remarks>
        public static IEnumerable<T[]> Group<T>(this IEnumerable<T> source, int groupSize)
        {
            if (groupSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(groupSize));
            }

            T[] group = new T[groupSize];
            int groupIndex = 0;

            foreach (var item in source)
            {
                group[groupIndex++] = item;

                if (groupIndex == groupSize)
                {
                    yield return group;

                    group = new T[groupSize];
                    groupIndex = 0;
                }
            }
        }
    }
}