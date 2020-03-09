namespace CommandLine.Tests.Fakes
{
    class Options_With_Only_Explicit_Interface : IInterface_With_Two_Scalar_Options
    {
        bool IInterface_With_Two_Scalar_Options.Verbose { get; set; }

        string IInterface_With_Two_Scalar_Options.InputFile { get; set; }
    }
}
