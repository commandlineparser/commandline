using FluentAssertions;
using Xunit;

// Issue #776 and #797
// When IgnoreUnknownArguments is used and there are unknown arguments with explicitly assigned values, other arguments with explicit assigned values should not be influenced.
// The bug only occured when the value was the same for a known and an unknown argument.

namespace CommandLine.Tests.Unit
{
    public class Issue776Tests
    {
        [Theory]
        [InlineData("3")]
        [InlineData("4")]
        public void IgnoreUnknownArguments_should_work_for_all_values(string dummyValue)
        {
            var arguments = new[] { "--cols=4", $"--dummy={dummyValue}" };
            var result = new Parser(with => { with.IgnoreUnknownArguments = true; })
                .ParseArguments<Options>(arguments);

            Assert.Empty(result.Errors);
            Assert.Equal(ParserResultType.Parsed, result.Tag);

            result.WithParsed(options =>
            {
                options.Cols.Should().Be(4);
            });
        }

        private class Options
        {
            [Option("cols", Required = false)]
            public int Cols { get; set; }
        }
    }
}
