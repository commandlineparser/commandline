namespace CommandLine.Tests.Fakes
{
    internal class OptionsWithValueOptionImplicitIndex
    {
        [ValueOption(0)]
        public string A { get; set; }

        [ValueOption(0)]
        public string B { get; set; }

        [ValueOption(0)]
        public string C { get; set; }
    }
}
