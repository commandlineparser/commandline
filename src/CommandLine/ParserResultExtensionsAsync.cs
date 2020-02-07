// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommandLine
{
    public static partial class ParserResultExtensions
    {
#if !NET40
        /// <summary>
        /// Executes asynchronously <paramref name="action"/> if <see cref="CommandLine.ParserResult{T}"/> contains
        /// parsed values.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="Func{T, Task}"/> to execute.</param>
        /// <returns>The same <paramref name="result"/> instance as a <see cref="Task"/> instance.</returns>
        public static async Task<ParserResult<T>> WithParsedAsync<T>(this ParserResult<T> result, Func<T, Task> action)
        {
            if (result is Parsed<T> parsed)
            {
                await action(parsed.Value);
            }
            return result;
        }

        /// <summary>
        /// Executes asynchronously <paramref name="action"/> if parsed values are of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An verb result instance.</param>
        /// <param name="action">The <see cref="Func{T, Task}"/> to execute.</param>
        /// <returns>The same <paramref name="result"/> instance as a <see cref="Task"/> instance.</returns>
        public static async Task<ParserResult<object>> WithParsedAsync<T>(this ParserResult<object> result, Func<T, Task> action)
        {
            if (result is Parsed<object> parsed)
            {
                if (parsed.Value is T value)
                {
                    await action(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Executes asynchronously <paramref name="action"/> if <see cref="CommandLine.ParserResult{T}"/> lacks
        /// parsed values and contains errors.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="System.Func{Task}"/> delegate to execute.</param>
        /// <returns>The same <paramref name="result"/> instance as a <see cref="Task"/> instance.</returns>
        public static async Task<ParserResult<T>> WithNotParsedAsync<T>(this ParserResult<T> result, Func<IEnumerable<Error>, Task> action)
        {
            if (result is NotParsed<T> notParsed)
            {
                await action(notParsed.Errors);
            }
            return result;
        }
#endif
    }
}
