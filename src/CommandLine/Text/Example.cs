// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text
{
    public sealed class Example<T> : IEquatable<Example<T>>
    {
        private readonly string group;
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> formatStyles;
        private readonly T sample;

        public Example(string group, string helpText, IEnumerable<UnParserSettings> formatStyles, T sample)
        {
            this.group = group;
            this.helpText = helpText;
            this.formatStyles = formatStyles;
            this.sample = sample;
        }

        public Example(string helpText, IEnumerable<UnParserSettings> formatStyles, T sample)
            : this(string.Empty, helpText, formatStyles, sample)
        {  
        }

        public Example(string helpText, UnParserSettings formatStyle, T sample)
            : this(string.Empty, helpText, new[] { formatStyle }, sample)
        {
        }

        public Example(string helpText, T sample)
            : this(string.Empty, helpText, Enumerable.Empty<UnParserSettings>(), sample)
        {
        }

        public string Group
        {
            get { return group; }
        }

        public string HelpText
        {
            get { return helpText; }
        }

        public IEnumerable<UnParserSettings> FormatStyles
        {
            get { return this.formatStyles; }
        }

        public T Sample
        {
            get { return sample; }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><value>true</value> if the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>; otherwise, <value>false</value>.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as Example<T>;
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
            return new { Group, HelpText, FormatStyles, Sample }.GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="CommandLine.Text.Example{T}"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLine.Text.Example{T}"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="CommandLine.Text.Example{T}"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(Example<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Group.Equals(other.Group)
                && HelpText.Equals(other.HelpText)
                && FormatStyles.SequenceEqual(other.FormatStyles)
                && Sample.Equals(other.Sample);
        }
    }
}
