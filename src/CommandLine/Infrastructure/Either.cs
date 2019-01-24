//Use project level define(s) when referencing with Paket.
//#define CSX_EITHER_INTERNAL // Uncomment this to set visibility to internal.
//#define CSX_REM_MAYBE_FUNC // Uncomment this to remove dependency to Maybe.cs.

using System;

namespace CSharpx
{
    #region Either Type
#if !CSX_EITHER_INTERNAL
    public
#endif
    enum EitherType
    {
        /// <summary>
        /// Failed computation case.
        /// </summary>
        Left,
        /// <summary>
        /// Sccessful computation case.
        /// </summary>
        Right
    }

#if !CSX_EITHER_INTERNAL
    public
#endif
    abstract class Either<TLeft, TRight>
    {
        private readonly EitherType tag;

        protected Either(EitherType tag)
        {
            this.tag = tag;
        }

        public EitherType Tag
        {
            get { return this.tag; }
        }

        #region Basic Match Methods
        public bool MatchLeft(out TLeft value)
        {
            value = Tag == EitherType.Left ? ((Left<TLeft, TRight>)this).Value : default(TLeft);
            return Tag == EitherType.Left;
        }

        public bool MatchRight(out TRight value)
        {
            value = Tag == EitherType.Right ? ((Right<TLeft, TRight>)this).Value : default(TRight);
            return Tag == EitherType.Right;
        }
        #endregion
    }

#if !CSX_EITHER_INTERNAL
    public
#endif
    sealed class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TLeft value;

        internal Left(TLeft value)
            : base(EitherType.Left)
        {
            this.value = value;
        }

        public TLeft Value
        {
            get { return value; }
        }
    }

#if !CSX_EITHER_INTERNAL
    public
#endif
    sealed class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        private readonly TRight value;

        internal Right(TRight value)
            : base(EitherType.Right)
        {
            this.value = value;
        }

        public TRight Value
        {
            get { return value; }
        }
    }
    #endregion

#if !CSX_EITHER_INTERNAL
    public
#endif
    static class Either
    {
        #region Value Case Constructors
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft value)
        {
            return new Left<TLeft, TRight>(value);
        }

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight value)
        {
            return new Right<TLeft, TRight>(value);
        }
        #endregion

        #region Monad
        /// <summary>
        /// Inject a value into the Either type, returning Right case.
        /// </summary>
        public static Either<string, TRight> Return<TRight>(TRight value)
        {
            return Either.Right<string, TRight>(value);
        }

        /// <summary>
        /// Fail with a message. Not part of mathematical definition of a monad.
        /// </summary>
        public static Either<string, TRight> Fail<TRight>(string message)
        {
            throw new Exception(message);
        }

        /// <summary>
        /// Monadic bind.
        /// </summary>
        public static Either<TLeft, TResult> Bind<TLeft, TRight, TResult>(Either<TLeft, TRight> either, Func<TRight, Either<TLeft, TResult>> func)
        {
            TRight right;
            if (either.MatchRight(out right))
            {
                return func(right);
            }
            return Either.Left<TLeft, TResult>(either.GetLeft());
        }
        #endregion

        #region Functor
        /// <summary>
        /// Transforms a Either's right value by using a specified mapping function.
        /// </summary>
        public static Either<TLeft, TResult> Map<TLeft, TRight, TResult>(Either<TLeft, TRight> either, Func<TRight, TResult> func)
        {
            TRight right;
            if (either.MatchRight(out right))
            {
                return Either.Right<TLeft, TResult>(func(right));
            }
            return Either.Left<TLeft, TResult>(either.GetLeft());
        }
        #endregion

        #region Bifunctor
        /// <summary>
        /// Maps both parts of a Either type. Applies the first function if Either is Left.
        /// Otherwise applies the second function.
        /// </summary>
        public static Either<TLeft1, TRight1> Bimap<TLeft, TRight, TLeft1, TRight1>(Either<TLeft, TRight> either, Func<TLeft, TLeft1> mapLeft, Func<TRight, TRight1> mapRight)
        {
            TRight right;
            if (either.MatchRight(out right))
            {
                return Either.Right<TLeft1, TRight1>(mapRight(right));
            }
            return Either.Left<TLeft1, TRight1>(mapLeft(either.GetLeft()));
        }
        #endregion

        #region Linq Operators
        /// <summary>
        /// Map operation compatible with Linq.
        /// </summary>
        public static Either<TLeft, TResult> Select<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> selector)
        {
            return Either.Map(either, selector);
        }

        public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TResult>(this Either<TLeft, TRight> result,
            Func<TRight, Either<TLeft, TResult>> func)
        {
            return Either.Bind(result, func);
        }
        #endregion

        /// <summary>
        /// Returns a Either Right or fail with an exception.
        /// </summary>
        public static TRight GetOrFail<TLeft, TRight>(Either<TLeft, TRight> either)
        {
            TRight value;
            if (either.MatchRight(out value))
                return value;
            throw new ArgumentException("either", string.Format("The either value was Left {0}", either));
        }

        /// <summary>
        /// Returns a Either Left or a defualt value.
        /// </summary>
        public static TLeft GetLeftOrDefault<TLeft, TRight>(Either<TLeft, TRight> either, TLeft @default)
        {
            TLeft value;
            return either.MatchLeft(out value) ? value : @default;
        }

        /// <summary>
        /// Returns a Either Right or a defualt value.
        /// </summary>
        public static TRight GetRightOrDefault<TLeft, TRight>(Either<TLeft, TRight> either, TRight @default)
        {
            TRight value;
            return either.MatchRight(out value) ? value : @default;
        }

        /// <summary>
        /// Wraps a function, encapsulates any exception thrown within to a Either.
        /// </summary>
        public static Either<Exception, TRight> Try<TRight>(Func<TRight> func)
        {
            try
            {
                return new Right<Exception, TRight>(func());
            }
            catch (Exception ex)
            {
                return new Left<Exception, TRight>(ex);
            }
        }

        /// <summary>
        /// Attempts to cast an object.
        /// Stores the cast value in 1Of2 if successful, otherwise stores the exception in 2Of2
        /// </summary>
        public static Either<Exception, TRight> Cast<TRight>(object obj)
        {
            return Either.Try(() => (TRight)obj);
        }

