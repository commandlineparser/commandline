namespace CommandLine.Tests.Fakes
{
    class Options_With_SetName_That_Ends_With_Previous_SetName
    {
        [Option(SetName = "web")]
        public string WebUrl { get; set; }

        [Option(SetName = "theweb")]
        public string SomethingElse { get; set; }
    }
}
