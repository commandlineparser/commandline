namespace CommandLine.Tests.Fakes
{
    public interface IFakeInterfaceOptions
    {
        [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
        bool Verbose { get; set; }

        [Option(HelpText = "Input file.")]
        string InputFile { get; set; }
    }
}
