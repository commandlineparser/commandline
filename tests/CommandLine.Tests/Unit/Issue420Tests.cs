using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace CommandLine.Tests.Unit
{
    public class Issue420Tests
    {
        // Test method (xUnit) which fails
        [Fact]
        public void parsing_of_enumerable_types_with_separator_chars_should_validate_value_is_present()
        {
            string[] args = { "-j", "1", "--categories", "--flag" };

            ParserResult<Options> parsedOptions = Parser.Default.ParseArguments<Options>(args);

            parsedOptions.Tag.Should().Be(ParserResultType.NotParsed);

            ParserResult<OptionsWithSeparator> parsedOptionsWithSeparator = Parser.Default.ParseArguments<OptionsWithSeparator>(args);

            parsedOptionsWithSeparator.Tag.Should().Be(ParserResultType.NotParsed);
        }

        // Options
        internal class Options
        {
            [Option('f', "flag", HelpText = "Flag")]
            public bool Flag { get; set; }

            [Option('c', "categories", Required = false, HelpText = "Categories")]
            public IEnumerable<string> Categories { get; set; }

            [Option('j', "jobId", Required = true, HelpText = "Texts.ExplainJob")]
            public int JobId { get; set; }
        }

        // Options
        internal class OptionsWithSeparator
        {
            [Option('f', "flag", HelpText = "Flag")]
            public bool Flag { get; set; }

            [Option('c', "categories", Required = false, Separator = ',', HelpText = "Categories")]
            public IEnumerable<string> Categories { get; set; }

            [Option('j', "jobId", Required = true, HelpText = "Texts.ExplainJob")]
            public int JobId { get; set; }
        }
    }
}
