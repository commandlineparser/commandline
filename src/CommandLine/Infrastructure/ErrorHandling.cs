//Use project level define(s) when referencing with Paket.
//#define ERRH_INTERNAL //  Uncomment or define at build time to set accessibility to internal.
//#define ERRH_ENABLE_INLINE_METHODS // Uncomment or define at build time to enable method inlining when compiling for >= NET 4.5.
//#define ERRH_ADD_MAYBE_METHODS // Uncomment or define at build time to add methods that use Maybe type

using System;
using System.Collections.Generic;
using System.Linq;
#if ERRH_ADD_MAYBE_METHODS
using CSharpx;
#endif

namespace RailwaySharp.ErrorHandling
{
#if !ERRH_INTERNAL
    public
#endif
    enum ResultType
    {
        Ok,
        Bad
    }

    /// <summary>
    /// Represents the result of a computation.
    /// </summary>
    /// <typeparam name="TSuccess">Type that models the result of a successful computation.</typeparam>
    /// <typeparam name="TMessage">Type that model a message related to a computation.</typeparam> 
#if !ERRH_INTERNAL
    public
#endif 
    abstract class Result<TSuccess, TMessage>
    {
        private readonly ResultType _tag;

        protected Result(ResultType tag)
        {
            _tag = tag;
        }

        public ResultType Tag
        {
            get { return _tag; }
        }

        public override string ToString()
        {
            switch (Tag) {
                default:
                    var ok = (Ok<TSuccess, TMessage>)this;
                    return string.Format(
                        "OK: {0} - {1}",
                        ok.Success,
                        string.Join(Environment.NewLine, ok.Messages.Select(v => v.ToString())));
                case ResultType.Bad:
                    var bad = (Bad<TSuccess, TMessage>)this;
                    return string.Format(
                        "Error: {0}",
                        string.Join(Environment.NewLine, bad.Messages.Select(v => v.ToString())));
            }
        }
    }

    /// <summary>
    /// Represents the result of a successful computation.
    /// </summary>
    /// <typeparam name="TSuccess">Type that models the result of a successful computation.</typeparam>
    /// <typeparam name="TMessage">Type that model a message related to a computation.</typeparam> 
#if !ERRH_INTERNAL
    public
#endif 
    sealed class Ok<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly Tuple<TSuccess, IEnumerable<TMessage>> _value;

        public Ok(TSuccess success, IEnumerable<TMessage> messages)
            : base(ResultType.Ok)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));

            _value = Tuple.Create(success, messages);
        }

        public TSuccess Success
        {
            get { return _value.Item1; }
        }

        public IEnumerable<TMessage> Messages
        {
            get { return _value.Item2; }
        }
    }

    /// <summary>
    /// Represents the result of a failed computation.
    /// </summary>
    /// <typeparam name="TSuccess">Type that models the result of a successful computation.</typeparam>
    /// <typeparam name="TMessage">Type that model a message related to a computation.</typeparam> 
#if !ERRH_INTERNAL
    public
#endif
    sealed class Bad<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly IEnumerable<TMessage> _messages;

        public Bad(IEnumerable<TMessage> messages)
            : base(ResultType.Bad)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            _messages = messages;
        }

        public IEnumerable<TMessage> Messages
        {
            get { return _messages; }
        }
    }

#if !ERRH_INTERNAL
    public
#endif
    static class Result
    {
        /// <summary>
        /// Creates a Failure result with the given messages.
        /// </summary>
        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(IEnumerable<TMessage> messages)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            return new Bad<TSuccess, TMessage>(messages);
        }

        /// <summary>
        /// Creates a Failure result with the given message.
        /// </summary>
        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        /// <summary>
        /// Creates a Success result with the given value.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        /// <summary>
        /// Creates a Success result with the given value and the given message.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Ok<TSuccess, TMessage>(value, new[] { message });
        }

        /// <summary>
        /// Creates a Success result with the given value and the given messages.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, IEnumerable<TMessage> messages)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            return new Ok<TSuccess, TMessage>(value, messages);
        }

        /// <summary>
        /// Executes the given function on a given success or captures the failure.
        /// </summary>
        public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> func)
        {
            if (func == null) throw new ArgumentException(nameof(func));

            try {
                return new Ok<TSuccess, Exception>(
                        func(), Enumerable.Empty<Exception>());
            }
            catch (Exception ex) {
                return new Bad<TSuccess, Exception>(
                    new[] { ex });
            }
        }
    }

#if !ERRH_INTERNAL
    public
