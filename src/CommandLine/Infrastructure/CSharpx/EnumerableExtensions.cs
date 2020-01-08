//#define CSX_ENUM_INTERNAL // Uncomment or define at build time to set accessibility to internal.
//#define CSX_REM_MAYBE_FUNC // Uncomment or define at build time to remove dependency to Maybe.cs.
//#define CSX_REM_CRYPTORAND // Uncomment or define at build time to remove dependency to CryptoRandom.cs.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using LinqEnumerable = System.Linq.Enumerable;

namespace CSharpx
{
#if !CSX_ENUM_INTERNAL
    public
#endif
    static class EnumerableExtensions
    {
#if !CSX_REM_MAYBE_FUNC
        /// <summary>
        /// Safe function that returns Just(first element) or None.
        /// </summary>
        public static Maybe<T> TryHead<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator()) {
                return e.MoveNext()
                    ? Maybe.Just(e.Current)
                    : Maybe.Nothing<T>();
            }
        }

        /// <summary>
        /// Turns an empty sequence to Nothing, otherwise Just(sequence).
        /// </summary>
        public static Maybe<IEnumerable<T>> ToMaybe<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator()) {
                return e.MoveNext()
                    ? Maybe.Just(source)
                    : Maybe.Nothing<IEnumerable<T>>();
            }
        }
