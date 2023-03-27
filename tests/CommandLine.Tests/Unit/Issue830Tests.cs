using System.Collections.Generic;
using System.IO;
using CommandLine.Text;
using FluentAssertions;
using Xunit;

// Issue #830
// Allow private properties as options and usage
namespace CommandLine.Tests.Unit
{
    public class Issue830Tests
    {
        [Fact]
        public void Parse_options_with_private_value_and_option()
        {
            var expectedOptions = new Options
            {
                Option = "a",
                Value = "b"
            };

            var result = Parser.Default.ParseArguments<Options>(
                new[] { "b", "--opt", "a" });

            ((Parsed<Options>)result).Value.Should().BeEquivalentTo(expectedOptions);
        }

        [Fact]
        public void Print_private_usage()
        {
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            sut.ParseArguments<Options>(new string[] { "--help" });
            var result = help.ToString();

            var lines = result.ToLines().TrimStringArray();
            lines[3].Should().BeEquivalentTo("Do something very cool:");
            lines[4].Should().BeEquivalentTo("myApp.txt --opt test1 test2");
        }

        private class Options
        {
            public string Option { get => PrivateOption; set => PrivateOption = value; }

            public string Value { get => PrivateValue; set => PrivateValue = value; }

            [Option("opt", Required = true)]
            private string PrivateOption { get; set; }

            [Value(0, Required = true)]
            private string PrivateValue { get; set; }

            [Usage(ApplicationAlias = "myApp.txt")]
            private static IEnumerable<Example> PrivateUsage { get; } = new List<Example>
            {
                new Example("Do something very cool", new Options
                {
                    PrivateOption = "test1",
                    PrivateValue = "test2"
                })
            };
        }
    }
}