#if !CSX_REM_MAYBE_FUNC
        public static Either<TLeft, TRight> OfMaybe<TLeft, TRight>(Maybe<TRight> maybe, TLeft left)
        {
            if (maybe.Tag == MaybeType.Just)
            {
                return Either.Right<TLeft, TRight>(((Just<TRight>)maybe).Value);
            }
            return Either.Left<TLeft, TRight>(left);
        }
#endif

        private static TLeft GetLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return ((Left<TLeft, TRight>)either).Value;
        }
    }

#if !CSX_EITHER_INTERNAL
    public
#endif
    static class EitherExtensions
    {
        #region Alternative Match Methods
        public static void Match<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> ifLeft, Action<TRight> ifRight)
        {
            TLeft left;
            if (either.MatchLeft(out left))
            {
                ifLeft(left);
                return;
            }
            ifRight(((Right<TLeft, TRight>)either).Value);
        }
        #endregion

        /// <summary>
        /// Equivalent to monadic <see cref="CSharpx.Either.Return{TRight}"/> operation.
        /// Builds a <see cref="CSharpx.Right{TLeft, TRight}"/> value in case <paramref name="value"/> by default.
        /// </summary>
        public static Either<string, TRight> ToEither<TRight>(this TRight value)
        {
            return Either.Return<TRight>(value);
        }

        public static Either<TLeft, TResult> Bind<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, Either<TLeft, TResult>> func)
        {
            return Either.Bind(either, func);
        }

        public static Either<TLeft, TResult> Map<TLeft, TRight, TResult>(
            this Either<TLeft, TRight> either,
            Func<TRight, TResult> func)
        {
            return Either.Map(either, func);
        }

        public static Either<TLeft1, TRight1> Bimap<TLeft, TRight, TLeft1, TRight1>(
            this Either<TLeft, TRight> either,
            Func<TLeft, TLeft1> mapLeft,
            Func<TRight, TRight1> mapRight)
        {
            return Either.Bimap(either, mapLeft, mapRight);
        }

        public static bool IsLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return either.Tag == EitherType.Left;
        }

        public static bool IsRight<TLeft, TRight>(this Either<TLeft, TRight> either)
        {
            return either.Tag == EitherType.Right;
        }
    }
}