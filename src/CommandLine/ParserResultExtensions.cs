// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;

namespace CommandLine
{
    /// <summary>
    /// Provides convenience extension methods for <see cref="CommandLine.ParserResult{T}"/>.
    /// </summary>
    public static class ParserResultExtensions
    {
        /// <summary>
        /// Executes <see cref="Action{T}"/> if <see cref="CommandLine.ParserResult{T}"/> contains
        /// parsed values.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="Action{T}"/> to execute.</param>
        /// <returns></returns>
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
        /// Executes <see cref="Action{IEnumerable{Error}}"/> if <see cref="CommandLine.ParserResult{T}"/> lacks
        /// parsed values and contains errors.
        /// </summary>
        /// <typeparam name="T">Type of the target instance built with parsed value.</typeparam>
        /// <param name="result">An <see cref="CommandLine.ParserResult{T}"/> instance.</param>
        /// <param name="action">The <see cref="Action{IEnumerable{Error}}"/> to execute.</param>
        /// <returns></returns>
        public static ParserResult<T> WithNotParsed<T>(this ParserResult<T> result, Action<IEnumerable<Error>> action)
        {
            var notParsed = result as NotParsed<T>;
            if (notParsed != null)
            {
                action(notParsed.Errors);
            }
            return result;
        }
    }
}