#endif

        private static IEnumerable<TSource> AssertCountImpl<TSource>(IEnumerable<TSource> source,
            int count, Func<int, int, Exception> errorSelector)
        {
            var collection = source as ICollection<TSource>; // Optimization for collections
            if (collection != null)
            {
                if (collection.Count != count) {
                    throw errorSelector(collection.Count.CompareTo(count), count);
                }   
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
                if (iterations > count) {
                    throw errorSelector(1, count);
                }
                yield return element;
            }
            if (iterations != count) {
                throw errorSelector(-1, count);
            }
        }

        /// <summary>
        /// Returns the Cartesian product of two sequences by combining each element of the first set with each in the second
        /// and applying the user=define projection to the pair.
        /// </summary>
        public static IEnumerable<TResult> Cartesian<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return from element1 in first
                   from element2 in second // TODO buffer to avoid multiple enumerations
                   select resultSelector(element1, element2);
        }

        /// <summary>
        /// Prepends a single value to a sequence.
        /// </summary>
        public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return LinqEnumerable.Concat(LinqEnumerable.Repeat(value, 1), source);
        }

        /// <summary>
        /// Returns a sequence consisting of the head element and the given tail elements.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this T head, IEnumerable<T> tail)
        {
            if (tail == null) throw new ArgumentNullException(nameof(tail));

            return tail.Prepend(head);
        }

        /// <summary>
        /// Returns a sequence consisting of the head elements and the given tail element.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> head, T tail)
        {
            if (head == null) throw new ArgumentNullException(nameof(head));

            return LinqEnumerable.Concat(head, LinqEnumerable.Repeat(tail, 1));
        }

        /// <summary>
        /// Excludes <paramref name="count"/> elements from a sequence starting at a given index
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence</typeparam>
        public static IEnumerable<T> Exclude<T>(this IEnumerable<T> sequence, int startIndex, int count)
        {
            if (sequence == null) throw new ArgumentNullException(nameof(sequence));
            if (startIndex < 0) throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return ExcludeImpl(sequence, startIndex, count);
        }

        private static IEnumerable<T> ExcludeImpl<T>(IEnumerable<T> sequence, int startIndex, int count)
        {
            var index = -1;
            var endIndex = startIndex + count;
            using (var iter = sequence.GetEnumerator())
            {
                // yield the first part of the sequence
                while (iter.MoveNext() && ++index < startIndex) {
                    yield return iter.Current;
                }
                // skip the next part (up to count elements)
                while (++index < endIndex && iter.MoveNext()) {
                    continue;
                }
                // yield the remainder of the sequence
                while (iter.MoveNext()) {
                    yield return iter.Current;
                }
            }
        }

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
            return source.Select((element, index) => new KeyValuePair<int, TSource>(startIndex + index, element));
        }

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
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (count == 1 && folder1 == null
                || count == 2 && folder2 == null
                || count == 3 && folder3 == null
                || count == 4 && folder4 == null)
            {                                                // ReSharper disable NotResolvedInText
                throw new ArgumentNullException("folder");   // ReSharper restore NotResolvedInText
            }

            var elements = new T[count];
            foreach (var e in AssertCountImpl(
                source.Index(), count, OnFolderSourceSizeErrorSelector)) {
                elements[e.Key] = e.Value;
            }

            switch (count) {
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
                        ? "Sequence contains too few elements when exactly {0} {1} expected"
                        : "Sequence contains too many elements when exactly {0} {1} expected";
            return new Exception(string.Format(message, count.ToString("N0"), count == 1 ? "was" : "were"));
        }

        /// <summary>
        /// Immediately executes the given action on each element in the source sequence.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var element in source) {
                action(element);
            }
        }

        /// <summary>
        /// Returns a sequence resulting from applying a function to each 
        /// element in the source sequence and its 
        /// predecessor, with the exception of the first element which is 
        /// only returned as the predecessor of the second element.
        /// </summary>
        public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return PairwiseImpl(source, resultSelector);
        }

        private static IEnumerable<TResult> PairwiseImpl<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TSource, TResult> resultSelector)
        {
            Debug.Assert(source != null);
            Debug.Assert(resultSelector != null);

            using (var e = source.GetEnumerator()) {
                if (!e.MoveNext()) {
                    yield break;
                }

                var previous = e.Current;
                while (e.MoveNext()) {
                    yield return resultSelector(previous, e.Current);
                    previous = e.Current;
                }
            }
        }

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
            if (source == null) throw new ArgumentNullException(nameof(source));

            return ToDelimitedStringImpl(source, delimiter, (sb, e) => sb.Append(e));
        }

        static string ToDelimitedStringImpl<T>(IEnumerable<T> source, string delimiter, Func<StringBuilder, T, StringBuilder> append)
        {
            Debug.Assert(source != null);
            Debug.Assert(append != null);

            delimiter = delimiter ?? CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var sb = new StringBuilder();
            var i = 0;

            foreach (var value in source) {
                if (i++ > 0) sb.Append(delimiter);
                append(sb, value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Return everything except first element and throws exception if empty.
        /// </summary>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator()) {
                if (e.MoveNext()) {
                    while (e.MoveNext()) {
                        yield return e.Current;
                    }
                }
                else {
                    throw new ArgumentException("Source sequence cannot be empty", nameof(source));
                }
            }
        }

        /// <summary>
        /// Return everything except first element without throwing exception if empty.
        /// </summary>
        public static IEnumerable<T> TailNoFail<T>(this IEnumerable<T> source)
        {
            using (var e = source.GetEnumerator())
            {
                if (e.MoveNext()) {
                    while (e.MoveNext()) {
                        yield return e.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Captures current state of a sequence.
        /// </summary>
        public static IEnumerable<T> Memoize<T>(this IEnumerable<T> source)
        {
            return source.GetType().IsArray ? source : source.ToArray();
        }

        /// <summary>
        /// Creates an immutable copy of a sequence.
        /// </summary>
        public static IEnumerable<T> Materialize<T>(this IEnumerable<T> source)
        {
            if (source is MaterializedEnumerable<T> || source.GetType().IsArray) {
                return source;
            }
            return new MaterializedEnumerable<T>(source);
        }

        private class MaterializedEnumerable<T> : IEnumerable<T>
        {
            private readonly ICollection<T> inner;

            public MaterializedEnumerable(IEnumerable<T> enumerable)
            {
                inner = enumerable as ICollection<T> ?? enumerable.ToArray();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return inner.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Selects a random element.
        /// </summary>
        public static T Choice<T>(this IEnumerable<T> source)
        {
#if CSX_REM_CRYPTORAND
            var index = new Random().Next(source.Count() - 1);
#else
            var index = new CryptoRandom().Next(source.Count() - 1);
#endif
            return source.ElementAt(index);
        }

        /// <summary>
        /// Takes an element and a sequence and `intersperses' that element between its elements.
        /// </summary>
        public static IEnumerable<T> Intersperse<T>(this IEnumerable<T> source, T element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            var count = source.Count();
            var last = count - 1;
            for (var i = 0; i < count; i++) {
                yield return source.ElementAt(i);
                if (i != last) {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Flattens a sequence by one level.
        /// </summary>
        public static IEnumerable<T> FlattenOnce<T>(this IEnumerable<IEnumerable<T>> source)
        {
            foreach (var element in source) {
                foreach (var subelement in element) {
                    yield return subelement;
                }
            }
        }

        /// <summary>
        /// Reduces a sequence of strings to a sequence of parts, splitted by space,
        /// of each original string.
        /// </summary>
        public static IEnumerable<string> FlattenOnce(this IEnumerable<string> source)
        {
            foreach (var element in source) {
                var parts = element.Split();
                foreach (var part in parts) {
                    yield return part;
                }
            }
        }
    }
}