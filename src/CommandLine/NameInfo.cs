﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Linq;

namespace CommandLine
{
    /// <summary>
    /// Models name information, used in <see cref="CommandLine.Error"/> instances.
    /// </summary>
    public sealed class NameInfo : IEquatable<NameInfo>
    {
        /// <summary>
        /// Represents an empty name information. Used when <see cref="CommandLine.Error"/> are tied to values,
        /// rather than options.
        /// </summary>
        public static readonly NameInfo EmptyName = new NameInfo(string.Empty, new string[0]);
        private readonly string[] longNames;
        private readonly string shortName;

        internal NameInfo(string shortName)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");

            this.longNames = new string[0];
            this.shortName = shortName;
        }

        internal NameInfo(string shortName, string longName)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longName == null) throw new ArgumentNullException("longName");
            if (longName == string.Empty)
            {
                this.longNames = new string[0];
            }
            else
            {
                this.longNames = new [] { longName };
            }

            this.shortName = shortName;
        }

        internal NameInfo(string shortName, string[] longNames)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longNames == null) throw new ArgumentNullException("longNames");
            if (longNames.Any(x => x == null)) throw new ArgumentNullException("longNames");
            this.longNames = longNames;
            this.shortName = shortName;
        }

        /// <summary>
        /// Gets the short name of the name information.
        /// </summary>
        public string ShortName
        {
            get { return shortName; }
        }

        /// <summary>
        /// Gets the long name of the name information.
        /// </summary>
        public string[] LongNames
        {
            get { return longNames; }
        }

        /// <summary>
        /// Gets a formatted text with unified name information.
        /// </summary>
        public string NameText
        {
            get
            {
                return ShortName.Length > 0 && LongNames.Length > 0
                           ? ShortName + ", " + string.Join(", ", LongNames)
                           : ShortName.Length > 0
                                ? ShortName
                                : string.Join(", ", LongNames);
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as NameInfo;
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
            return CSharpx.EnumerableExtensions.Prepend(LongNames, ShortName).ToArray().GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="CommandLine.NameInfo"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLine.NameInfo"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="CommandLine.NameInfo"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(NameInfo other)
        {
            if (other == null)
            {
                return false;
            }

            return ShortName.Equals(other.ShortName) && LongNames.SequenceEqual(other.LongNames);
        }
    }
}
