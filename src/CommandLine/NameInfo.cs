// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Core;

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
        public static readonly NameInfo EmptyName = new NameInfo(string.Empty, string.Empty);
        private readonly string longName;
        private readonly string shortName;

        internal NameInfo(string shortName, string longName)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longName == null) throw new ArgumentNullException("longName");

            this.longName = longName;
            this.shortName = shortName;
        }

        /// <summary>
        /// Gets the short name of the name information.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
        }

        /// <summary>
        /// Gets the long name of the name information.
        /// </summary>
        public string LongName
        {
            get { return this.longName; }
        }

        /// <summary>
        /// Gets a formatted text with unified name information.
        /// </summary>
        public string NameText
        {
            get
            {
                return this.ShortName.Length > 0 && this.LongName.Length > 0
                           ? this.ShortName + ", " + this.LongName
                           : this.ShortName.Length > 0
                                ? this.ShortName
                                : this.LongName;
            }
        }

        internal static NameInfo FromOptionSpecification(OptionSpecification specification)
        {
            return new NameInfo(
                specification.LongName,
                specification.ShortName);
        }

        internal static NameInfo FromSpecification(Specification specification)
        {
            switch (specification.Tag)
            {
                case SpecificationType.Option:
                    return NameInfo.FromOptionSpecification((OptionSpecification)specification);
                default:
                    return NameInfo.EmptyName;
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
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <remarks>A hash code for the current <see cref="System.Object"/>.</remarks>
        public override int GetHashCode()
        {
            return this.ShortName.GetHashCode() ^ this.LongName.GetHashCode();
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

            return this.ShortName.Equals(other.ShortName) && this.LongName.Equals(other.LongName);
        }
    }
}