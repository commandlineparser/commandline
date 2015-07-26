using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RailwaySharp.ErrorHandling;

namespace CommandLine.Infrastructure
{
    static class ResultExtensions
    {
        public static IEnumerable<TMessage> Messages<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Value.Messages;
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            return bad.Messages;
        }
    }
}
