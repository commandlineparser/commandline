using Xunit;

namespace CommandLine.Tests.Unit.Examples
{
    public class TheBasics
    {
        public class SampleOptions
        {
            [Option('v', "verbose")]
            public bool Verbose { get; set; }
        }

        [Fact]
        public void obtain_parsed_options_like_this()
        {
            SampleOptions result = ((Parsed<SampleOptions>)Parser.Default.ParseArguments<SampleOptions>(new[] {"Sample.exe", "-v" })).Value;

            Assert.True(result.Verbose);
        }
    }
}
