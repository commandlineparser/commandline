﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using CommandLine.Infrastructure;

using System;

namespace CommandLine
{
    /// <summary>
    /// Models an option specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionAttribute : BaseAttribute
    {
        private readonly string[] longNames;
        private readonly string shortName;
        private string setName;
        private bool flagCounter;
        private char separator;
        private string group=string.Empty;

        private OptionAttribute(string shortName, string[] longNames) : base()
        {
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (longNames == null) throw new ArgumentNullException("longNames");

            this.shortName = shortName;
            this.longNames = longNames;
            setName = string.Empty;
            separator = '\0';
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// The default long name will be inferred from target property.
        /// </summary>
        public OptionAttribute()
            : this(string.Empty, new string[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the option.</param>
        public OptionAttribute(string longName)
            : this(string.Empty, new []{ longName })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="longNames">The long name of the option.</param>
        public OptionAttribute(string[] longNames)
            : this(string.Empty, longNames)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionAttribute(char shortName, string longName)
            : this(shortName.ToOneCharString(), new []{ longName })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longNames">The long name of the option or null if not used.</param>
        public OptionAttribute(char shortName, string[] longNames)
            : this(shortName.ToOneCharString(), longNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option..</param>
        public OptionAttribute(char shortName)
            : this(shortName.ToOneCharString(), new string[0])
        {
        }

        /// <summary>
        /// Gets long name of this command line option. This name is usually a single english word.
        /// </summary>
        public string[] LongNames
        {
            get { return longNames; }
        }

        /// <summary>
        /// Gets a short name of this command line option, made of one character.
        /// </summary>
        public string ShortName
        {
            get { return shortName; }
        }

        /// <summary>
        /// Gets or sets the option's mutually exclusive set name.
        /// </summary>
        public string SetName
        {
            get { return setName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                setName = value;
            }
        }

        /// <summary>
        /// If true, this is an int option that counts how many times a flag was set (e.g. "-v -v -v" or "-vvv" would return 3).
        /// The property must be of type int (signed 32-bit integer).
        /// </summary>
        public bool FlagCounter
        {
            get { return flagCounter; }
            set { flagCounter = value; }
        }

        /// <summary>
        /// When applying attribute to <see cref="System.Collections.Generic.IEnumerable{T}"/> target properties,
        /// it allows you to split an argument and consume its content as a sequence.
        /// </summary>
        public char Separator
        {
            get { return separator; }
            set { separator = value; }
        }

        /// <summary>
        /// Gets or sets the option group name. When one or more options are grouped, at least one of them should have value. Required rules are ignored.
        /// </summary>
        public string Group
        {
            get { return group; }
            set { group = value; }
        }
    }
}