#endif
    static class Trial
    {
        /// <summary>
        /// Wraps a value in a Success.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Ok<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        /// <summary>
        /// Wraps a value in a Success.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Pass<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>());
        }

        /// <summary>
        /// Wraps a value in a Success and adds a message.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Warn<TSuccess, TMessage>(TMessage message, TSuccess value)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Ok<TSuccess, TMessage>(value, new[] { message });
        }

        /// <summary>
        /// Wraps a message in a Failure.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Fail<TSuccess, TMessage>(TMessage message)
        {
            if (message == null) throw new ArgumentException(nameof(message));

            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        /// <summary>
        /// Returns true if the result was not successful.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Failed<TSuccess, TMessage>(Result<TSuccess, TMessage> result)
        {
            return result.Tag == ResultType.Bad;
        }

        /// <summary>
        /// Takes a Result and maps it with successFunc if it is a Success otherwise it maps it with failureFunc.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TResult Either<TSuccess, TMessage, TResult>(
            Func<TSuccess, IEnumerable<TMessage>, TResult> successFunc,
            Func<IEnumerable<TMessage>, TResult> failureFunc,
            Result<TSuccess, TMessage> trialResult)
        {
            if (successFunc == null) throw new ArgumentException(nameof(successFunc));
            if (failureFunc == null) throw new ArgumentException(nameof(failureFunc));

            var ok = trialResult as Ok<TSuccess, TMessage>;
            if (ok != null) {
                return successFunc(ok.Success, ok.Messages);
            }
            var bad = (Bad<TSuccess, TMessage>)trialResult;
            return failureFunc(bad.Messages);
        }

        /// <summary>
        /// If the given result is a Success the wrapped value will be returned. 
        /// Otherwise the function throws an exception with Failure message of the result.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TSuccess ReturnOrFail<TSuccess, TMessage>(Result<TSuccess, TMessage> result)
        {
            Func<IEnumerable<TMessage>, TSuccess> raiseExn = msgs =>
            {
                throw new Exception(
                    string.Join(
                    Environment.NewLine, msgs.Select(m => m.ToString())));
            };

            return Either((succ, _) => succ, raiseExn, result);
        }

        /// <summary>
        /// Appends the given messages with the messages in the given result.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> MergeMessages<TSuccess, TMessage>(
            IEnumerable<TMessage> messages,
            Result<TSuccess, TMessage> result)
        {
            if (messages == null) throw new ArgumentException(nameof(messages));

            Func<TSuccess, IEnumerable<TMessage>, Result<TSuccess, TMessage>> successFunc =
                (succ, msgs) =>
                    new Ok<TSuccess, TMessage>(
                        succ, messages.Concat(msgs));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                errors => new Bad<TSuccess, TMessage>(errors.Concat(messages));

            return Either(successFunc, failureFunc, result);
        }

        /// <summary>
        /// If the result is a Success it executes the given function on the value.
        /// Otherwise the exisiting failure is propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Bind<TValue, TSuccess, TMessage>(
            Func<TValue, Result<TSuccess, TMessage>> func,
            Result<TValue, TMessage> result)
        {
            if (func == null) throw new ArgumentException(nameof(func));

            Func<TValue, IEnumerable<TMessage>, Result<TSuccess, TMessage>> successFunc =
                (succ, msgs) => MergeMessages(msgs, func(succ));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                messages => new Bad<TSuccess, TMessage>(messages);

            return Either(successFunc, failureFunc, result);
        }

        /// <summary>
        /// Flattens a nested result given the Failure types are equal.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Flatten<TSuccess, TMessage>(
            Result<Result<TSuccess, TMessage>, TMessage> result)
        {
            return Bind(x => x, result);
        }
        
        /// <summary>
        /// If the wrapped function is a success and the given result is a success the function is applied on the value. 
        /// Otherwise the exisiting error messages are propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Apply<TValue, TSuccess, TMessage>(
            Result<Func<TValue, TSuccess>, TMessage> wrappedFunction,
            Result<TValue, TMessage> result)
        {
            if (wrappedFunction == null) throw new ArgumentException(nameof(wrappedFunction));

            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Ok) {
                var ok1 = (Ok<Func<TValue, TSuccess>, TMessage>)wrappedFunction;
                var ok2 = (Ok<TValue, TMessage>)result;

                return new Ok<TSuccess, TMessage>(
                    ok1.Success(ok2.Success), ok1.Messages.Concat(ok2.Messages));
            }
            if (wrappedFunction.Tag == ResultType.Bad && result.Tag == ResultType.Ok) {
                return new Bad<TSuccess, TMessage>(((Bad<TValue, TMessage>)result).Messages);
            }
            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Bad) {
                return new Bad<TSuccess, TMessage>(
                    ((Bad<TValue, TMessage>)result).Messages);
            }

            var bad1 = (Bad<Func<TValue, TSuccess>, TMessage>)wrappedFunction;
            var bad2 = (Bad<TValue, TMessage>)result;
            return new Bad<TSuccess, TMessage>(bad1.Messages.Concat(bad2.Messages));
        }

        /// <summary>
        /// Lifts a function into a Result container and applies it on the given result.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Lift<TValue, TSuccess, TMessage>(
            Func<TValue, TSuccess> func,
            Result<TValue, TMessage> result)
        {
            return Apply(Ok<Func<TValue, TSuccess>, TMessage>(func), result);
        }

        /// <summary>
        /// Promote a function to a monad/applicative, scanning the monadic/applicative arguments from left to right.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess1, TMessage1> Lift2<TSuccess, TMessage, TSuccess1, TMessage1>(
            Func<TSuccess, Func<TMessage, TSuccess1>> func,
            Result<TSuccess, TMessage1> first,
            Result<TMessage, TMessage1> second)
        {
            return Apply(Lift(func, first), second);
        }

        /// <summary>
        /// Collects a sequence of Results and accumulates their values.
        /// If the sequence contains an error the error will be propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<IEnumerable<TSuccess>, TMessage> Collect<TSuccess, TMessage>(
            IEnumerable<Result<TSuccess, TMessage>> xs)
        {
            return Lift(Enumerable.Reverse,
                xs.Aggregate<Result<TSuccess, TMessage>, Result<IEnumerable<TSuccess>, TMessage>, Result<IEnumerable<TSuccess>, TMessage>>(
                null, 
                (result, next) => {
                    if (result.Tag == ResultType.Ok && next.Tag == ResultType.Ok) {
                        var ok1 = (Ok<IEnumerable<TSuccess>, TMessage>)result;
                        var ok2 = (Ok<TSuccess, TMessage>)next;
                        return
                            new Ok<IEnumerable<TSuccess>, TMessage>(
                                    Enumerable.Empty<TSuccess>().Concat(new [] { ok2.Success }).Concat(ok1.Success),
                                    ok1.Messages.Concat(ok2.Messages));
                    }
                    if ((result.Tag == ResultType.Ok && next.Tag == ResultType.Bad)
                        || (result.Tag == ResultType.Bad && next.Tag == ResultType.Ok)) {
                        var m1 = result.Tag == ResultType.Ok
                            ? ((Ok<IEnumerable<TSuccess>, TMessage>)result).Messages
                            : ((Bad<TSuccess, TMessage>)next).Messages;
                        var m2 = result.Tag == ResultType.Bad
                            ? ((Bad<IEnumerable<TSuccess>, TMessage>)result).Messages
                            : ((Ok<TSuccess, TMessage>)next).Messages;
                        return new Bad<IEnumerable<TSuccess>, TMessage>(m1.Concat(m2));
                    }

                    var bad1 = (Bad<IEnumerable<TSuccess>, TMessage>)result;
                    var bad2 = (Bad<TSuccess, TMessage>)next;            
                    return new Bad<IEnumerable<TSuccess>, TMessage>(bad1.Messages.Concat(bad2.Messages));
                }, x => x));
        }
    }

    /// <summary>
    /// Extensions methods for easier usage.
    /// </summary>
