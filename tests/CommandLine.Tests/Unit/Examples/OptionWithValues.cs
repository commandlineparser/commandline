using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CommandLine.Tests.Unit.Examples
{
    public class OptionWithValues
    {
        public class SampleOptions
        {
            [Option('n', "name", Separator = ',')]
            public IEnumerable<string> Names { get; set; }
        }

        [Fact]
        public void supply_single_value()
        {
            Parser.Default.ParseArguments<SampleOptions>(new [] { "Sample.exe", "--n A"}).
                WithParsed(options =>
                {
                    Assert.Equal(new[] { "A" }, options.Names);
                });
        }

        [Fact]
        public void single_dash_produces_unexpected_result()
        {
            Parser.Default.ParseArguments<SampleOptions>(new[] { "Sample.exe", "-n A" }).
                WithParsed(options =>
                {
                    // [!] unexpected as it prepends a space
                    Assert.Equal(new[] { " A" }, options.Names);
                });
        }

        [Fact]
        public void supply_multiple_values()
        {
            Parser.Default.ParseArguments<SampleOptions>(new[] { "Sample.exe", "--n A,B,C" }).
                WithParsed(options =>
                {
                    Assert.Equal(new[] { "A", "B", "C" }, options.Names);
                });
        }

        [Fact]
        public void defaults_to_empty_array()
        {
            Parser.Default.ParseArguments<SampleOptions>(new[] { "Sample.exe" }).
                WithParsed(options =>
                {
                    Assert.Equal(new string[0], options.Names);
                });
        }

        public class SampleOptionsMissingSeparator
        {
            [Option('n', "name")]
            public IEnumerable<string> Names { get; set; }
        }

        [Fact]
        public void you_must_supply_option_separator_otherwise_you_get_blank_results()
        {
            Parser.Default.ParseArguments<SampleOptionsMissingSeparator>(new[] { "Sample.exe", "--n A" }).
                WithParsed(options =>
                {
                    Assert.Equal(new string[0], options.Names);
                }).WithNotParsed(errors =>
                {
                    Assert.Equal(ErrorType.UnknownOptionError, errors.Single().Tag);
                });
        }
    }
}
