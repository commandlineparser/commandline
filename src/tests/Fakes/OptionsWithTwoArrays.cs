namespace CommandLine.Tests.Fakes
{
    class OptionsWithTwoArrays
    {
        [Option('s', "source", Required = true, HelpText = "Source Word2007+ document path")]
        public string SourceDocument { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output Excel2007+ spreadsheet path")]
        public string OutputSpreadsheet { get; set; }

        [OptionArray('h', "headers", Required = true, HelpText = "List of coords to cells with headers")]
        public uint[] Headers { get; set; }

        [OptionArray('c', "content", Required = true, HelpText = "List of coords to cells with content")]
        public uint[] Content { get; set; }

        [Option('v', "verbose", Required = false, DefaultValue = false, HelpText = "Print details during execution")]
        public bool Verbose { get; set; }
    }
}
