namespace CommandLine.Tests.Fakes
{
    public interface IInterface_With_Two_Scalar_Options
    {
        [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
        bool Verbose { get; set; }

        [Option(HelpText = "Input file.")]
        string InputFile { get; set; }
    }
}
