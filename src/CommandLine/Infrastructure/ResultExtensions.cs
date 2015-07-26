// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Infrastructure
{
    static class ResultExtensions
    {
        public static IEnumerable<TMessage> SuccessfulMessages<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Value.Messages;
            }
            return Enumerable.Empty<TMessage>();
        }

        public static Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>> TupledWith
            <TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3>(this Result<TSuccess1, TMessage1> first, Result<TSuccess2, TMessage2> second, Result<TSuccess3, TMessage3> third)
        {
            return Tuple.Create(first, second, third);
        }

        public static Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>, TValue> TupledWith
            <TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3, TValue>(this Result<TSuccess1, TMessage1> first, Result<TSuccess2, TMessage2> second, Result<TSuccess3, TMessage3> third, TValue value)
        {
            return Tuple.Create(first, second, third, value);
        }

        /// <summary>
        /// Simulates F# tuple pattern matching on 3 computation resultsAndValue, like 'match result1, result2, result3 with'.
        /// </summary>
        public static void Match<TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3>(
            this Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>> results,
            Action<TSuccess1, IEnumerable<TMessage1>, TSuccess2, IEnumerable<TMessage2>, TSuccess3, IEnumerable<TMessage3>> ifAllSuccess,
            Action<IEnumerable<TMessage1>, IEnumerable<TMessage2>, IEnumerable<TMessage3>> ifAllFailure, 
            Action<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>> fallback)
        {
            var ok1 = results.Item1 as Ok<TSuccess1, TMessage1>;
            var ok2 = results.Item2 as Ok<TSuccess2, TMessage2>;
            var ok3 = results.Item3 as Ok<TSuccess3, TMessage3>;
            if (ok1 != null && ok2 != null && ok3 != null)
            {
                ifAllSuccess(
                    ok1.Value.Success, ok1.Value.Messages,
                    ok2.Value.Success, ok2.Value.Messages,
                    ok3.Value.Success, ok3.Value.Messages);
                return;
            }
            if (ok1 == null && ok2 == null && ok3 == null)
            {
                ifAllFailure(
                    ((Bad<TSuccess1, TMessage1>)results.Item1).Messages,
                    ((Bad<TSuccess2, TMessage2>)results.Item2).Messages,
                    ((Bad<TSuccess3, TMessage3>)results.Item3).Messages);
                return;
            }
            fallback(results.Item1, results.Item2, results.Item3);
        }

        public static void Match<TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3, TValue>(
            this Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>, TValue> resultsAndValue,
            Action<TSuccess1, IEnumerable<TMessage1>, TSuccess2, IEnumerable<TMessage2>, TSuccess3, IEnumerable<TMessage3>, TValue> ifAllSuccess,
            Action<IEnumerable<TMessage1>, IEnumerable<TMessage2>, IEnumerable<TMessage3>, TValue> ifAllFailure,
            Action<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>, TValue> fallback)
        {
            var ok1 = resultsAndValue.Item1 as Ok<TSuccess1, TMessage1>;
            var ok2 = resultsAndValue.Item2 as Ok<TSuccess2, TMessage2>;
            var ok3 = resultsAndValue.Item3 as Ok<TSuccess3, TMessage3>;
            var value = resultsAndValue.Item4;
            if (ok1 != null && ok2 != null && ok3 != null)
            {
                ifAllSuccess(
                    ok1.Value.Success, ok1.Value.Messages,
                    ok2.Value.Success, ok2.Value.Messages,
                    ok3.Value.Success, ok3.Value.Messages,
                    value);
                return;
            }
            if (ok1 == null && ok2 == null && ok3 == null)
            {
                ifAllFailure(
                    ((Bad<TSuccess1, TMessage1>)resultsAndValue.Item1).Messages,
                    ((Bad<TSuccess2, TMessage2>)resultsAndValue.Item2).Messages,
                    ((Bad<TSuccess3, TMessage3>)resultsAndValue.Item3).Messages,
                    value);
                return;
            }
            fallback(
                resultsAndValue.Item1,
                resultsAndValue.Item2,
                resultsAndValue.Item3,
                value);
        }

        public static TResult Either<TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3, TValue, TResult>(
            this Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>, TValue> resultsAndValue,
            Func<TSuccess1, IEnumerable<TMessage1>, TSuccess2, IEnumerable<TMessage2>, TSuccess3, IEnumerable<TMessage3>, TValue, TResult> ifAllSuccess,
            Func<IEnumerable<TMessage1>, IEnumerable<TMessage2>, IEnumerable<TMessage3>, TValue, TResult> ifAllFailure,
            Func<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>, TValue, TResult> fallback)
        {
            var ok1 = resultsAndValue.Item1 as Ok<TSuccess1, TMessage1>;
            var ok2 = resultsAndValue.Item2 as Ok<TSuccess2, TMessage2>;
            var ok3 = resultsAndValue.Item3 as Ok<TSuccess3, TMessage3>;
            var value = resultsAndValue.Item4;
            if (ok1 != null && ok2 != null && ok3 != null)
            {
                return ifAllSuccess(
                    ok1.Value.Success, ok1.Value.Messages,
                    ok2.Value.Success, ok2.Value.Messages,
                    ok3.Value.Success, ok3.Value.Messages,
                    value);
            }
            if (ok1 == null && ok2 == null && ok3 == null)
            {
                return ifAllFailure(
                    ((Bad<TSuccess1, TMessage1>)resultsAndValue.Item1).Messages,
                    ((Bad<TSuccess2, TMessage2>)resultsAndValue.Item2).Messages,
                    ((Bad<TSuccess3, TMessage3>)resultsAndValue.Item3).Messages,
                    value);
            }
            return fallback(
                resultsAndValue.Item1,
                resultsAndValue.Item2,
                resultsAndValue.Item3,
                value);
        }
    }
}