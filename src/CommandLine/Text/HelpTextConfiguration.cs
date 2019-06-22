using System;
using System.IO;

namespace CommandLine.Text
{
    /// <summary>
    ///     Contains the configuration used when building HelpText
    /// </summary>
    /// <remarks>
    ///     It is intended that this class should contain all settings/logic associated with configuring the
    ///     display of helptext.
    /// </remarks>
    public class HelpTextConfiguration
    {
        /// <summary>
        ///     The default console width
        /// </summary>
        public const int DefaultMaximumLength = 80;

        /// <summary>
        ///     Constructor - private to avoid too much reliance on the particular argument list
        /// </summary>
        private HelpTextConfiguration(Action<HelpText> configurer, int displayWidth, TextWriter writer,
            bool autoVersion, bool autoHelp)
        {
            Configurer = configurer;
            DisplayWidth = displayWidth;
            HelpWriter = writer;
            AutoVersion = autoVersion;
            AutoHelp = autoHelp;
        }

        /// <summary>
        ///     The width of the display.  Text will wrap if this is exceeded.
        /// </summary>
        public int DisplayWidth { get; private set; }

        /// <summary>
        ///     Method used to set display options within the HelpText
        /// </summary>
        /// <remarks>
        ///     In the current implementation, there is only one HelpText and it grows as verbs and options
        ///     are scanned.  Hence any flags this method sets will apply to all text.  E.g. setting the
        ///     'display enums' flag will set it for all options.
        /// </remarks>
        public Action<HelpText> Configurer { get; private set; }

        /// <summary>
        ///     The output for the HelpText
        /// </summary>
        /// <remarks>
        ///     This was moved from ParserSettings because logically is part of the help-text generation phase.
        /// </remarks>
        public TextWriter HelpWriter { get; private set; }

        /// <summary>
        ///     Default Configuration which will give acceptable results
        /// </summary>
        public static HelpTextConfiguration Default { get; } =
            new HelpTextConfiguration(_ => { }, DefaultMaximumLength, Console.Error, true, true);

        public bool AutoHelp { get; private set; }
        public bool AutoVersion { get; private set; }

        private HelpTextConfiguration Copy()
        {
            return new HelpTextConfiguration(Configurer, DisplayWidth, HelpWriter, AutoVersion, AutoHelp);
        }

        /// <summary>
        ///     Sets the TextWriter
        /// </summary>
        /// <remarks>
        ///     The client is expected to dispose of the writer
        /// </remarks>
        public HelpTextConfiguration WithHelpWriter(TextWriter writer)
        {
            var c = Copy();
            c.HelpWriter = writer;
            return c;
        }

        /// <summary>
        ///     Sets a different width of the help-text output
        /// </summary>
        public HelpTextConfiguration WithDisplayWidth(int maxDisplayWidth)
        {
            var c = Copy();
            c.DisplayWidth = maxDisplayWidth;
            return c;
        }

        /// <summary>
        ///     Allows the client to pass in an action which will be called to configure the HelpText class
        /// </summary>
        /// <remarks>
        ///     The Parser constructs the HelpText object then calls this method at the earliest opportunity
        ///     which allows various display flags and options to be set.
        /// </remarks>
        public HelpTextConfiguration WithConfigurer(Action<HelpText> func)
        {
            var c = Copy();
            c.Configurer = func;
            return c;
        }

        public HelpTextConfiguration WithAutoVersion(bool enable)
        {
            var c = Copy();
            c.AutoVersion = enable;
            return c;
        }

        public HelpTextConfiguration WithAutoHelp(bool enable)
        {
            var c = Copy();
            c.AutoHelp = enable;
            return c;
        }
    }
}
