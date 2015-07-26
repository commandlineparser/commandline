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

        /// <summary>
        /// Simulates F# tuple pattern matching on 3 computation results, like 'match result1, result2, result3 with'.
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
            }
            fallback(results.Item1, results.Item2, results.Item3);
        }
    }
}

namespace CSharpx
{
    static partial class Trial
    {
        public static Tuple<Result<TSuccess1, TMessage1>, Result<TSuccess2, TMessage2>, Result<TSuccess3, TMessage3>> Tupled
            <TSuccess1, TMessage1, TSuccess2, TMessage2, TSuccess3, TMessage3>(Result<TSuccess1, TMessage1> first, Result<TSuccess2, TMessage2> second, Result<TSuccess3, TMessage3> third)
        {
            return Tuple.Create(first, second, third);
        }
    }
}