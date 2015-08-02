// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Text
{
    /// <summary>
    /// Models a command line usage example.
    /// </summary>
    public sealed class Example : IEquatable<Example>
    {
        private readonly string helpText;
        private readonly IEnumerable<UnParserSettings> formatStyles;
        private readonly object sample;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.Example"/> class.
        /// </summary>
        /// <param name="helpText">Example description.</param>
        /// <param name="formatStyles">A <see cref="CommandLine.UnParserSettings"/> instances sequence that defines command line arguments format.</param>
        /// <param name="sample">A sample instance.</param>
        public Example(string helpText, IEnumerable<UnParserSettings> formatStyles, object sample)
        {
            if (string.IsNullOrEmpty(helpText)) throw new ArgumentException("helpText can't be null or empty", "helpText");
            if (formatStyles == null) throw new ArgumentNullException("formatStyles");
            if (sample == null) throw new ArgumentNullException("sample");

            this.helpText = helpText;
            this.formatStyles = formatStyles;
            this.sample = sample;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.Example"/> class.
        /// </summary>
        /// <param name="helpText">Example description.</param>
        /// <param name="formatStyle">A <see cref="CommandLine.UnParserSettings"/> instance that defines command line arguments format.</param>
        /// <param name="sample">A sample instance.</param>
        public Example(string helpText, UnParserSettings formatStyle, object sample)
            : this(helpText, new[] { formatStyle }, sample)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.Example"/> class.
        /// </summary>
        /// <param name="helpText">Example description.</param>
        /// <param name="sample">A sample instance.</param>
        public Example(string helpText, object sample)
            : this(helpText, Enumerable.Empty<UnParserSettings>(), sample)
        {
        }

        /// <summary>
        /// Example description. 
        /// </summary>
        public string HelpText
        {
            get { return helpText; }
        }
        
        /// <summary>
        /// A sequence of format styles.
        /// </summary>
        public IEnumerable<UnParserSettings> FormatStyles
        {
            get { return this.formatStyles; }
        }

        /// <summary>
        /// A sample instance.
        /// </summary>
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
            return new { HelpText, FormatStyles, Sample }.GetHashCode();
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

            return HelpText.Equals(other.HelpText)
                && FormatStyles.SequenceEqual(other.FormatStyles)
                && Sample.Equals(other.Sample);
        }
    }

    static class ExampleExtensions
    {
        public static IEnumerable<UnParserSettings> GetFormatStylesOrDefault(this Example example)
        {
            return example.FormatStyles.Any()
                ? example.FormatStyles
                : new[] { new UnParserSettings { Consumed = true } };
        }
    }
}
