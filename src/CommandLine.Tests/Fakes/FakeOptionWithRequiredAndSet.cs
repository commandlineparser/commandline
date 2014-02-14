namespace CommandLine.Tests.Fakes
{
    public class FakeOptionWithRequiredAndSet {
        [Option("ftpurl", SetName = "SetA", Required = true)]
        public string FtpUrl { get; set; }

        [Option("weburl", SetName = "SetA",  Required = true)]
        public string WebUrl { get; set; }
    }
}