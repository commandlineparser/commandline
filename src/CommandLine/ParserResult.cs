// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    /// <summary>
    /// Discriminator enumeration of <see cref="CommandLine.ParserResultType"/> derivates.
    /// </summary>
    public enum ParserResultType
    {
        /// <summary>
        /// Value of <see cref="CommandLine.Parsed{T}"/> type.
        /// </summary>
        Parsed,
        /// <summary>
        /// Value of <see cref="CommandLine.NotParsed{T}"/> type.
        /// </summary>
        NotParsed
    }

    /// <summary>
    /// Models a parser result. When inherited by <see cref="CommandLine.Parsed{T}"/>, it contains an instance of type <typeparamref name="T"/>
    /// with parsed values.
    /// When inherited by <see cref="CommandLine.NotParsed{T}"/>, it contains a sequence of <see cref="CommandLine.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public abstract class ParserResult<T>
    {
        private readonly ParserResultType tag;
        private readonly IEnumerable<Type> verbTypes;
        private readonly Type typeInfo;

        internal ParserResult(ParserResultType tag, Type typeInfo, IEnumerable<Type> verbTypes)
        {
            this.tag = tag;
            this.typeInfo = typeInfo;
            this.verbTypes = verbTypes;
        }

        /// <summary>
        /// Parser result type discriminator, defined as <see cref="CommandLine.ParserResultType"/> enumeration.
        /// </summary>
        public ParserResultType Tag
        {
            get { return this.tag; }
        }

        internal Type TypeInfo
        {
            get { return typeInfo; }
        }

        internal IEnumerable<Type> VerbTypes
        {
            get { return verbTypes; }
        }
    }

    /// <summary>
    /// It contains an instance of type <typeparamref name="T"/> with parsed values.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public sealed class Parsed<T> : ParserResult<T>, IEquatable<Parsed<T>>
    {
        private readonly T value;

        internal Parsed(T value, IEnumerable<Type> verbTypes)
            : base(ParserResultType.Parsed, value.GetType(), verbTypes)
            //: base(ParserResultType.Parsed, value, verbTypes)
        {
            this.value = value;
        }

        internal Parsed(T value)
            : this(value, Enumerable.Empty<Type>())
        {
        }

        /// <summary>
        /// Gets the instance with parsed values.
        /// </summary>
        public T Value
        {
            get { return value; }
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
            return new { Tag, Value, VerbTypes }.GetHashCode();
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

            return this.Tag.Equals(other.Tag)
                    && Value.Equals(other.Value)
                    && VerbTypes.SequenceEqual(other.VerbTypes);
        }
    }

    /// <summary>
    /// It contains a sequence of <see cref="CommandLine.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public sealed class NotParsed<T> : ParserResult<T>, IEquatable<NotParsed<T>>
    {
        private readonly IEnumerable<Error> errors;

        internal NotParsed(Type typeInfo, IEnumerable<Type> verbTypes, IEnumerable<Error> errors)
            : base(ParserResultType.NotParsed, typeInfo, verbTypes)
        {
            this.errors = errors;
        }

        internal NotParsed(Type typeInfo, IEnumerable<Error> errors)
            : this(typeInfo, Enumerable.Empty<Type>(), errors)
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
            //return new { Value, Errors }.GetHashCode();
            return new { Tag, Errors }.GetHashCode();
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

            return this.Tag.Equals(other.Tag) && Errors.SequenceEqual(other.Errors);
        }
    }

    partial class ParserResultExtensions
    {
        internal static ParserResult<T> MapErrors<T>(
            this ParserResult<T> parserResult,
            Func<IEnumerable<Error>, IEnumerable<Error>> func)
        {
            var notParsed = parserResult as NotParsed<T>;
            if (notParsed != null)
                return new NotParsed<T>(notParsed.TypeInfo, func(notParsed.Errors));
            return parserResult;
        }
    }
}