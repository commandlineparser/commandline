#region License
//
// Command Line Library: ParserConfigurator.cs
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
using System.Globalization;
using System.IO;
using CommandLine.Helpers;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides an API for configuring a <see cref="CommandLine.IParser"/> instance.
    /// </summary>
    public sealed class ParserConfigurator : IHideObjectMembers
    {
        /// <summary>
        /// Initialize a new instance of <see cref="CommandLine.ParserConfigurator"/> class.
        /// </summary>
        /// <param name="parser">The <see cref="CommandLine.IParser"/> instance that should be configured.</param>
        public ParserConfigurator(IParser parser)
        {
            _parser = parser;
        }

        /// <summary>
        /// Configures the parser to not be case sensitive.
        /// </summary>
        /// <returns>A reference to the current <see cref="CommandLine.ParserConfigurator"/>.</returns>
        public ParserConfigurator NoCaseSensitive()
        {
            _parser.Settings.CaseSensitive = false;
            return this;
        }

        /// <summary>
        /// Configures the parser to use the provided instance of <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="helpWriter">The <see cref="System.IO.TextWriter"/> used for help method output.</param>
        /// <returns>A reference to the current <see cref="CommandLine.ParserConfigurator"/>.</returns>
        public ParserConfigurator HelpWriter(TextWriter helpWriter)
        {
            Assumes.NotNull(helpWriter, "helpWriter");

            _parser.Settings.HelpWriter = helpWriter;
            return this;
        }

        /// <summary>
        /// Enables the parser to handle mutually exclusive options.
        /// </summary>
        /// <returns>A reference to the current <see cref="CommandLine.ParserConfigurator"/>.</returns>
        public ParserConfigurator EnableMutuallyExclusive()
        {
            _parser.Settings.MutuallyExclusive = true;
            return this;
        }

        /// <summary>
        /// Enables the parser to handle unknown arguments for plug-in scenario.
        /// </summary>
        /// <returns>A reference to the current <see cref="CommandLine.ParserConfigurator"/>.</returns>
        public ParserConfigurator IgnoreUnknownArguments()
        {
            _parser.Settings.IgnoreUnknownArguments = true;
            return this;
        }

        /// <summary>
        /// Forces the parser to override default culture setting. 
        /// </summary>
        /// <param name="parsingCulture">The parsing culture to use</param>
        /// <returns>A reference to the current <see cref="CommandLine.ParserConfigurator"/>.</returns>
        /// <remarks>
        /// Default is CurrentCulture of <see cref="System.Threading.Thread.CurrentThread"/>.
        /// </remarks>
        public ParserConfigurator UseCulture(CultureInfo parsingCulture)
        {
            _parser.Settings.ParsingCulture = parsingCulture;
            return this;
        }

        private readonly IParser _parser;
    }
}