// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine
{
    public enum ParserResultType { Parsed, NotParsed }

    /// <summary>
    /// Models a parser result. It contains an instance of type <typeparamref name="T"/> with parsed values and
    /// a sequence of <see cref="CommandLine.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public abstract class ParserResult<T>
    {
        private readonly ParserResultType parserResultType;
        private readonly T value;
        private readonly IEnumerable<Type> verbTypes;

        internal ParserResult(ParserResultType parserResultType, T value, IEnumerable<Type> verbTypes)
        {
            this.parserResultType = parserResultType;
            this.value = value;
            this.verbTypes = verbTypes;
        }

        internal ParserResultType ParserResultType
        {
            get { return this.parserResultType; }
        }

        internal IEnumerable<Type> VerbTypes
        {
            get { return verbTypes; }
        }

        /// <summary>
        /// Gets the instance with parsed values.
        /// </summary>
        public T Value
        {
            get { return value; }
        }
    }

    public sealed class Parsed<T> : ParserResult<T>, IEquatable<Parsed<T>>
    {
        internal Parsed(T value, IEnumerable<Type> verbTypes)
            : base(ParserResultType.Parsed, value, verbTypes)
        {
        }

        internal Parsed(T value)
            : this(value, Enumerable.Empty<Type>())
        {
        }


        public new T Value
        {
            get { return base.Value; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Parsed<T>;
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
            return new { ParserResultType, Value, VerbTypes }.GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="CommandLine.Parsed{T}"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLine.Parsed{T}"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="CommandLine.Parsed{T}"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(Parsed<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return ParserResultType.Equals(other.ParserResultType)
                    && Value.Equals(other.Value)
                    && VerbTypes.SequenceEqual(other.VerbTypes);
        }
    }

    public sealed class NotParsed<T> : ParserResult<T>, IEquatable<NotParsed<T>>
    {
        private readonly IEnumerable<Error> errors;

        internal NotParsed(T value, IEnumerable<Type> verbTypes, IEnumerable<Error> errors)
            : base(ParserResultType.NotParsed, value, verbTypes)
        {
            this.errors = errors;
        }

        internal NotParsed(T value, IEnumerable<Error> errors)
            : this(value, Enumerable.Empty<Type>(), errors)
        {
        }


        /// <summary>
        /// Gets the sequence of parsing errors.
        /// </summary>
        public IEnumerable<Error> Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as NotParsed<T>;
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
            return new { Value, Errors }.GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="CommandLine.NotParsed{T}"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLine.NotParsed{T}"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="CommandLine.NotParsed{T}"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(NotParsed<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return ParserResultType.Equals(other.ParserResultType) && Errors.SequenceEqual(other.Errors);
        }
    }

    internal static class ParserResult
    {
        //public static ParserResult<T> Create<T>(T instance, IEnumerable<Error> errors)
        //{
        //    return Create(tag, instance, errors, Maybe.Nothing<IEnumerable<Type>>());
        //}

        //public static ParserResult<T> Create<T>(ParserResultType tag, T instance, IEnumerable<Error> errors, Maybe<IEnumerable<Type>> verbTypes)
        //{
        //    if (Equals(instance, default(T))) throw new ArgumentNullException("instance");
        //    if (errors == null) throw new ArgumentNullException("errors");
        //    if (verbTypes == null) throw new ArgumentNullException("verbTypes");

        //    return new ParserResult<T>(tag, instance, errors, verbTypes);
        //}

        public static NotParsed<T> MapErrors<T>(
            this NotParsed<T> parserResult,
            Func<IEnumerable<Error>, IEnumerable<Error>> func)
        {
            return new NotParsed<T>(parserResult.Value, func(parserResult.Errors));
        }
    }
}