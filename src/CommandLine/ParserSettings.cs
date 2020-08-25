// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Globalization;
using System.IO;

using CommandLine.Infrastructure;
using CSharpx;

namespace CommandLine
{
    /// <summary>
    /// Provides settings for <see cref="CommandLine.Parser"/>. Once consumed cannot be reused.
    /// </summary>
    public class ParserSettings : IDisposable
    {
        private const int DefaultMaximumLength = 80; // default console width

        private bool disposed;
        private bool caseSensitive;
        private bool caseInsensitiveEnumValues;
        private TextWriter helpWriter;
        private bool ignoreUnknownArguments;
        private bool autoHelp;
        private bool autoVersion;
        private CultureInfo parsingCulture;
        private Maybe<bool> enableDashDash;
        private int maximumDisplayWidth;
        private Maybe<bool> allowMultiInstance;
        private bool getoptMode;
        private Maybe<bool> posixlyCorrect;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSettings"/> class.
        /// </summary>
        public ParserSettings()
        {
            caseSensitive = true;
            caseInsensitiveEnumValues = false;
            autoHelp = true;
            autoVersion = true;
            parsingCulture = CultureInfo.InvariantCulture;
            maximumDisplayWidth = GetWindowWidth();
            getoptMode = false;
            enableDashDash = Maybe.Nothing<bool>();
            allowMultiInstance = Maybe.Nothing<bool>();
            posixlyCorrect = Maybe.Nothing<bool>();
        }

        private int GetWindowWidth()
        {

#if !NET40
            if (Console.IsOutputRedirected) return DefaultMaximumLength;
#endif
            var width = 1;
            try
            {
                width = Console.WindowWidth;
                if (width < 1)
                {
                    width = DefaultMaximumLength;
                }
            }           
            catch (Exception e) when (e is IOException || e is PlatformNotSupportedException || e is ArgumentOutOfRangeException)
            {
               width = DefaultMaximumLength;
            }
            return width;
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
        public TextWriter HelpWriter
        {
            get { return helpWriter; }
            set { PopsicleSetter.Set(Consumed, ref helpWriter, value); }
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
        /// Gets or sets a value indicating whether implicit option or verb 'help' should be supported.
        /// </summary>
        public bool AutoHelp
        {
            get { return autoHelp; }
            set { PopsicleSetter.Set(Consumed, ref autoHelp, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether implicit option or verb 'version' should be supported.
        /// </summary>
        public bool AutoVersion
        {
            get { return autoVersion; }
            set { PopsicleSetter.Set(Consumed, ref autoVersion, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether enable double dash '--' syntax,
        /// that forces parsing of all subsequent tokens as values.
        /// If GetoptMode is true, this defaults to true, but can be turned off by explicitly specifying EnableDashDash = false.
        /// </summary>
        public bool EnableDashDash
        {
            get => enableDashDash.MatchJust(out bool value) ? value : getoptMode;
            set => PopsicleSetter.Set(Consumed, ref enableDashDash, Maybe.Just(value));
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        public int MaximumDisplayWidth
        {
            get { return maximumDisplayWidth; }
            set { maximumDisplayWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether options are allowed to be specified multiple times.
        /// If GetoptMode is true, this defaults to true, but can be turned off by explicitly specifying AllowMultiInstance = false.
        /// </summary>
        public bool AllowMultiInstance
        {
            get => allowMultiInstance.MatchJust(out bool value) ? value : getoptMode;
            set => PopsicleSetter.Set(Consumed, ref allowMultiInstance, Maybe.Just(value));
        }

        /// <summary>
        /// Whether strict getopt-like processing is applied to option values; if true, AllowMultiInstance and EnableDashDash will default to true as well.
        /// </summary>
        public bool GetoptMode
        {
            get => getoptMode;
            set => PopsicleSetter.Set(Consumed, ref getoptMode, value);
        }

        /// <summary>
        /// Whether getopt-like processing should follow the POSIX rules (the equivalent of using the "+" prefix in the C getopt() call).
        /// If not explicitly set, will default to false unless the POSIXLY_CORRECT environment variable is set, in which case it will default to true.
        /// </summary>
        public bool PosixlyCorrect
        {
            get => posixlyCorrect.MapValueOrDefault(val => val, () => Environment.GetEnvironmentVariable("POSIXLY_CORRECT").ToBooleanLoose());
            set => PopsicleSetter.Set(Consumed, ref posixlyCorrect, Maybe.Just(value));
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
