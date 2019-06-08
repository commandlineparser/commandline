using System;
using System.IO;


namespace CommandLine.Text
{
    /// <summary>
    /// Contains the configuration used when building HelpText
    /// </summary>
    /// <remarks>
    /// It is intended that this class should contain all settings/logic associated with configuring the
    /// display of helptext.
    /// </remarks>
    public class HelpTextConfiguration
    {
        /// <summary>
        /// The default console width
        /// </summary>
        public const int DefaultMaximumLength = 80; 

        /// <summary>
        /// The width of the display.  Text will wrap if this is exceeded.
        /// </summary>
        public readonly int DisplayWidth;

        /// <summary>
        /// Method used to set display options within the HelpText
        /// </summary>
        /// <remarks>
        /// In the current implementation, there is only one HelpText and it grows as verbs and options
        /// are scanned.  Hence any flags this method sets will apply to all text.  E.g. setting the 
        /// 'display enums' flag will set it for all options.</remarks>
        public readonly Action<HelpText> Configurer;

        /// <summary>
        /// The output for the HelpText
        /// </summary>
        /// <remarks>
        /// This was moved from ParserSettings because logically is part of the help-text generation phase.
        /// </remarks>
        public readonly TextWriter HelpWriter;

        /// <summary>
        /// Constructor - private to avoid too much reliance on the particular argument list 
        /// </summary>
        private HelpTextConfiguration(Action<HelpText> configurer, int displayWidth,TextWriter writer)
        {
            Configurer = configurer;
            DisplayWidth = displayWidth;
            HelpWriter = writer;
        }
       
        /// <summary>
        /// Default Configuration which will give acceptable results 
        /// </summary>
        public static HelpTextConfiguration Default { get; } = new HelpTextConfiguration(_ => { },DefaultMaximumLength,Console.Error);
       
        /// <summary>
        /// Sets the TextWriter
        /// </summary>
        /// <remarks>
        /// The client is expected to dispose of the writer
        /// </remarks>
        public HelpTextConfiguration WithHelpWriter(TextWriter writer)
        {
            return new HelpTextConfiguration(Configurer,DisplayWidth,writer);
        }
        /// <summary>
        /// Sets a different width of the help-text output
        /// </summary>
        public HelpTextConfiguration WithDisplayWidth(int maxDisplayWidth)
        {
            return new HelpTextConfiguration(Configurer,maxDisplayWidth,HelpWriter);
        }
        /// <summary>
        /// Allows the client to pass in an action which will be called to configure the HelpText class
        /// </summary>
        /// <remarks>
        /// The Parser constructs the HelpText object then calls this method at the earliest opportunity
        /// which allows various display flags and options to be set.
        /// </remarks>
        public HelpTextConfiguration WithConfigurer(Action<HelpText> func)
        {
            return new HelpTextConfiguration(func,DisplayWidth,HelpWriter);
        }
    }
}
