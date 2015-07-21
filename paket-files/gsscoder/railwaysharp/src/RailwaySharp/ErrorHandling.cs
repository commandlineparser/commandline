#define ERRH_PUBLIC // Comment this to set visibility to internal.
#define ERRH_INLINE_METHODS // Comment this to disable method inlining when compiling for <= NET 4.0.
#define ERRH_BUILTIN_ENUMEXT // Comment this to use CSharpx.EnumerableExtensions.
#define ERRH_BUILTIN_UNIT // Comment this to use CSharpx.Unit.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
#if !ERRH_BUILTIN_ENUMEXT || !ERRH_BUILTIN_UNIT
using CSharpx;
#endif

namespace RailwaySharp.ErrorHandling
{
    #region Enumerable Extensions
#if ERRH_BUILTIN_ENUMEXT
    static class EnumerableExtensions
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

        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, TResult> folder)
        {
            return FoldImpl(source, 1, folder, null, null, null);
        }

        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, TResult> folder)
        {
            return FoldImpl(source, 2, null, folder, null, null);
        }

        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, T, TResult> folder)
        {
            return FoldImpl(source, 3, null, null, folder, null);
        }

        public static TResult Fold<T, TResult>(this IEnumerable<T> source, Func<T, T, T, T, TResult> folder)
        {
            return FoldImpl(source, 4, null, null, null, folder);
        }

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source)
        {
            return source.Index(0);
        }

        public static IEnumerable<KeyValuePair<int, TSource>> Index<TSource>(this IEnumerable<TSource> source, int startIndex)
        {
            return source.Select((item, index) => new KeyValuePair<int, TSource>(startIndex + index, item));
        }
    }
#endif
    #endregion

    #region Unit Type
#if ERRH_BUILTIN_UNIT
#if ERRH_PUBLIC
    public
#endif
    struct Unit : IEquatable<Unit>
    {
        private static readonly Unit @default = new Unit();

        public bool Equals(Unit other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "()";
        }

        public static bool operator ==(Unit first, Unit second)
        {
            return true;
        }

        public static bool operator !=(Unit first, Unit second)
        {
            return false;
        }

        public static Unit Default { get { return @default; } }
    }
#endif
    #endregion

    #region Ok Type Custom Tuple
    /// <summary>
    /// Tuple for <see cref="RailwaySharp.ErrorHandling.Ok{TSuccess, TMessage}"/> type. 
    /// </summary>
#if ERRH_PUBLIC
    public
