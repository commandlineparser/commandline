// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;

namespace CommandLine
{
    /// <summary>
    /// Models an value specification, or better how to handle values not bound to options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValueAttribute : Attribute
    {
        private readonly int index;
        private int min;
        private int max;
        private object @default;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.ValueAttribute"/> class.
        /// </summary>
        public ValueAttribute(int index)
        {
            this.index = index;
            min = -1;
            max = -1;
        }

        /// <summary>
        /// Gets the position this option has on the command line.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a command line option is required.
        /// </summary>
        public bool Required
        {
            get; set;
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the lower range of items.
        /// </summary>
        /// <remarks>If not set, no lower range is enforced.</remarks>
        public int Min
        {
            get { return min; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                min = value;
            }
        }

        /// <summary>
        /// When applied to <see cref="System.Collections.Generic.IEnumerable{T}"/> properties defines
        /// the upper range of items.
        /// </summary>
        /// <remarks>If not set, no upper range is enforced.</remarks>
        public int Max
        {
            get { return max; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentNullException("value");
                }

                max = value;
            }
        }

        /// <summary>
        /// Gets or sets mapped property default value.
        /// </summary>
        public object Default
        {
            get { return @default; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                @default = value;
            }
        }
    }
}