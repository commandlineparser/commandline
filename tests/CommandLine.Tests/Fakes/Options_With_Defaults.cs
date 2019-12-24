namespace CommandLine.Tests.Fakes
{
    class Options_With_Defaults
    {
        [Option(Default = 99)]
        public int P1 { get; set; }
        [Option()]
        public string P2 { get; set; }
        [Option(Default = 88)]
        public int P3 { get; set; }
        [Option(Default = Shapes.Square)]
        public Shapes P4 { get; set; }
    }
    class Nuulable_Options_With_Defaults
    {
        [Option(Default = 99)]
        public int? P1 { get; set; }
        [Option()]
        public string P2 { get; set; }
        [Option(Default = 88)]
        public int? P3 { get; set; }
        [Option(Default = Shapes.Square)]
        public Shapes? P4 { get; set; }
    }
}

