// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine
{
    public sealed class TypeInfo
    {
        private readonly Type current;
        private readonly IEnumerable<Type> choices;

        private TypeInfo(Type current, IEnumerable<Type> choices)
        {
            this.current = current;
            this.choices = choices;
        }

        public Type Current
        {
            get { return this.current; }
        }

        public IEnumerable<Type> Choices
        {
            get { return this.choices; }
        }

        internal static TypeInfo Create(Type current)
        {
            return new TypeInfo(current, Enumerable.Empty<Type>());
        }

        internal static TypeInfo Create(Type current, IEnumerable<Type> choices)
        {
            return new TypeInfo(current, choices);
        }
    }

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
        private readonly TypeInfo typeInfo;

        internal ParserResult(IEnumerable<Error> errors, TypeInfo typeInfo)
        {
            this.tag = ParserResultType.NotParsed;
            this.typeInfo = typeInfo ?? TypeInfo.Create(typeof(T));
            Errors = errors ?? new Error[0];
            Value = default;
        }

        internal ParserResult(T value, TypeInfo typeInfo)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            this.tag = ParserResultType.Parsed;
            this.typeInfo = typeInfo ?? TypeInfo.Create(value.GetType());
            Errors = new Error[0];
        }

        /// <summary>
        /// Parser result type discriminator, defined as <see cref="CommandLine.ParserResultType"/> enumeration.
        /// </summary>
        public ParserResultType Tag
        {
            get { return this.tag; }
        }

        public TypeInfo TypeInfo
        {
            get { return typeInfo; }
        }

        /// <summary>
        /// Gets the instance with parsed values. If one or more errors occures, <see langword="default"/> is returned.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets the sequence of parsing errors. If there are no errors, then an empty IEnumerable is returned.
        /// </summary>
        public IEnumerable<Error> Errors { get; }
    }

    /// <summary>
    /// It contains an instance of type <typeparamref name="T"/> with parsed values.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public sealed class Parsed<T> : ParserResult<T>, IEquatable<Parsed<T>>
    {
        internal Parsed(T value, TypeInfo typeInfo)
            : base(value, typeInfo)
        {
        }

        internal Parsed(T value)
            : this(value, TypeInfo.Create(value.GetType()))
        {
        }


        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Parsed<T> other)
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
            return new { Tag, Value }.GetHashCode();
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
                && Value.Equals(other.Value);
        }
    }

    /// <summary>
    /// It contains a sequence of <see cref="CommandLine.Error"/>.
    /// </summary>
    /// <typeparam name="T">The type with attributes that define the syntax of parsing rules.</typeparam>
    public sealed class NotParsed<T> : ParserResult<T>, IEquatable<NotParsed<T>>
    {

        internal NotParsed(TypeInfo typeInfo, IEnumerable<Error> errors)
            : base(errors, typeInfo)
        {
        }


        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NotParsed<T> other)
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

            return this.Tag.Equals(other.Tag)
                && Errors.SequenceEqual(other.Errors);
        }
    }
}
