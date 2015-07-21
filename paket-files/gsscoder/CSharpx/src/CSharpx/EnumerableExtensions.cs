// Most methods taken from code.google.com/p/morelinq/.

#define CSHARPX_PUBLIC // Comment this to set visibility to internal.
#define CSHARPX_CARTESIAN // Comment this to remove Cartesian method.
#define CSHARPX_PREPEND // Comment this to remove Prepend method.
#define CSHARPX_CONCAT // Comment this to remove Concat methods (depend on Prepend).
#define CSHARPX_EXCLUDE // Comment this to remove Prepend method.
#define CSHARPX_INDEX // Comment this to remove Index methods.
#define CSHARPX_FOLD // Comment this to remove Fold methods (depend on Index).
#define CSHARPX_FOREACH // Comment this to remove ForEach method.
#define CSHARPX_PAIRWISE // Comment this to remove Index method.
#define CSHARPX_TODELIMITEDSTRING // Comment this to remove ToDelimitedString methods.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using LinqEnumerable = System.Linq.Enumerable;

namespace CSharpx
{
#if CSHARPX_PUBLIC
    public
#endif
    static partial class EnumerableExtensions
    {
        private static IEnumerable<TSource> AssertCountImpl<TSource>(IEnumerable<TSource> source,
            int count, Func<int, int, Exception> errorSelector)
        {
            var collection = source as ICollection<TSource>; // Optimization for collections
            if (collection != null)
            {
                if (collection.Count != count)
                    throw errorSelector(collection.Count.CompareTo(count), count);
                return source;
            }

            return ExpectingCountYieldingImpl(source, count, errorSelector);
        }

        private static IEnumerable<TSource> ExpectingCountYieldingImpl<TSource>(IEnumerable<TSource> source,
            int count, Func<int, int, Exception> errorSelector)
        {
            var iterations = 0;
            foreach (var element in source)
            {
                iterations++;
                if (iterations > count)
                {
                    throw errorSelector(1, count);
                }
                yield return element;
            }
            if (iterations != count)
            {
                throw errorSelector(-1, count);
            }
        }

#if CSHARPX_CARTESIAN
        /// <summary>
        /// Returns the Cartesian product of two sequences by combining each element of the first set with each in the second
        /// and applying the user=define projection to the pair.
        /// </summary>
        public static IEnumerable<TResult> Cartesian<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");

            return from item1 in first
                   from item2 in second // TODO buffer to avoid multiple enumerations
                   select resultSelector(item1, item2);
        }
#endif

#if CSHARPX_PREPEND
        /// <summary>
        /// Prepends a single value to a sequence.
        /// </summary>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            if (source == null) throw new ArgumentNullException("source");
            return LinqEnumerable.Concat(LinqEnumerable.Repeat(value, 1), source);
        }
#endif

#if CSHARPX_CONCAT
        /// <summary>
        /// Returns a sequence consisting of the head element and the given tail elements.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
        {
            if (tail == null) throw new ArgumentNullException("tail");
            return tail.Prepend(head);
        }

        /// <summary>
        /// Returns a sequence consisting of the head elements and the given tail element.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, T tail)
        {
            if (head == null) throw new ArgumentNullException("head");
            return LinqEnumerable.Concat(head, LinqEnumerable.Repeat(tail, 1));
        }
#endif

#if CSHARPX_EXCLUDE
        /// <summary>
        /// Excludes <paramref name="count"/> elements from a sequence starting at a given index
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> sequence, int startIndex, int count)
        {
            if (sequence == null) throw new ArgumentNullException("sequence");
            if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
            if (count < 0) throw new ArgumentOutOfRangeException("count");

            return ExcludeImpl(sequence, startIndex, count);
        }

        private static IEnumerable<T> ExcludeImpl<T>(IEnumerable<T> sequence, int startIndex, int count)
        {
            var index = -1;
            var endIndex = startIndex + count;
            using (var iter = sequence.GetEnumerator())
            {
                // yield the first part of the sequence
                while (iter.MoveNext() && ++index < startIndex)
                    yield return iter.Current;
                // skip the next part (up to count items)
                while (++index < endIndex && iter.MoveNext())
                    continue;
                // yield the remainder of the sequence
                while (iter.MoveNext())
                    yield return iter.Current;
            }
        }
#endif

#if CSHARPX_INDEX
        /// <summary>
        /// Returns a sequence of <see cref="KeyValuePair{TKey,TValue}"/> 
        /// where the key is the zero-based index of the value in the source 
        /// sequence.
        /// </summary>
        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source)
        {
            return source.Index(0);
        }

        /// <summary>
        /// Returns a sequence of <see cref="KeyValuePair{TKey,TValue}"/> 
        /// where the key is the index of the value in the source sequence.
        /// An additional parameter specifies the starting index.
        /// </summary>
        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source, int startIndex)
        {
            return source.Select((item, index) => new KeyValuePair<int, TSource>(startIndex + index, item));
        }