#endif
    sealed class OkPair<TSuccess, TMessage> : IEquatable<OkPair<TSuccess, TMessage>>
    {
        private readonly TSuccess success;
        private readonly IEnumerable<TMessage> messages;

        internal OkPair(TSuccess success, IEnumerable<TMessage> messages)
        {
            this.success = success;
            this.messages = messages;
        }

        public TSuccess Success
        {
            get { return success; }
        }

        public IEnumerable<TMessage> Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as OkPair<TSuccess, TMessage>;
            if (other != null)
            {
                return Equals(other);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <remarks>A hash code for the current <see cref="System.Object"/>.</remarks>
        public override int GetHashCode()
        {
            return new { Success, Messages }.GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="RailwaySharp.ErrorHandling.OkPair{TSuccess, TMessage}"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="RailwaySharp.ErrorHandling.OkPair{TSuccess, TMessage}"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="RailwaySharp..ErrorHandling.OkPair{TSuccess, TMessage}"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(OkPair<TSuccess, TMessage> other)
        {
            if (other == null)
            {
                return false;
            }

            return Success.Equals(other.Success) && Messages.SequenceEqual(other.Messages);
        }
    }

#if ERRH_PUBLIC
    public
#endif
    static class OkPair
    {
        public static OkPair<TSuccess, TMessage> Create<TSuccess, TMessage>(TSuccess success, IEnumerable<TMessage> messages)
        {
            if (success == null) throw new ArgumentNullException("success");
            if (messages == null) throw new ArgumentNullException("messages");

            return new OkPair<TSuccess, TMessage>(success, messages);
        }

        public static TSuccess First<TSuccess, TMessage>(OkPair<TSuccess, TMessage> okPair)
        {
            return okPair.Success;
        }

        public static IEnumerable<TMessage> Second<TSuccess, TMessage>(OkPair<TSuccess, TMessage> okPair)
        {
            return okPair.Messages;
        }
    }
    #endregion

#if ERRH_PUBLIC
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
#if ERRH_PUBLIC
    public
#endif 
    abstract class Result<TSuccess, TMessage>
    {
        private readonly ResultType tag;

        protected Result(ResultType tag)
        {
            this.tag = tag;
        }

        public ResultType Tag
        {
            get { return tag; }
        }

        public override string ToString()
        {
            switch (Tag)
            {
                case ResultType.Ok:
                    var ok = (Ok<TSuccess, TMessage>)this;
                    return string.Format(
                        "OK: {0} - {1}",
                        ok.Value.Success,
                        string.Join(Environment.NewLine, ok.Value.Messages.Select(v => v.ToString())));
                default:
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
#if ERRH_PUBLIC
    public
#endif 
    sealed class Ok<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly OkPair<TSuccess, TMessage> value;

        public Ok(OkPair<TSuccess, TMessage> value)
            : base(ResultType.Ok)
        {
            this.value = value;
        }

        public OkPair<TSuccess, TMessage> Value
        {
            get { return value; }
        }
    }

    /// <summary>
    /// Represents the result of a failed computation.
    /// </summary>
    /// <typeparam name="TSuccess">Type that models the result of a successful computation.</typeparam>
    /// <typeparam name="TMessage">Type that model a message related to a computation.</typeparam> 
#if ERRH_PUBLIC
    public
#endif
    sealed class Bad<TSuccess, TMessage> : Result<TSuccess, TMessage>
    {
        private readonly IEnumerable<TMessage> messages;

        public Bad(IEnumerable<TMessage> messages)
            : base(ResultType.Bad)
        {
            this.messages = messages;
        }

        public IEnumerable<TMessage> Messages
        {
            get { return messages; }
        }
    }

#if ERRH_PUBLIC
    public
#endif
    static class Result
    {
        /// <summary>
        /// Creates a Failure result with the given messages.
        /// </summary>
        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(IEnumerable<TMessage> messages)
        {
            return new Bad<TSuccess, TMessage>(messages);
        }

        /// <summary>
        /// Creates a Failure result with the given message.
        /// </summary>
        public static Result<TSuccess, TMessage> FailWith<TSuccess, TMessage>(TMessage message)
        {
            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        /// <summary>
        /// Creates a Success result with the given value.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>()));
        }

        /// <summary>
        /// Creates a Success result with the given value and the given message.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, TMessage message)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, new[] { message }));
        }

        /// <summary>
        /// Creates a Success result with the given value and the given messages.
        /// </summary>
        public static Result<TSuccess, TMessage> Succeed<TSuccess, TMessage>(TSuccess value, IEnumerable<TMessage> messages)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, messages));
        }

        /// <summary>
        /// Executes the given function on a given success or captures the failure.
        /// </summary>
        public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> func)
        {
            try
            {
                return new Ok<TSuccess, Exception>(
                    new OkPair<TSuccess, Exception>(
                        func(), Enumerable.Empty<Exception>()));
            }
            catch (Exception ex)
            {
                return new Bad<TSuccess, Exception>(
                    new[] { ex });
            }
        }
    }

#if ERRH_PUBLIC
    public
#endif
    static class Trial
    {
        /// <summary>
        /// Wraps a value in a Success.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Ok<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>()));
        }

        /// <summary>
        /// Wraps a value in a Success.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Pass<TSuccess, TMessage>(TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, Enumerable.Empty<TMessage>()));
        }

        /// <summary>
        /// Wraps a value in a Success and adds a message.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Warn<TSuccess, TMessage>(TMessage message, TSuccess value)
        {
            return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(value, new[] { message }));
        }

        /// <summary>
        /// Wraps a message in a Failure.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Fail<TSuccess, TMessage>(TMessage message)
        {
            return new Bad<TSuccess, TMessage>(new[] { message });
        }

        /// <summary>
        /// Returns true if the result was not successful.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool Failed<TSuccess, TMessage>(Result<TSuccess, TMessage> result)
        {
            return result.Tag == ResultType.Bad;
        }

        /// <summary>
        /// Takes a Result and maps it with successFunc if it is a Success otherwise it maps it with failureFunc.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TResult Either<TSuccess, TMessage, TResult>(
            Func<OkPair<TSuccess, TMessage>, TResult> successFunc,
            Func<IEnumerable<TMessage>, TResult> failureFunc,
            Result<TSuccess, TMessage> trialResult)
        {
            var ok = trialResult as Ok<TSuccess, TMessage>;
            if (ok != null)
            {
                return successFunc(ok.Value);
            }
            var bad = (Bad<TSuccess, TMessage>)trialResult;
            return failureFunc(bad.Messages);
        }

        /// <summary>
        /// If the given result is a Success the wrapped value will be returned. 
        /// Otherwise the function throws an exception with Failure message of the result.
        /// </summary>
