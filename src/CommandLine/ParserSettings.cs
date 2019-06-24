// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Globalization;
using System.IO;

using CommandLine.Infrastructure;
using CommandLine.Text;


namespace CommandLine
{
    /// <summary>
    /// Provides settings for <see cref="CommandLine.Parser"/>. Once consumed cannot be reused.
    /// </summary>
    public class ParserSettings : IDisposable
    {
        private bool disposed;
        private bool caseSensitive;
        private bool caseInsensitiveEnumValues;
        private HelpTextConfiguration helpTextConfiguration = HelpTextConfiguration.Default;
        private bool ignoreUnknownArguments;
        private CultureInfo parsingCulture;
        private bool enableDashDash;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
        {
            caseSensitive = true;
            caseInsensitiveEnumValues = false;
            parsingCulture = CultureInfo.InvariantCulture;
            try
            {
                if (Console.WindowWidth >= 1)
                {
                    HelpTextConfiguration=HelpTextConfiguration.WithDisplayWidth(Console.WindowWidth);
                }
            }
            catch (IOException)
            {
            }
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
        /// Note that case insensitivity only applies to <i>parameters</i>, not the values
        /// assigned to them (for example, enum parsing).
        /// </summary>
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { PopsicleSetter.Set(Consumed, ref caseSensitive, value); }
        }

      
        /// <summary>
        /// Gets or sets a value indicating whether perform case sensitive comparisons of <i>values</i>.
        /// Note that case insensitivity only applies to <i>values</i>, not the parameters.
        /// </summary>
        public bool CaseInsensitiveEnumValues
        {
            get { return caseInsensitiveEnumValues; }
            set { PopsicleSetter.Set(Consumed, ref caseInsensitiveEnumValues, value); }
        }

        /// <summary>
        /// Gets or sets the culture used when parsing arguments to typed properties.
        /// </summary>
        /// <remarks>
        /// Default is invariant culture, <see cref="System.Globalization.CultureInfo.InvariantCulture"/>.
        /// </remarks>
        public CultureInfo ParsingCulture
        {
            get { return parsingCulture; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                PopsicleSetter.Set(Consumed, ref parsingCulture, value); 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.TextWriter"/> used for help method output.
        /// Setting this property to null, will disable help screen.
        /// </summary>
        /// <remarks>
        /// It is the caller's responsibility to dispose or close the <see cref="TextWriter"/>.
        /// </remarks>
        [Obsolete("Internal use only - prefer Parser.Default.SetHelpWriter")]
        public TextWriter HelpWriter
        {
            get { return HelpTextConfiguration.HelpWriter; }
            set { HelpTextConfiguration = HelpTextConfiguration.WithHelpWriter(value); }
        }

        /// <summary>
        /// Allows the HelpText to be configured
        /// </summary>
        /// <remarks>
        /// It is intended that any future HelpText configuration should be encapsulated in this object.
        /// </remarks>
        [Obsolete("Internal use only - prefer Parser.Default.SetHelpTextConfiguration")]
        public HelpTextConfiguration HelpTextConfiguration
        {
            get { return helpTextConfiguration; }
            set { PopsicleSetter.Set(Consumed, ref helpTextConfiguration, value); }
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
            get { return ignoreUnknownArguments; }
            set { PopsicleSetter.Set(Consumed, ref ignoreUnknownArguments, value); }
        }
     

        /// <summary>
        /// Gets or sets a value indicating whether enable double dash '--' syntax,
        /// that forces parsing of all subsequent tokens as values.
        /// </summary>
        public bool EnableDashDash
        {
            get { return enableDashDash; }
            set { PopsicleSetter.Set(Consumed, ref enableDashDash, value); }
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [Obsolete("Prefer Parser.Default.SetDisplayWidth")]
        public int MaximumDisplayWidth
        {
            get { return HelpTextConfiguration.DisplayWidth; }
            set { HelpTextConfiguration = HelpTextConfiguration.WithDisplayWidth(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether implicit option or verb 'version' should be supported.
        /// </summary>
        /// <remarks>
        /// Note that AutoVersion and AutoHelp straddle the line between being PARSER and HELP settings;
        /// the are used buy the Help-text system to generate output but but the parser itself to decide
        /// which verbs to accept.  For simplicity, they are stored in HelpTextConfiguration and proxied here
        /// </remarks>
        [Obsolete("Internal use only - prefer Parser.Default.SetAutoVersion")]
        public bool AutoVersion
        {
            get { return HelpTextConfiguration.AutoVersion; }
            set { HelpTextConfiguration = HelpTextConfiguration.WithAutoVersion(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether implicit option or verb 'help' should be supported.
        /// </summary>
        [Obsolete("Internal use only - prefer Parser.Default.SetAutoHelp")]
        public bool AutoHelp
        {
            get { return HelpTextConfiguration.AutoHelp; }
            set { HelpTextConfiguration = HelpTextConfiguration.WithAutoHelp(value); }
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
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                // Do not dispose HelpWriter. It is the caller's responsibility.

                disposed = true;
            }
        }
    }
}
