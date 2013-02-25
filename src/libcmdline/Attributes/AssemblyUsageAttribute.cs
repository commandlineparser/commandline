#region License
// <copyright file="AssemblyUsageAttribute.cs" company="Giacomo Stelluti Scala">
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
using System.Runtime.InteropServices;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Models a multiline assembly usage text.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false), ComVisible(false)]
    public sealed class AssemblyUsageAttribute : MultilineTextAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.AssemblyUsageAttribute"/> class
        /// with one line of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        public AssemblyUsageAttribute(string line1)
            : base(line1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.AssemblyUsageAttribute"/> class
        /// with two lines of text.
        /// </summary>
        /// <param name="line1">First line of usage text.</param>
        /// <param name="line2">Second line of usage text.</param>
        public AssemblyUsageAttribute(string line1, string line2)
            : base(line1, line2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.AssemblyUsageAttribute"/> class
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
        /// Initializes a new instance of the <see cref="CommandLine.AssemblyUsageAttribute"/> class
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
        /// Initializes a new instance of the <see cref="CommandLine.AssemblyUsageAttribute"/> class
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