#if ERRH_INLINE_METHODS
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

            return Either(OkPair.First, raiseExn, result);
        }

        /// <summary>
        /// Appends the given messages with the messages in the given result.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> MergeMessages<TSuccess, TMessage>(
            IEnumerable<TMessage> messages,
            Result<TSuccess, TMessage> result)
        {
            Func<OkPair<TSuccess, TMessage>, Result<TSuccess, TMessage>> successFunc =
                pair =>
                    new Ok<TSuccess, TMessage>(
                        new OkPair<TSuccess, TMessage>(pair.Success, messages.Concat(pair.Messages)));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                errors => new Bad<TSuccess, TMessage>(errors.Concat(messages));

            return Either(successFunc, failureFunc, result);
        }

        /// <summary>
        /// If the result is a Success it executes the given function on the value.
        /// Otherwise the exisiting failure is propagated.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Bind<TValue, TSuccess, TMessage>(
            Func<TValue, Result<TSuccess, TMessage>> func,
            Result<TValue, TMessage> result)
        {
            Func<OkPair<TValue, TMessage>, Result<TSuccess, TMessage>> successFunc =
                pair => MergeMessages(pair.Messages, func(pair.Success));

            Func<IEnumerable<TMessage>, Result<TSuccess, TMessage>> failureFunc =
                messages => new Bad<TSuccess, TMessage>(messages);

            return Either(successFunc, failureFunc, result);
        }

        /// <summary>
        /// Flattens a nested result given the Failure types are equal.
        /// </summary>
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess, TMessage> Apply<TValue, TSuccess, TMessage>(
            Result<Func<TValue, TSuccess>, TMessage> wrappedFunction,
            Result<TValue, TMessage> result)
        {
            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Ok)
            {
                var ok1 = (Ok<Func<TValue, TSuccess>, TMessage>)wrappedFunction;
                var ok2 = (Ok<TValue, TMessage>)result;

                return new Ok<TSuccess, TMessage>(new OkPair<TSuccess, TMessage>(
                    ok1.Value.Success(ok2.Value.Success), ok1.Value.Messages.Concat(ok2.Value.Messages)));
            }
            if (wrappedFunction.Tag == ResultType.Bad && result.Tag == ResultType.Ok)
            {
                return new Bad<TSuccess, TMessage>(((Bad<TValue, TMessage>)result).Messages);
            }
            if (wrappedFunction.Tag == ResultType.Ok && result.Tag == ResultType.Bad)
            {
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
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TSuccess1, TMessage1> Lift2<TSuccess, TMessage, TSuccess1, TMessage1>(
            Func<TSuccess, Func<TMessage, TSuccess1>> func,
            Result<TSuccess, TMessage1> a,
            Result<TMessage, TMessage1> b)
        {
            return Apply(Lift(func, a), b);
        }

        /// <summary>
        /// Collects a sequence of Results and accumulates their values.
        /// If the sequence contains an error the error will be propagated.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<IEnumerable<TSuccess>, TMessage> Collect<TSuccess, TMessage>(
            IEnumerable<Result<TSuccess, TMessage>> xs)
        {
            return Lift(Enumerable.Reverse, xs.Fold<Result<TSuccess, TMessage>, Result<IEnumerable<TSuccess>, TMessage>>(
                (result, next) =>
                {
                    if (result.Tag == ResultType.Ok && next.Tag == ResultType.Ok)
                    {
                        var ok1 = (Ok<TSuccess, TMessage>)result;
                        var ok2 = (Ok<TSuccess, TMessage>)next;
                        return
                            new Ok<IEnumerable<TSuccess>, TMessage>(
                                new OkPair<IEnumerable<TSuccess>, TMessage>(
                                    new[] { ok2.Value.Success, ok1.Value.Success },
                                    ok1.Value.Messages.Concat(ok2.Value.Messages)));
                    }
                    if ((result.Tag == ResultType.Ok && next.Tag == ResultType.Bad)
                        || (result.Tag == ResultType.Bad && next.Tag == ResultType.Ok))
                    {
                        var m1 = result.Tag == ResultType.Ok
                            ? ((Ok<TSuccess, TMessage>)result).Value.Messages
                            : ((Bad<TSuccess, TMessage>)next).Messages;
                        var m2 = result.Tag == ResultType.Bad
                            ? ((Bad<TSuccess, TMessage>)result).Messages
                            : ((Ok<TSuccess, TMessage>)next).Value.Messages;
                        return new Bad<IEnumerable<TSuccess>, TMessage>(m1.Concat(m2));
                    }
                    var bad1 = (Bad<TSuccess, TMessage>)result;
                    var bad2 = (Bad<TSuccess, TMessage>)next;
                    return new Bad<IEnumerable<TSuccess>, TMessage>(bad1.Messages.Concat(bad2.Messages));
                }));
        }
    }

    /// <summary>
    /// Extensions methods for easier usage.
    /// </summary>
