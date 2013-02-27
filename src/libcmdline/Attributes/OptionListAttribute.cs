#region License
// <copyright file="OptionListAttribute.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Diagnostics.CodeAnalysis;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Models an option that can accept multiple values.
    /// Must be applied to a field compatible with an <see cref="System.Collections.Generic.IList&lt;T&gt;"/> interface
    /// of <see cref="System.String"/> instances.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class OptionListAttribute : BaseOptionAttribute
    {
        private const char DefaultSeparator = ':';

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionListAttribute"/> class.
        /// The default long name will be inferred from target property.
        /// </summary>
        public OptionListAttribute()
        {
            AutoLongName = true;

            Separator = DefaultSeparator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionListAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        public OptionListAttribute(char shortName)
            : base(shortName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionListAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionListAttribute(string longName)
            : base(null, longName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionListAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        public OptionListAttribute(char shortName, string longName)
            : base(shortName, longName)
        {
            Separator = DefaultSeparator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.OptionListAttribute"/> class.
        /// </summary>
        /// <param name="shortName">The short name of the option or null if not used.</param>
        /// <param name="longName">The long name of the option or null if not used.</param>
        /// <param name="separator">Values separator character.</param>
        public OptionListAttribute(char shortName, string longName, char separator)
            : base(shortName, longName)
        {
            Separator = separator;
        }

        /// <summary>
        /// Gets or sets the values separator character.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "The char Separator property matches shortName char constructor argument because the ShortName property is defined in BaseOptionAttribute as nullable char")]
        public char Separator
        {
            get;
            set;
        }
    }
}