#region License
// <copyright file="ParserSettings.cs" company="Giacomo Stelluti Scala">
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
using System.Threading;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides settings for <see cref="CommandLine.Parser"/>. Once consumed cannot be reused.
    /// </summary>
    public sealed class ParserSettings
    {
        private const bool CaseSensitiveDefault = true;
        private bool _disposed;
        private bool _caseSensitive;
        private bool _mutuallyExclusive;
        private bool _ignoreUnknownArguments;
        private TextWriter _helpWriter;
        private CultureInfo _parsingCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
            : this(CaseSensitiveDefault, false, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting the case comparison behavior.
        /// </summary>
        /// <param name="caseSensitive">If set to true, parsing will be case sensitive.</param>
        public ParserSettings(bool caseSensitive)
            : this(caseSensitive, false, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting the <see cref="System.IO.TextWriter"/> used for help method output.
        /// </summary>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// default <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        public ParserSettings(TextWriter helpWriter)
            : this(CaseSensitiveDefault, false, false, helpWriter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting case comparison and help output options.
        /// </summary>
        /// <param name="caseSensitive">If set to true, parsing will be case sensitive.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// default <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        public ParserSettings(bool caseSensitive, TextWriter helpWriter)
            : this(caseSensitive, false, false, helpWriter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting case comparison and mutually exclusive behaviors.
        /// </summary>
        /// <param name="caseSensitive">If set to true, parsing will be case sensitive.</param>
        /// <param name="mutuallyExclusive">If set to true, enable mutually exclusive behavior.</param>
        public ParserSettings(bool caseSensitive, bool mutuallyExclusive)
            : this(caseSensitive, mutuallyExclusive, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting case comparison, mutually exclusive behavior and help output option.
        /// </summary>
        /// <param name="caseSensitive">If set to true, parsing will be case sensitive.</param>
        /// <param name="mutuallyExclusive">If set to true, enable mutually exclusive behavior.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// default <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        public ParserSettings(bool caseSensitive, bool mutuallyExclusive, TextWriter helpWriter)
            : this(caseSensitive, mutuallyExclusive, false, helpWriter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class,
        /// setting case comparison, mutually exclusive behavior and help output option.
        /// </summary>
        /// <param name="caseSensitive">If set to true, parsing will be case sensitive.</param>
        /// <param name="mutuallyExclusive">If set to true, enable mutually exclusive behavior.</param>
        /// <param name="ignoreUnknownArguments">If set to true, allow the parser to skip unknown argument, otherwise return a parse failure</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// default <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        public ParserSettings(bool caseSensitive, bool mutuallyExclusive, bool ignoreUnknownArguments, TextWriter helpWriter)
        {
            CaseSensitive = caseSensitive;
            MutuallyExclusive = mutuallyExclusive;
            HelpWriter = helpWriter;
            IgnoreUnknownArguments = ignoreUnknownArguments;
            ParsingCulture = Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CommandLine.ParserSettings"/> class.
        /// </summary>
        ~ParserSettings()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether perform case sensitive comparisons.
        /// </summary>
        public bool CaseSensitive
        {
            get
            {
                return _caseSensitive;
            }

            set
            {
                PopsicleSetter.Set(Consumed, ref _caseSensitive, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether set a mutually exclusive behavior.
        /// Default is set to false.
        /// </summary>
        public bool MutuallyExclusive
        {
            get
            {
                return _mutuallyExclusive;
            }

            set
            {
                PopsicleSetter.Set(Consumed, ref _mutuallyExclusive, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.TextWriter"/> used for help method output.
        /// Setting this property to null, will disable help screen.
        /// </summary>
        public TextWriter HelpWriter
        {
            get
            {
                return _helpWriter;
            }

            set
            {
                PopsicleSetter.Set(Consumed, ref _helpWriter, value);
            }
        }

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
        public bool IgnoreUnknownArguments
        {
            get
            {
                return _ignoreUnknownArguments;
            }

            set
            {
                PopsicleSetter.Set(Consumed, ref _ignoreUnknownArguments, value);
            }
        }

        /// <summary>
        /// Gets or sets the culture used when parsing arguments to typed properties.
        /// </summary>
        /// <remarks>
        /// Default is CurrentCulture of <see cref="System.Threading.Thread.CurrentThread"/>.
        /// </remarks>
        public CultureInfo ParsingCulture
        {
            get
            {
                return _parsingCulture;
            }

            set
            {
                PopsicleSetter.Set(Consumed, ref _parsingCulture, value);
            }
        }

        internal bool Consumed { get; set; }

        /// <summary>
        /// Frees resources owned by the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_helpWriter != null)
                {
                    _helpWriter.Dispose();
                    _helpWriter = null;
                }

                _disposed = true;
            }
        }
    }
}