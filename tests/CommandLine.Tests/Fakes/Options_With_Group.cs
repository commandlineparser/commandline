namespace CommandLine.Tests.Fakes
{
    public class Options_With_Group
    {
        [Option('v', "version")]
        public string Version { get; set; }

        [Option("option1", Group = "err-group")]
        public string Option1 { get; set; }

        [Option("option2", Group = "err-group")]
        public string Option2 { get; set; }
    }
}
