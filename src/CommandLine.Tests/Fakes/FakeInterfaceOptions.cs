namespace CommandLine.Tests.Fakes
{
    class FakeInterfaceOptions : IFakeInterfaceOptions
    {
        public bool Verbose { get; set; }

        public string InputFile { get; set; }

        [Option(HelpText = "Output file.")]
        public string OutputFile { get; set; }
    }
}
