using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
using CommandLine.Text;

namespace CommandLine.Tests.Unit.Text
{
    public class HelpTextAutoBuildFix
    {
        [Fact]
        public void HelpText_with_AdditionalNewLineAfterOption_true_should_have_newline()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AdditionalNewLineAfterOption = true }
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToLines();

            lines[2].Should().BeEquivalentTo("  stringvalue        Define a string value here.");
            lines[3].Should().BeEquivalentTo(String.Empty);
            lines[4].Should().BeEquivalentTo("  s, shortandlong    Example with both short and long name.");
            lines[5].Should().BeEquivalentTo(String.Empty);
            lines[7].Should().BeEquivalentTo(String.Empty);
            lines[9].Should().BeEquivalentTo(String.Empty);
            lines[11].Should().BeEquivalentTo(String.Empty);
            lines[13].Should().BeEquivalentTo(String.Empty);
            lines[14].Should().BeEquivalentTo("  value pos. 0       Define a long value here.");
            // Teardown
        }

        [Fact]
        public void HelpText_with_AdditionalNewLineAfterOption_false_should_not_have_newline()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AdditionalNewLineAfterOption = false }
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToLines();

            lines[2].Should().BeEquivalentTo("  stringvalue        Define a string value here.");

            lines[3].Should().BeEquivalentTo("  s, shortandlong    Example with both short and long name.");
            lines[8].Should().BeEquivalentTo("  value pos. 0       Define a long value here.");
            // Teardown
        }
        [Fact]
        public void HelpText_with_by_default_should_include_help_version_option()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText ()
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines.Should().HaveCount(c => c ==7);
            lines.Should().Contain("  help               Display more information on a specific command.");
            lines.Should().Contain("  version            Display version information.");
            // Teardown
        }

        [Fact]
        public void HelpText_with_AutoHelp_false_should_hide_help_option()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AutoHelp = false,AutoVersion = false}
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines.Should().HaveCount(c => c ==5);
            lines.Should().NotContain("  help               Display more information on a specific command.");
            lines.Should().NotContain("  version            Display version information.");
            // Teardown
        }
    }
}
