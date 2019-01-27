
namespace CommandLine.Tests.Fakes
{
    class Options_With_InvalidDefaults
    {
        // Default of string and integer type property will also throw.

        [Option(Default = false)]
        public string FileName { get; set; }
    }
}
