#region License
// <copyright file="IParserSettings.cs" company="Giacomo Stelluti Scala">
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
using System.Globalization;
using System.IO;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Defines an interface that specifies a set of features to configure 
    /// of a type that implements <see cref="CommandLine.IParser"/>.
    /// </summary>
    public interface IParserSettings : IDisposable
    {
        /// <summary>
        /// Gets or sets a value indicating whether perform case sensitive comparisons.
        /// </summary>
        bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether set a mutually exclusive behavior.
        /// Default is set to false.
        /// </summary>
        bool MutuallyExclusive { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.TextWriter"/> used for help method output.
        /// Setting this property to null, will disable help screen.
        /// </summary>
        TextWriter HelpWriter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the parser shall move on to the next argument and ignore the given argument if it
        /// encounter an unknown arguments
        /// </summary>
        /// <value>
        /// <c>true</c> to allow parsing the arguments with different class options that do not have all the arguments.
        /// </value>
        /// <remarks>
        /// This allows fragmented version class parsing, useful for project with add-on where add-ons also requires command line arguments but
        /// when these are unknown by the main program at build time.
        /// </remarks>
        bool IgnoreUnknownArguments { get; set; }

        /// <summary>
        /// Gets or sets the culture used when parsing arguments to typed properties.
        /// </summary>
        /// <remarks>
        /// Default is CurrentCulture of <see cref="System.Threading.Thread.CurrentThread"/>.
        /// </remarks>
        CultureInfo ParsingCulture { get; set; }
    }
}