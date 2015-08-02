// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace CommandLine.Text
{
    /// <summary>
    /// Models a multiline assembly usage text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false), ComVisible(false)]
    public sealed class AssemblyUsageAttribute : MultilineTextAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.AssemblyUsageAttribute"/> class
        /// with one line of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        public AssemblyUsageAttribute(string line1)
            : base(line1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.AssemblyUsageAttribute"/> class
        /// with two lines of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        /// <param name="line2">Second line of usage text.</param>
        public AssemblyUsageAttribute(string line1, string line2)
            : base(line1, line2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.AssemblyUsageAttribute"/> class
        /// with three lines of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        /// <param name="line2">Second line of usage text.</param>
        /// <param name="line3">Third line of usage text.</param>
        public AssemblyUsageAttribute(string line1, string line2, string line3)
            : base(line1, line2, line3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.AssemblyUsageAttribute"/> class
        /// with four lines of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        /// <param name="line2">Second line of usage text.</param>
        /// <param name="line3">Third line of usage text.</param>
        /// <param name="line4">Fourth line of usage text.</param>
        public AssemblyUsageAttribute(string line1, string line2, string line3, string line4)
            : base(line1, line2, line3, line4)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.AssemblyUsageAttribute"/> class
        /// with five lines of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        /// <param name="line2">Second line of usage text.</param>
        /// <param name="line3">Third line of usage text.</param>
        /// <param name="line4">Fourth line of usage text.</param>
        /// <param name="line5">Fifth line of usage text.</param>
        public AssemblyUsageAttribute(string line1, string line2, string line3, string line4, string line5)
            : base(line1, line2, line3, line4, line5)
        {
        }
    }
}
