// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine
{
    /// <summary>
    /// Provides convenience extension methods for <see cref="CommandLine.ParserResult{T}"/>.
    /// </summary>
    public static partial class ParserResultExtensions
    {
        /// <summary>
        /// Executes <paramref name="action"/> if <see cref="CommandLine.ParserResult{T}"/> contains
        /// parsed values.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <returns>The same <paramref name="result"/> instance.</returns>
        public static ParserResult<T> WithParsed<T>(this ParserResult<T> result, Action<T> action)
        {
            var parsed = result as Parsed<T>;
            if (parsed != null)
            {
                action(parsed.Value);
            }
            return result;
        }

        /// <summary>
        /// Executes <paramref name="action"/> if parsed values are of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An verb result instance.</param>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <returns>The same <paramref name="result"/> instance.</returns>
        public static ParserResult<object> WithParsed<T>(this ParserResult<object> result, Action<T> action)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T)
                {
                    action((T)parsed.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// Executes <paramref name="action"/> if <see cref="CommandLine.ParserResult{T}"/> lacks
        /// parsed values and contains errors.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="System.Action"/> delegate to execute.</param>
        /// <returns>The same <paramref name="result"/> instance.</returns>
        public static ParserResult<T> WithNotParsed<T>(this ParserResult<T> result, Action<IEnumerable<Error>> action)
        {
            var notParsed = result as NotParsed<T>;
            if (notParsed != null)
            {
                action(notParsed.Errors);
            }
            return result;
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="TSource">Type of the target instance built with parsed value.</typeparam>
        /// <typeparam name="TResult">The type of the new value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="parsedFunc">Lambda executed on successful parsing.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<TSource, TResult>(this ParserResult<TSource> result,
            Func<TSource, TResult> parsedFunc,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<TSource>;
            if (parsed != null)
            {
                return parsedFunc(parsed.Value);
            }
            return notParsedFunc(((NotParsed<TSource>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="T12">Twelfth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="parsedFunc12">Lambda executed on successful parsing of <typeparamref name="T12"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<T12, TResult> parsedFunc12,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                if (parsed.Value is T12)
                {
                    return parsedFunc12((T12)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="T12">Twelfth verb type.</typeparam>
        /// <typeparam name="T13">Thirteenth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="parsedFunc12">Lambda executed on successful parsing of <typeparamref name="T12"/>.</param>
        /// <param name="parsedFunc13">Lambda executed on successful parsing of <typeparamref name="T13"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<T12, TResult> parsedFunc12,
            Func<T13, TResult> parsedFunc13,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                if (parsed.Value is T12)
                {
                    return parsedFunc12((T12)parsed.Value);
                }
                if (parsed.Value is T13)
                {
                    return parsedFunc13((T13)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="T12">Twelfth verb type.</typeparam>
        /// <typeparam name="T13">Thirteenth verb type.</typeparam>
        /// <typeparam name="T14">Fourteenth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="parsedFunc12">Lambda executed on successful parsing of <typeparamref name="T12"/>.</param>
        /// <param name="parsedFunc13">Lambda executed on successful parsing of <typeparamref name="T13"/>.</param>
        /// <param name="parsedFunc14">Lambda executed on successful parsing of <typeparamref name="T14"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<T12, TResult> parsedFunc12,
            Func<T13, TResult> parsedFunc13,
            Func<T14, TResult> parsedFunc14,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                if (parsed.Value is T12)
                {
                    return parsedFunc12((T12)parsed.Value);
                }
                if (parsed.Value is T13)
                {
                    return parsedFunc13((T13)parsed.Value);
                }
                if (parsed.Value is T14)
                {
                    return parsedFunc14((T14)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="T12">Twelfth verb type.</typeparam>
        /// <typeparam name="T13">Thirteenth verb type.</typeparam>
        /// <typeparam name="T14">Fourteenth verb type.</typeparam>
        /// <typeparam name="T15">Fifteenth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="parsedFunc12">Lambda executed on successful parsing of <typeparamref name="T12"/>.</param>
        /// <param name="parsedFunc13">Lambda executed on successful parsing of <typeparamref name="T13"/>.</param>
        /// <param name="parsedFunc14">Lambda executed on successful parsing of <typeparamref name="T14"/>.</param>
        /// <param name="parsedFunc15">Lambda executed on successful parsing of <typeparamref name="T15"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<T12, TResult> parsedFunc12,
            Func<T13, TResult> parsedFunc13,
            Func<T14, TResult> parsedFunc14,
            Func<T15, TResult> parsedFunc15,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                if (parsed.Value is T12)
                {
                    return parsedFunc12((T12)parsed.Value);
                }
                if (parsed.Value is T13)
                {
                    return parsedFunc13((T13)parsed.Value);
                }
                if (parsed.Value is T14)
                {
                    return parsedFunc14((T14)parsed.Value);
                }
                if (parsed.Value is T15)
                {
                    return parsedFunc15((T15)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }

        /// <summary>
        /// Provides a way to transform result data into another value.
        /// </summary>
        /// <typeparam name="T1">First verb type.</typeparam>
        /// <typeparam name="T2">Second verb type.</typeparam>
        /// <typeparam name="T3">Third verb type.</typeparam>
        /// <typeparam name="T4">Fourth verb type.</typeparam>
        /// <typeparam name="T5">Fifth verb type.</typeparam>
        /// <typeparam name="T6">Sixth verb type.</typeparam>
        /// <typeparam name="T7">Seventh verb type.</typeparam>
        /// <typeparam name="T8">Eighth verb type.</typeparam>
        /// <typeparam name="T9">Ninth verb type.</typeparam>
        /// <typeparam name="T10">Tenth verb type.</typeparam>
        /// <typeparam name="T11">Eleventh verb type.</typeparam>
        /// <typeparam name="T12">Twelfth verb type.</typeparam>
        /// <typeparam name="T13">Thirteenth verb type.</typeparam>
        /// <typeparam name="T14">Fourteenth verb type.</typeparam>
        /// <typeparam name="T15">Fifteenth verb type.</typeparam>
        /// <typeparam name="T16">Sixteenth verb type.</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="result">The result in verb scenario.</param>
        /// <param name="parsedFunc1">Lambda executed on successful parsing of <typeparamref name="T1"/>.</param>
        /// <param name="parsedFunc2">Lambda executed on successful parsing of <typeparamref name="T2"/>.</param>
        /// <param name="parsedFunc3">Lambda executed on successful parsing of <typeparamref name="T3"/>.</param>
        /// <param name="parsedFunc4">Lambda executed on successful parsing of <typeparamref name="T4"/>.</param>
        /// <param name="parsedFunc5">Lambda executed on successful parsing of <typeparamref name="T5"/>.</param>
        /// <param name="parsedFunc6">Lambda executed on successful parsing of <typeparamref name="T6"/>.</param>
        /// <param name="parsedFunc7">Lambda executed on successful parsing of <typeparamref name="T7"/>.</param>
        /// <param name="parsedFunc8">Lambda executed on successful parsing of <typeparamref name="T8"/>.</param>
        /// <param name="parsedFunc9">Lambda executed on successful parsing of <typeparamref name="T9"/>.</param>
        /// <param name="parsedFunc10">Lambda executed on successful parsing of <typeparamref name="T10"/>.</param>
        /// <param name="parsedFunc11">Lambda executed on successful parsing of <typeparamref name="T11"/>.</param>
        /// <param name="parsedFunc12">Lambda executed on successful parsing of <typeparamref name="T12"/>.</param>
        /// <param name="parsedFunc13">Lambda executed on successful parsing of <typeparamref name="T13"/>.</param>
        /// <param name="parsedFunc14">Lambda executed on successful parsing of <typeparamref name="T14"/>.</param>
        /// <param name="parsedFunc15">Lambda executed on successful parsing of <typeparamref name="T15"/>.</param>
        /// <param name="parsedFunc16">Lambda executed on successful parsing of <typeparamref name="T16"/>.</param>
        /// <param name="notParsedFunc">Lambda executed on failed parsing.</param>
        /// <returns>The new value.</returns>
        public static TResult MapResult<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this ParserResult<object> result,
            Func<T1, TResult> parsedFunc1,
            Func<T2, TResult> parsedFunc2,
            Func<T3, TResult> parsedFunc3,
            Func<T4, TResult> parsedFunc4,
            Func<T5, TResult> parsedFunc5,
            Func<T6, TResult> parsedFunc6,
            Func<T7, TResult> parsedFunc7,
            Func<T8, TResult> parsedFunc8,
            Func<T9, TResult> parsedFunc9,
            Func<T10, TResult> parsedFunc10,
            Func<T11, TResult> parsedFunc11,
            Func<T12, TResult> parsedFunc12,
            Func<T13, TResult> parsedFunc13,
            Func<T14, TResult> parsedFunc14,
            Func<T15, TResult> parsedFunc15,
            Func<T16, TResult> parsedFunc16,
            Func<IEnumerable<Error>, TResult> notParsedFunc)
        {
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                if (parsed.Value is T1)
                {
                    return parsedFunc1((T1)parsed.Value);
                }
                if (parsed.Value is T2)
                {
                    return parsedFunc2((T2)parsed.Value);
                }
                if (parsed.Value is T3)
                {
                    return parsedFunc3((T3)parsed.Value);
                }
                if (parsed.Value is T4)
                {
                    return parsedFunc4((T4)parsed.Value);
                }
                if (parsed.Value is T5)
                {
                    return parsedFunc5((T5)parsed.Value);
                }
                if (parsed.Value is T6)
                {
                    return parsedFunc6((T6)parsed.Value);
                }
                if (parsed.Value is T7)
                {
                    return parsedFunc7((T7)parsed.Value);
                }
                if (parsed.Value is T8)
                {
                    return parsedFunc8((T8)parsed.Value);
                }
                if (parsed.Value is T9)
                {
                    return parsedFunc9((T9)parsed.Value);
                }
                if (parsed.Value is T10)
                {
                    return parsedFunc10((T10)parsed.Value);
                }
                if (parsed.Value is T11)
                {
                    return parsedFunc11((T11)parsed.Value);
                }
                if (parsed.Value is T12)
                {
                    return parsedFunc12((T12)parsed.Value);
                }
                if (parsed.Value is T13)
                {
                    return parsedFunc13((T13)parsed.Value);
                }
                if (parsed.Value is T14)
                {
                    return parsedFunc14((T14)parsed.Value);
                }
                if (parsed.Value is T15)
                {
                    return parsedFunc15((T15)parsed.Value);
                }
                if (parsed.Value is T16)
                {
                    return parsedFunc16((T16)parsed.Value);
                }
                throw new InvalidOperationException();
            }
            return notParsedFunc(((NotParsed<object>)result).Errors);
        }
    }
}
