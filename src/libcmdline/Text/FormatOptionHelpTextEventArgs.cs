#region License
// <copyright file="FormatOptionHelpTextEventArgs.cs" company="Giacomo Stelluti Scala">
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
#endregion

namespace CommandLine.Text
{
    /// <summary>
    /// Provides data for the FormatOptionHelpText event.
    /// </summary>
    public class FormatOptionHelpTextEventArgs : EventArgs
    {
        private readonly BaseOptionAttribute _option;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.FormatOptionHelpTextEventArgs"/> class.
        /// </summary>
        /// <param name="option">Option to format.</param>
        public FormatOptionHelpTextEventArgs(BaseOptionAttribute option)
        {
            _option = option;
        }

        /// <summary>
        /// Gets the option to format.
        /// </summary>
        public BaseOptionAttribute Option
        {
            get
            {
                return _option;
            }
        }
    }
}