#endif

#if CSHARPX_FOLD
        /// <summary>
        /// Returns the result of applying a function to a sequence of 
        /// 1 element.
        /// </summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, TResult> folder)
        {
            return FoldImpl(source, 1, folder, null, null, null);
        }

        /// <summary>
        /// Returns the result of applying a function to a sequence of 
        /// 2 elements.
        /// </summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, TResult> folder)
        {
            return FoldImpl(source, 2, null, folder, null, null);
        }

        /// <summary>
        /// Returns the result of applying a function to a sequence of 
        /// 3 elements.
        /// </summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, T, TResult> folder)
        {
            return FoldImpl(source, 3, null, null, folder, null);
        }

        /// <summary>
        /// Returns the result of applying a function to a sequence of 
        /// 4 elements.
        /// </summary>
        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, T, T, TResult> folder)
        {
            return FoldImpl(source, 4, null, null, null, folder);
        }

        static TResult FoldImpl<T, TResult>(IEnumerable<T> source, int count,
            Func<T, TResult> folder1,
            Func<T, T, TResult> folder2,
            Func<T, T, T, TResult> folder3,
            Func<T, T, T, T, TResult> folder4)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count == 1 && folder1 == null
                || count == 2 && folder2 == null
                || count == 3 && folder3 == null
                || count == 4 && folder4 == null)
            {                                                // ReSharper disable NotResolvedInText
                throw new ArgumentNullException("folder");   // ReSharper restore NotResolvedInText
            }

            var elements = new T[count];
            foreach (var e in AssertCountImpl(source.Index(), count, OnFolderSourceSizeErrorSelector))
                elements[e.Key] = e.Value;

            switch (count)
            {
                case 1: return folder1(elements[0]);
                case 2: return folder2(elements[0], elements[1]);
                case 3: return folder3(elements[0], elements[1], elements[2]);
                case 4: return folder4(elements[0], elements[1], elements[2], elements[3]);
                default: throw new NotSupportedException();
            }
        }

        static readonly Func<int, int, Exception> OnFolderSourceSizeErrorSelector = OnFolderSourceSizeError;

        static Exception OnFolderSourceSizeError(int cmp, int count)
        {
            var message = cmp < 0
                        ? "Sequence contains too few elements when exactly {0} {1} expected."
                        : "Sequence contains too many elements when exactly {0} {1} expected.";
            return new Exception(string.Format(message, count.ToString("N0"), count == 1 ? "was" : "were"));
        }
#endif

#if CSHARPX_FOREACH
        /// <summary>
        /// Immediately executes the given action on each element in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (var element in source)
            {
                action(element);
            }
        }
#endif

#if CSHARPX_PAIRWISE
        /// <summary>
        /// Returns a sequence resulting from applying a function to each 
        /// element in the source sequence and its 
        /// predecessor, with the exception of the first element which is 
        /// only returned as the predecessor of the second element.
        /// </summary>
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (resultSelector == null) throw new ArgumentNullException("resultSelector");
            return PairwiseImpl(source, resultSelector);
        }

        private static IEnumerable<TResult> PairwiseImpl<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            Debug.Assert(source != null);
            Debug.Assert(resultSelector != null);

            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    yield break;

                var previous = e.Current;
                while (e.MoveNext())
                {
                    yield return resultSelector(previous, e.Current);
                    previous = e.Current;
                }
            }
        }
#endif

#if CSHARPX_TODELIMITEDSTRING
        /// <summary>
        /// Creates a delimited string from a sequence of values. The 
        /// delimiter used depends on the current culture of the executing thread.
        /// </summary>
        public static string ToDelimitedString<TSource>(this IEnumerable<TSource> source)
        {
            return ToDelimitedString(source, null);
        }

        /// <summary>
        /// Creates a delimited string from a sequence of values and
        /// a given delimiter.
        /// </summary>
        public static string ToDelimitedString<TSource>(this IEnumerable<TSource> source, string delimiter)
        {
            if (source == null) throw new ArgumentNullException("source");
            return ToDelimitedStringImpl(source, delimiter, (sb, e) => sb.Append(e));
        }

        static string ToDelimitedStringImpl<T>(IEnumerable<T> source, string delimiter, Func<StringBuilder, T, StringBuilder> append)
        {
            Debug.Assert(source != null);
            Debug.Assert(append != null);

            delimiter = delimiter ?? CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var sb = new StringBuilder();
            var i = 0;

            foreach (var value in source)
            {
                if (i++ > 0) sb.Append(delimiter);
                append(sb, value);
            }

            return sb.ToString();
        }
#endif
    }
}
