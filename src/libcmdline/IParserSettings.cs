using System;
using System.Globalization;
using System.IO;

namespace CommandLine
{
    /// <summary>
    /// Defines an interface that specifies a set of features to configure the behaviour
    /// of a type that implements <see cref="CommandLine.IParser"/>.
    /// </summary>
    public interface IParserSettings : IDisposable
    {
        /// <summary>
        /// Gets or sets the case comparison behavior.
        /// Default is set to true.
        /// </summary>
        bool CaseSensitive { get; set; }

        /// <summary>
        /// Gets or sets the mutually exclusive behavior.
        /// Default is set to false.
        /// </summary>
        bool MutuallyExclusive { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="System.IO.TextWriter"/> used for help method output.
        /// Setting this property to null, will disable help screen.
        /// </summary>
        TextWriter HelpWriter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if the parser shall move on to the next argument and ignore the given argument if it
        /// encounter an unknown arguments
        /// </summary>
        /// <value>
        /// <c>true</c> to allow parsing the arguments with differents class options that do not have all the arguments.
        /// </value>
        /// <remarks>
        /// This allows fragmented version class parsing, useful for project with addon where addons also requires command line arguments but
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