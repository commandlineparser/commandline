namespace CommandLine.Tests.Fakes
{
    internal class OptionsWithValueOptionExplicitIndex
    {
        [ValueOption(2)]
        public string A { get; set; }

        [ValueOption(1)]
        public string B { get; set; }

        [ValueOption(0)]
        public string C { get; set; }
    }
}
