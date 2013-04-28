// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core
{
    internal class StatePair<T> : IEquatable<StatePair<T>>
    {
        private readonly T value;
        private readonly IEnumerable<Error> errors;

        internal StatePair(T value, IEnumerable<Error> errors)
        {
            if (object.Equals(value, default(T))) throw new ArgumentNullException("value");
            if (errors == null) throw new ArgumentNullException("errors");

            this.value = value;
            this.errors = errors;
        }

        public T Value
        {
            get { return this.value; }
        }

        public IEnumerable<Error> Errors
        {
            get { return this.errors; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as StatePair<T>;
            if (other != null)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.Errors.GetHashCode();
        }

        public bool Equals(StatePair<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Value.Equals(other.Value) && this.Errors.SequenceEqual(other.Errors);
        }
    }

    internal static class StatePair
    {
        public static StatePair<T> Create<T>(T value, IEnumerable<Error> errors)
        {
            if (object.Equals(value, default(T))) throw new ArgumentNullException("value");
            if (errors == null) throw new ArgumentNullException("errors");

            return new StatePair<T>(value, errors);
        }

        public static StatePair<T2> MapValue<T1, T2>(
            this StatePair<T1> statePair,
            Func<T1, T2> func)
        {
            return new StatePair<T2>(func(statePair.Value), statePair.Errors);
        }
    }
}
