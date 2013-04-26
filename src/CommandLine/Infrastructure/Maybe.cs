// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

namespace CommandLine.Infrastructure
{
    internal enum MaybeType { Just, Nothing }

    /// <summary>
    /// C# implementation of F# 'T option. Based on the one found in http://goo.gl/jMwzg.
    /// Here Haskell naming is used, to not create confusion with local option concept.
    /// </summary>
    internal abstract class Maybe<T>
    {
        private readonly MaybeType tag;

        protected Maybe(MaybeType tag)
        {
            this.tag = tag;
        }

        public MaybeType Tag
        {
            get { return this.tag; }
        }

        public bool MatchNothing()
        {
            return this.Tag == MaybeType.Nothing;
        }

        public bool MatchJust(out T value)
        {
            value = this.Tag == MaybeType.Just
                ? ((Just<T>)this).Value
                : default(T);
            return this.Tag == MaybeType.Just;
        }
    }

    internal sealed class Nothing<T> : Maybe<T>
    {
        public Nothing() : base(MaybeType.Nothing) { }
    }

    internal sealed class Just<T> : Maybe<T>
    {
        private readonly T value;

        public Just(T value)
            : base(MaybeType.Just)
        {
            this.value = value;
        }

        public T Value
        {
            get
            {
                return this.value;
            }
        }
    }

    internal static class Maybe
    {
        public static Maybe<T> Nothing<T>()
        {
            return new Nothing<T>();
        }

        public static Just<T> Just<T>(T value)
        {
            return new Just<T>(value);
        }
    }

    internal static class MaybeExtensions
    {
        /// <summary>
        /// Equivalent to Unit operation.
        /// </summary>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return object.Equals(value, default(T)) ? Maybe.Nothing<T>() : Maybe.Just(value);
        }

        public static Maybe<T2> Bind<T1, T2>(this Maybe<T1> maybe, Func<T1, Maybe<T2>> func)
        {
            T1 value1;
            return maybe.MatchJust(out value1)
                ? func(value1)
                : Maybe.Nothing<T2>();
        }

        public static Maybe<T2> Map<T1, T2>(this Maybe<T1> maybe, Func<T1, T2> func)
        {
            T1 value1;
            return maybe.MatchJust(out value1)
                ? Maybe.Just(func(value1))
                : Maybe.Nothing<T2>();
        }

        public static T2 Return<T1, T2>(this Maybe<T1> maybe, Func<T1, T2> func, T2 noneValue)
        {
            T1 value1;
            return maybe.MatchJust(out value1)
                ? func(value1)
                : noneValue;
        }

        public static void Do<T>(this Maybe<T> maybe, Action<T> action)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                action(value);
            }
        }

        public static Maybe<TResult> Select<TSource, TResult>(
            this Maybe<TSource> maybe, Func<TSource, TResult> selector)
        {
            return maybe.Map(selector);
        }

        public static Maybe<TResult> SelectMany<TSource, TValue, TResult>(
            this Maybe<TSource> maybe,
            Func<TSource, Maybe<TValue>> valueSelector,
            Func<TSource, TValue, TResult> resultSelector)
        {
            return maybe.Bind(
                sourceValue => valueSelector(sourceValue)
                    .Map(
                        resultValue => resultSelector(sourceValue, resultValue)));
        }

        public static T FromJust<T>(this Maybe<T> maybe, Exception exceptionToThrow = null)
        {
            T value;
            if (maybe.MatchJust(out value))
            {
                return value;
            }
            throw exceptionToThrow ?? new ArgumentException("Value empty.");
        }

        public static bool IsNothing<T>(this Maybe<T> maybe)
        {
            return maybe.Tag == MaybeType.Nothing;
        }

        public static bool IsJust<T>(this Maybe<T> maybe)
        {
            return maybe.Tag == MaybeType.Just;
        }
    }
}