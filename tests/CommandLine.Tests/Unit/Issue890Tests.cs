using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

//Issue #890
//When options class is mutable but doesn't have a parameterless constructor parsing fails.

namespace CommandLine.Tests.Unit
{
    public class Issue890Tests
    {
        [Fact]
        public void Create_mutable_instance_without_parameterless_ctor_should_not_fail()
        {
            var result = Parser.Default.ParseArguments<Options>(new[] { "-a" });

            Assert.Equal(ParserResultType.Parsed, result.Tag);
            Assert.NotNull(result.Value);
            Assert.Equal("a", result.Value.Option);

            Assert.Empty(result.Errors);
        }
        private class Options
        {
            public Options(string option)
            {
                Option = option;
            }
            [Option("a", Required = false)]
            public string Option { get; set; }
        }
    }
}
