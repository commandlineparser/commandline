namespace CommandLine.Tests.Fakes
{
    public class Options_With_Multiple_Groups
    {
        [Option('v', "version")]
        public string Version { get; set; }

        [Option("option11", Group = "err-group")]
        public string Option11 { get; set; }

        [Option("option12", Group = "err-group")]
        public string Option12 { get; set; }

        [Option("option21", Group = "err-group2")]
        public string Option21 { get; set; }

        [Option("option22", Group = "err-group2")]
        public string Option22 { get; set; }
    }
}