#if ERRH_PUBLIC
    public
#endif
    static class ResultExtensions
    {
        /// <summary>
        /// Allows pattern matching on Results.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Match<TSuccess, TMessage>(this Result<TSuccess, TMessage> result,
            Action<TSuccess, IEnumerable<TMessage>> ifSuccess,
            Action<IEnumerable<TMessage>> ifFailure)
        {
            var ok = result as Ok<TSuccess, TMessage>;
            if (ok != null)
            {
                ifSuccess(ok.Value.Success, ok.Value.Messages);
                return;
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            ifFailure(bad.Messages);
        }

        /// <summary>
        /// Allows pattern matching on Results.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TResult Either<TSuccess, TMessage, TResult>(this Result<TSuccess, TMessage> result,
            Func<TSuccess, IEnumerable<TMessage>, TResult> ifSuccess,
            Func<IEnumerable<TMessage>, TResult> ifFailure)
        {
            var ok = result as Ok<TSuccess, TMessage>;
            if (ok != null)
            {
                return ifSuccess(ok.Value.Success, ok.Value.Messages);
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            return ifFailure(bad.Messages);
        }

        /// <summary>
        /// Lifts a Func into a Result and applies it on the given result.
        /// </summary>
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<IEnumerable<TSuccess>, TMessage> Flatten<TSuccess, TMessage>(this Result<IEnumerable<Result<TSuccess, TMessage>>, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<IEnumerable<Result<TSuccess, TMessage>>, TMessage>)result;
                var values = ok.Value.Success;
                var result1 = Collect(values);
                if (result1.Tag == ResultType.Ok)
                {
                    var ok1 = (Ok<IEnumerable<TSuccess>, TMessage>)result1;
                    return new Ok<IEnumerable<TSuccess>, TMessage>(new OkPair<IEnumerable<TSuccess>, TMessage>(ok1.Value.Success, ok1.Value.Messages));
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
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Result<TResult, TMessage> SelectMany<TSuccess, TMessage, TValue, TResult>(
            this Result<TSuccess, TMessage> result,
            Func<TSuccess, Result<TValue, TMessage>> func,
            Func<TSuccess, TValue, TResult> mapperFunc)
        {
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
#if ERRH_INLINE_METHODS
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
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TMessage> FailedWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                throw new Exception(
                    string.Format("Result was a success: {0} - {1}",
                    ok.Value.Success,
                    string.Join(Environment.NewLine, ok.Value.Messages.Select(m => m.ToString()))));
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            return bad.Messages;
        }

        /// <summary>
        /// Returns the result or fails if the result was an error.
        /// </summary>
#if ERRH_INLINE_METHODS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TSuccess SucceededWith<TSuccess, TMessage>(this Result<TSuccess, TMessage> result)
        {
            if (result.Tag == ResultType.Ok)
            {
                var ok = (Ok<TSuccess, TMessage>)result;
                return ok.Value.Success;
            }
            var bad = (Bad<TSuccess, TMessage>)result;
            throw new Exception(
                string.Format("Result was an error: {0}",
                string.Join(Environment.NewLine, bad.Messages.Select(m => m.ToString()))));
        }
    }
}
