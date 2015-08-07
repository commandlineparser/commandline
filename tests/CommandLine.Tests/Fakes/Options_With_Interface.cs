namespace CommandLine.Tests.Fakes
{
    class Options_With_Interface : IInterface_With_Two_Scalar_Options
    {
        public bool Verbose { get; set; }

        public string InputFile { get; set; }

        [Option(HelpText = "Output file.")]
        public string OutputFile { get; set; }
    }
}
