// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Globalization;
using System.IO;

using CommandLine.Infrastructure;

namespace CommandLine
{
    /// <summary>
    /// Provides settings for <see cref="CommandLine.Parser"/>. Once consumed cannot be reused.
    /// </summary>
    public class ParserSettings : IDisposable
    {
        private bool disposed;
        private bool caseSensitive;
        private TextWriter helpWriter;
        private bool ignoreUnknownArguments;
        private CultureInfo parsingCulture;
        private bool enableDashDash;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
        {
            this.caseSensitive = true;
            this.parsingCulture = CultureInfo.InvariantCulture;
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
            get { return this.caseSensitive; }
            set { PopsicleSetter.Set(Consumed, ref this.caseSensitive, value); }
        }

        /// <summary>
        /// Gets or sets the culture used when parsing arguments to typed properties.
        /// </summary>
        /// <remarks>
        /// Default is invariant culture, <see cref="System.Globalization.CultureInfo.InvariantCulture"/>.
        /// </remarks>
        public CultureInfo ParsingCulture
        {
            get { return this.parsingCulture; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                PopsicleSetter.Set(Consumed, ref this.parsingCulture, value); 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.TextWriter"/> used for help method output.
        /// Setting this property to null, will disable help screen.
        /// </summary>
        public TextWriter HelpWriter
        {
            get { return this.helpWriter; }
            set { PopsicleSetter.Set(Consumed, ref this.helpWriter, value); }
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
            get { return this.ignoreUnknownArguments; }
            set { PopsicleSetter.Set(Consumed, ref this.ignoreUnknownArguments, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable double dash '--' syntax,
        /// that forces parsing of all subsequent tokens as values.
        /// </summary>
        public bool EnableDashDash
        {
            get { return this.enableDashDash; }
            set { PopsicleSetter.Set(Consumed, ref this.enableDashDash, value); }
        }

        internal StringComparer NameComparer
        {
            get
            {
                return CaseSensitive
                    ? StringComparer.Ordinal
                    : StringComparer.OrdinalIgnoreCase;
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
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (HelpWriter != null)
                {
                    this.helpWriter.Dispose();
                    this.helpWriter = null;
                }

                this.disposed = true;
            }
        }
    }
}