namespace CommandLine.Tests.Mocks
{
    internal class OptionsWithValueOptionExplicitIndex
    {
        [ValueOption(Index = 2)]
        public string A { get; set; }

        [ValueOption(Index = 1)]
        public string B { get; set; }

        [ValueOption(Index = 0)]
        public string C { get; set; }
    }
}
