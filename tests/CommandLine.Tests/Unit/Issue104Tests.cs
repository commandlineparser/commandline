using System.Linq;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
using CommandLine.Text;

//Issue #104
//When outputting HelpText, the code will correctly list valid values for enum type options. However, if the option is a nullable enum type, then it will not list the valid values.

namespace CommandLine.Tests.Unit
{
    public class Issue104Tests
    {

        [Fact]
        public void Create_instance_with_enum_options_enabled_and_nullable_enum()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true, AddEnumValuesToHelpText = true, MaximumDisplayWidth = 80 }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Options_With_Nullable_Enum_Having_HelpText>(TypeInfo.Create(typeof(Options_With_Enum_Having_HelpText)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().BeEquivalentTo("pre-options");
            lines[1].Should().BeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].Should().BeEquivalentTo("--shape          Define a enum value here. Valid values: Circle, Square,");
            lines[3].Should().BeEquivalentTo("Triangle");
            lines[4].Should().BeEquivalentTo("--help           Display this help screen.");
            lines[5].Should().BeEquivalentTo("--version        Display version information.");
            lines[6].Should().BeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Help_with_enum_options_enabled_and_nullable_enum()
        {
            // Fixture setup
            // Exercize system 
            var args = "--help".Split();
            var sut = new Parser(config => config.HelpWriter = null);
            var parserResult = sut.ParseArguments<Options_With_Nullable_Enum_Having_HelpText>(args);
            HelpText helpText = null;
            parserResult.WithNotParsed(errors =>
            {
                // Use custom help text to ensure valid enum values are displayed
                helpText = HelpText.AutoBuild(parserResult);
                helpText.AddEnumValuesToHelpText = true;
                helpText.AddOptions(parserResult);
            });

            // Verify outcome

            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();
            lines[2].Should().BeEquivalentTo("--stringvalue    Define a string value here.");
            lines[3].Should().BeEquivalentTo("--shape          Define a enum value here. Valid values: Circle, Square,");
            lines[4].Should().BeEquivalentTo("Triangle");
            lines[5].Should().BeEquivalentTo("--help           Display this help screen.");
            lines[6].Should().BeEquivalentTo("--version        Display version information.");
            // Teardown
        }
    }

}
