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
        const char OptionSwitch = 'o';
        [Fact]
        public void Create_mutable_instance_without_parameterless_ctor_should_not_fail()
        {
            const string optionValue = "val";

            var result = Parser.Default.ParseArguments<Options>(new string[] { $"-{OptionSwitch}", optionValue });

            Assert.Equal(ParserResultType.Parsed, result.Tag);
            Assert.NotNull(result.Value);
            Assert.Equal(optionValue, result.Value.Opt);
            Assert.Empty(result.Errors);
        }
        private class Options
        {
            public Options(string opt)
            {
                Opt = opt;
            }
            [Option(OptionSwitch, "opt", Required = false)]
            public string Opt { get; set; }
        }
    }
}
