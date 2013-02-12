namespace CommandLine.Tests.Mocks
{
    internal class OptionsWithValueOptionImplicitIndex
    {
        [ValueOption]
        public string A { get; set; }

        [ValueOption]
        public string B { get; set; }

        [ValueOption]
        public string C { get; set; }
    }
}
