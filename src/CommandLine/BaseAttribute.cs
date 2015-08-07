// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine
{
    /// <summary>
    /// Models a base attribute to define command line syntax.
    /// </summary>
    public abstract class BaseAttribute : Attribute
    {
        private int min;
        private int max;
        private object @default;
        private string helpText;
        private string metaValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.BaseAttribute"/> class.
        /// </summary>
        protected internal BaseAttribute()
        {
            min = -1;
            max = -1;
            helpText = string.Empty;
            metaValue = string.Empty;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a command line option is required.
        /// </summary>
        public bool Required
        {
            get;
            set;
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

        /// <summary>
        /// Gets or sets a short description of this command line option. Usually a sentence summary.
        /// </summary>
        public string HelpText
        {
            get { return helpText; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                helpText = value;
            }
        }

        /// <summary>
        /// Gets or sets mapped property meta value. Usually an uppercase hint of required value type.
        /// </summary>
        public string MetaValue
        {
            get { return metaValue; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                metaValue = value;
            }
        }
    }
}