#if !ERRH_INTERNAL
    public
#endif
    static class ResultExtensions
    {
        /// <summary>
        /// Allows pattern matching on Results.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Match<TSuccess, TMessage>(this Result<TSuccess, TMessage> result,
            Action<TSuccess, IEnumerable<TMessage>> ifSuccess,
            Action<IEnumerable<TMessage>> ifFailure)
        {
            if (ifSuccess == null) throw new ArgumentException(nameof(ifSuccess));
            if (ifFailure == null) throw new ArgumentException(nameof(ifFailure));

            var ok = result as Ok<TSuccess, TMessage>;
            if (ok != null) {
                ifSuccess(ok.Success, ok.Messages);
                return;
            }

            var bad = (Bad<TSuccess, TMessage>)result;
            ifFailure(bad.Messages);
        }

        /// <summary>
        /// Allows pattern matching on Results.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TResult Either<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, IEnumerable<TMessage>, TResult> ifSuccess,
            Func<IEnumerable<TMessage>, TResult> ifFailure)
        {
            return Trial.Either(ifSuccess, ifFailure, result);
        }

        /// <summary>
        /// Lifts a Func into a Result and applies it on the given result.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TResult, TMessage> Map<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, TResult> func)
        {
            return Trial.Lift(func, result);
        }

        /// <summary>
        /// Collects a sequence of Results and accumulates their values.
        /// If the sequence contains an error the error will be propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<IEnumerable<TSuccess>, TMessage> Collect<TSuccess, TMessage>(
            this IEnumerable<Result<TSuccess, TMessage>> values)
        {
            return Trial.Collect(values);
        }

        /// <summary>
        /// Collects a sequence of Results and accumulates their values.
        /// If the sequence contains an error the error will be propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<IEnumerable<TSuccess>, TMessage> Flatten<TSuccess, TMessage>(this Result<IEnumerable<Result<TSuccess, TMessage>>, TMessage> result)
        {
            if (result.Tag == ResultType.Ok) {
                var ok = (Ok<IEnumerable<Result<TSuccess, TMessage>>, TMessage>)result;
                var values = ok.Success;
                var result1 = Collect(values);
                if (result1.Tag == ResultType.Ok) {
                    var ok1 = (Ok<IEnumerable<TSuccess>, TMessage>)result1;
                    return new Ok<IEnumerable<TSuccess>, TMessage>(ok1.Success, ok1.Messages);
                }
                var bad1 = (Bad<IEnumerable<TSuccess>, TMessage>)result1;
                return new Bad<IEnumerable<TSuccess>, TMessage>(bad1.Messages);
            }
            var bad = (Bad<IEnumerable<Result<TSuccess, TMessage>>, TMessage>)result;
            return new Bad<IEnumerable<TSuccess>, TMessage>(bad.Messages);
        }

        /// <summary>
        /// If the result is a Success it executes the given Func on the value.
        /// Otherwise the exisiting failure is propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TResult, TMessage> SelectMany<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, Result<TResult, TMessage>> func)
        {
            return Trial.Bind(func, result);
        }

        /// <summary>
        /// If the result is a Success it executes the given Func on the value.
        /// If the result of the Func is a Success it maps it using the given Func.
        /// Otherwise the exisiting failure is propagated.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TResult, TMessage> SelectMany<TSuccess, TMessage, TValue, TResult>(
            this Result<TSuccess, TMessage> result,
            Func<TSuccess, Result<TValue, TMessage>> func,
            Func<TSuccess, TValue, TResult> mapperFunc)
        {
            if (func == null) throw new ArgumentException(nameof(func));
            if (mapperFunc == null) throw new ArgumentException(nameof(mapperFunc));

            Func<TSuccess, Func<TValue, TResult>> curriedMapper = suc => val => mapperFunc(suc, val);
            Func<
                Result<TSuccess, TMessage>,
                Result<TValue, TMessage>,
                Result<TResult, TMessage>
            > liftedMapper = (a, b) => Trial.Lift2(curriedMapper, a, b);
            var v = Trial.Bind(func, result);
            return liftedMapper(result, v);
        }

        /// <summary>
        /// Lifts a Func into a Result and applies it on the given result.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TResult, TMessage> Select<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, TResult> func)
        {
            return Trial.Lift(func, result);
        }

        /// <summary>
        /// Returns the error messages or fails if the result was a success.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TMessage> FailedWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok) {
                var ok = (Ok<TSuccess, TMessage>)result;
                throw new Exception(
                    string.Format("Result was a success: {0} - {1}",
                    ok.Success,
                    string.Join(Environment.NewLine, ok.Messages.Select(m => m.ToString()))));
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            return bad.Messages;
        }

        /// <summary>
        /// Returns the result or fails if the result was an error.
        /// </summary>
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TSuccess SucceededWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok) {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Success;
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            throw new Exception(
                string.Format("Result was an error: {0}",
                string.Join(Environment.NewLine, bad.Messages.Select(m => m.ToString()))));
        }

        /// <summary>
        /// Returns messages in case of success, otherwise an empty sequence. 
        /// </summary>
        public static IEnumerable<TMessage> SuccessMessages<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok) {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Messages;
            }
            return Enumerable.Empty<TMessage>();
        }

#if ERRH_ADD_MAYBE_METHODS
#if ERRH_ENABLE_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        /// <summary>
        /// Builds a Maybe type instance from a Result one.
        /// </summary>
        public static Maybe<TSuccess> ToMaybe<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok) {
                var ok = (Ok<TSuccess, TMessage>)result;
                return Maybe.Just(ok.Success);
            }
            return Maybe.Nothing<TSuccess>();
        }
#endif
    }
}