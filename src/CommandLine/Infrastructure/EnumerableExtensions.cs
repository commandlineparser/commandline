// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Infrastructure
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return source.PairwiseImpl(selector);
        }

        private static IEnumerable<TResult> PairwiseImpl<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> selector)
        {
            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    var left = enumerator.Current;

                    while (enumerator.MoveNext())
                    {
                        var right = enumerator.Current;
                        yield return selector(left, right);
                        left = right;
                    }
                }
            }
        }

        //public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (action == null) throw new ArgumentNullException("action");

        //    foreach (var item in source)
        //    {
        //        action(item);
        //    }
        //}

        //public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    if (action == null) throw new ArgumentNullException("action");

        //    var index = 0;

        //    foreach (var item in source)
        //    {
        //        action(item, index++);
        //    }
        //}


        public static IEnumerable<T> ToEnumerable<T>(this List<T> value)
        {
            return value;
        }

        public static object ToArray(this IEnumerable<object> value, Type type)
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