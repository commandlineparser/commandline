using System;
using System.IO;
using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit
{
    public class HelpTextConfigurationTests
    {
        public enum AnEnum
        {
            Option1,
            Option2
        }
        // Options
        [Verb("run", HelpText = "a verb")]
        internal class Options
        {
            [Option]
            public AnEnum AnOption { get; set; }
        }


        // Test method (xUnit) which fails
        [Fact]
        public void ConfigurationIsAccepted()
        {
            var help = new StringWriter();
            var sut = new Parser(config =>
            {
                config.HelpWriter = help;
                config.MaximumDisplayWidth = 80;

                config.HelpTextConfiguration = HelpTextConfiguration.Default
                    .WithHelpWriter(help)
                    .WithDisplayWidth(50)
                    .WithConfigurer(h => h.AddEnumValuesToHelpText = true);
            });


            //There seems to a bug that prevents "help VERB" outputting anything if
            //the parser is only supplied one verb  so this test provides
            //Add_verb as a workaround
            sut.ParseArguments<Add_Verb,Options>(
                new[] {"help", "run",});
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines.Any(line=>line.Contains("Option1")).Should().BeTrue();
            lines.Any(line=>line.Contains("Option2")).Should().BeTrue();

        }

        [Fact]
        public void ConfigurationIsAcceptedUsingFluentAPI()
        {
            var help = new StringWriter();
            var sut =Parser.Default
                .SetDisplayWidth(80,WidthPolicy.FitToScreen)
                .SetTextWriter(help)
                .SetHelpTextConfiguration(h => h.AddEnumValuesToHelpText = true);

            //There seems to a bug that prevents "help VERB" outputting anything if
            //the parser is only supplied one verb  so this test provides
            //Add_verb as a workaround
            sut.ParseArguments<Add_Verb,Options>(
                new[] {"help", "run",});
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines.Any(line=>line.Contains("Option1")).Should().BeTrue();
            lines.Any(line=>line.Contains("Option2")).Should().BeTrue();

        }
    }
}
