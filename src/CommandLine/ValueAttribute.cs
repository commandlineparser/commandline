// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;

namespace CommandLine
{
    /// <summary>
    /// Models an value specification, or better how to handle values not bound to options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValueAttribute : BaseAttribute
    {
        private readonly int index;
        private string metaName; 

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.ValueAttribute"/> class.
        /// </summary>
        public ValueAttribute(int index) : base()
        {
            this.index = index;
            this.metaName = string.Empty;
        }

        /// <summary>
        /// Gets the position this option has on the command line.
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// Gets or sets name of this positional value specification.
        /// </summary>
        public string MetaName
        {
            get { return metaName; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                metaName = value;
            }
        }
    }
}