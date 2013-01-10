#region License
//
// Command Line Library: MultiLineTextAttribute.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
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
//
#endregion
#region Using Directives
using System;
using CommandLine.Internal;
#endregion

namespace CommandLine.Text
{
    /// <summary>
    /// Provides base properties for creating an attribute, used to define multiple lines of text.
    /// </summary>
    public abstract class MultiLineTextAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.MultiLineTextAttribute"/> derived type
        /// using one line of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        protected MultiLineTextAttribute(string line1)
        {
            Assumes.NotNullOrEmpty(line1, "line1");
            _line1 = line1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.MultiLineTextAttribute"/> derived type
        /// using two lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        protected MultiLineTextAttribute(string line1, string line2)
            : this(line1)
        {
            Assumes.NotNullOrEmpty(line2, "line2");
            _line2 = line2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.MultiLineTextAttribute"/> derived type
        /// using three lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        protected MultiLineTextAttribute(string line1, string line2, string line3)
            : this(line1, line2)
        {
            Assumes.NotNullOrEmpty(line3, "line3");
            _line3 = line3;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.MultiLineTextAttribute"/> derived type
        /// using four lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        /// <param name="line4">The fourth line of text.</param>
        protected MultiLineTextAttribute(string line1, string line2, string line3, string line4)
            : this(line1, line2, line3)
        {
            Assumes.NotNullOrEmpty(line4, "line4");
            _line4 = line4;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.MultiLineTextAttribute"/> derived type
        /// using five lines of text.
        /// </summary>
        /// <param name="line1">The first line of text.</param>
        /// <param name="line2">The second line of text.</param>
        /// <param name="line3">The third line of text.</param>
        /// <param name="line4">The fourth line of text.</param>
        /// <param name="line5">The fifth line of text.</param>
        protected MultiLineTextAttribute(string line1, string line2, string line3, string line4, string line5)
            : this(line1, line2, line3, line4)
        {
            Assumes.NotNullOrEmpty(line5, "line5");
            _line5 = line5;
        }

        internal void AddToHelpText(HelpText helpText, bool before)
        {
            if (before)
            {
                if (!string.IsNullOrEmpty(_line1)) { helpText.AddPreOptionsLine(_line1); }
                if (!string.IsNullOrEmpty(_line2)) { helpText.AddPreOptionsLine(_line2); }
                if (!string.IsNullOrEmpty(_line3)) { helpText.AddPreOptionsLine(_line3); }
                if (!string.IsNullOrEmpty(_line4)) { helpText.AddPreOptionsLine(_line4); }
                if (!string.IsNullOrEmpty(_line5)) { helpText.AddPreOptionsLine(_line5); }
            }
            else
            {
                if (!string.IsNullOrEmpty(_line1)) { helpText.AddPostOptionsLine(_line1); }
                if (!string.IsNullOrEmpty(_line2)) { helpText.AddPostOptionsLine(_line2); }
                if (!string.IsNullOrEmpty(_line3)) { helpText.AddPostOptionsLine(_line3); }
                if (!string.IsNullOrEmpty(_line4)) { helpText.AddPostOptionsLine(_line4); }
                if (!string.IsNullOrEmpty(_line5)) { helpText.AddPostOptionsLine(_line5); }
            }
        }

        private readonly string _line1;
        private readonly string _line2;
        private readonly string _line3;
        private readonly string _line4;
        private readonly string _line5;
    }
}
