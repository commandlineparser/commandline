// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using CommandLine.Infrastructure;

namespace CommandLine
{
    /// <summary>
    /// Models an option specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : Attribute
    {
        private readonly string longName;
        private readonly string shortName;
        private string setName;
        private int min;
        private int max;
        private object defaultValue;
        private string helpText;
        private string metaValue;

        private OptionAttribute(string shortName, string longName)
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longName == null) throw new ArgumentNullException("longName");

            this.shortName = shortName;
            this.longName = longName;
            this.setName = string.Empty;
            this.min = -1;
            this.max = -1;
            this.helpText = string.Empty;
            this.metaValue = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// The default long name will be inferred from target property.
        /// </summary>
        public OptionAttribute()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the option.</param>
        public OptionAttribute(string longName)
            : this(string.Empty, longName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionAttribute(char shortName, string longName)
            : this(shortName.ToOneCharString(), longName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option..</param>
        public OptionAttribute(char shortName)
            : this(shortName.ToOneCharString(), string.Empty)
        {
        }

        /// <summary>
        /// Gets long name of this command line option. This name is usually a single english word.
        /// </summary>
        public string LongName
        {
            get { return this.longName; }
        }

        /// <summary>
        /// Gets a short name of this command line option, made of one character.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a command line option is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the option's mutually exclusive set name.
        /// </summary>
        public string SetName
        {
            get { return this.setName; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.setName = value;
            }
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the lower range of items.
        /// </summary>
        /// <remarks>If not set, no lower range is enforced.</remarks>
        public int Min
        {
            get { return this.min; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                this.min = value;
            }
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the upper range of items.
        /// </summary>
        /// <remarks>If not set, no upper range is enforced.</remarks>
        public int Max
        {
            get { return this.max; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                this.max = value;
            }
        }

        /// <summary>
        /// Gets or sets mapped property default value.
        /// </summary>
        public object DefaultValue
        {
            get { return this.defaultValue; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.defaultValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a short description of this command line option. Usually a sentence summary.
        /// </summary>
        public string HelpText
        {
            get { return this.helpText; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.helpText = value;
            }
        }

        /// <summary>
        /// Gets or sets mapped property meta value. Usually an uppercase hint of required value type.
        /// </summary>
        public string MetaValue
        {
            get { return this.metaValue; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.metaValue = value;
            }
        }
    }
}