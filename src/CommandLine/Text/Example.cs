// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text
{
    public sealed class Example : IEquatable<Example>
    {
        private readonly string group;
        private readonly string groupDescription;
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> formatStyles;
        private readonly object sample;

        internal Example(string group, string groupDescription, string helpText, IEnumerable<UnParserSettings> formatStyles, object sample)
        {
            this.group = group;
            this.group = groupDescription;
            this.helpText = helpText;
            this.formatStyles = formatStyles;
            this.sample = sample;
        }

        public Example(string group, string helpText, IEnumerable<UnParserSettings> formatStyles, object sample)
            : this(group, string.Empty, helpText, formatStyles, sample)
        {
        }

        public Example(string helpText, IEnumerable<UnParserSettings> formatStyles, object sample)
            : this(string.Empty, helpText, formatStyles, sample)
        {  
        }

        public Example(string helpText, UnParserSettings formatStyle, object sample)
            : this(string.Empty, helpText, new[] { formatStyle }, sample)
        {
        }

        public Example(string helpText, object sample)
            : this(string.Empty, helpText, Enumerable.Empty<UnParserSettings>(), sample)
        {
        }

        public string Group
        {
            get { return group; }
        }

        internal string GroupDescription
        {
            get { return groupDescription }
        }

        public string HelpText
        {
            get { return helpText; }
        }

        public IEnumerable<UnParserSettings> FormatStyles
        {
            get { return this.formatStyles; }
        }

        public object Sample
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
            var other = obj as Example;
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
            return new { Group, GroupDescription, HelpText, FormatStyles, Sample }.GetHashCode();
        }

        /// <summary>
        /// Returns a value that indicates whether the current instance and a specified <see cref="CommandLine.Text.Example"/> have the same value.
        /// </summary>
        /// <param name="other">The <see cref="CommandLine.Text.Example"/> instance to compare.</param>
        /// <returns><value>true</value> if this instance of <see cref="CommandLine.Text.Example"/> and <paramref name="other"/> have the same value; otherwise, <value>false</value>.</returns>
        public bool Equals(Example other)
        {
            if (other == null)
            {
                return false;
            }

            return Group.Equals(other.Group)
                && GroupDescription.Equals(other.GroupDescription)
                && HelpText.Equals(other.HelpText)
                && FormatStyles.SequenceEqual(other.FormatStyles)
                && Sample.Equals(other.Sample);
        }
    }

    static class ExampleExtensions
    {
        public static Example WithGroupDescription(this Example example, string newGroupDescription)
        {
            return new Example(
                example.Group,
                newGroupDescription,
                example.HelpText,
                example.FormatStyles,
                example.Sample);
        }
    }
}
