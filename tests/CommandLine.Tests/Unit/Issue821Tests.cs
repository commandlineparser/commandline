using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

//Issue #591
//When options class is only having explicit interface declarations, it should be detected as mutable.

namespace CommandLine.Tests.Unit
{
    public class Issue821Tests
    {
        [Verb("do")]
        private class EmptyDoOptions{
        };

        [Verb("nothing")]
        private class EmptyNothingOptions{
        };

        [Fact]
        public void not_parsed_should_produce_version_if_set_to_true()
        {
            StringBuilder output = new StringBuilder();
            ParserResult<object> result;
            using (StringWriter writer = new StringWriter(output))
            {
                void ConfigureParser(ParserSettings settings)
                {
                    settings.HelpWriter = writer;
                    settings.AutoVersion = true;
                }

                using (Parser parser = new Parser(ConfigureParser))                
                {
                    result = parser.ParseArguments<EmptyDoOptions, EmptyNothingOptions>(Array.Empty<string>());
                }
            }

            result.Should().BeOfType<NotParsed<object>>();
            string outputContent = output.ToString();
            outputContent.Should().Contain("version", "Version is set to true and must be contained");
        }

        [Fact]
        public void not_parsed_should_not_produce_version_if_set_to_false()
        {
            StringBuilder output = new StringBuilder();
            ParserResult<object> result;
            using (StringWriter writer = new StringWriter(output))
            {
                void ConfigureParser(ParserSettings settings)
                {
                    settings.HelpWriter = writer;
                    settings.AutoVersion = false;
                }

                using (Parser parser = new Parser(ConfigureParser))                
                {
                    result = parser.ParseArguments<EmptyDoOptions, EmptyNothingOptions>(Array.Empty<string>());
                }
            }

            result.Should().BeOfType<NotParsed<object>>();
            string outputContent = output.ToString();
            outputContent.Should().NotContain("version", "Version is set to false and must not be contained");
        }

        [Fact]
        public void not_parsed_should_produce_help_if_set_to_true()
        {
            StringBuilder output = new StringBuilder();
            ParserResult<object> result;
            using (StringWriter writer = new StringWriter(output))
            {
                void ConfigureParser(ParserSettings settings)
                {
                    settings.HelpWriter = writer;
                    settings.AutoHelp = true;
                }

                using (Parser parser = new Parser(ConfigureParser))                
                {
                    result = parser.ParseArguments<EmptyDoOptions, EmptyNothingOptions>(Array.Empty<string>());
                }
            }

            result.Should().BeOfType<NotParsed<object>>();
            string outputContent = output.ToString();
            outputContent.Should().Contain("help", "Help is set to true and must be contained");
        }

        [Fact]
        public void not_parsed_should_not_produce_help_if_set_to_false()
        {
            StringBuilder output = new StringBuilder();
            ParserResult<object> result;
            using (StringWriter writer = new StringWriter(output))
            {
                void ConfigureParser(ParserSettings settings)
                {
                    settings.HelpWriter = writer;
                    settings.AutoHelp = false;
                }

                using (Parser parser = new Parser(ConfigureParser))                
                {
                    result = parser.ParseArguments<EmptyDoOptions, EmptyNothingOptions>(Array.Empty<string>());
                }
            }

            result.Should().BeOfType<NotParsed<object>>();
            string outputContent = output.ToString();
            outputContent.Should().NotContain("help", "Help is set to false and must not be contained");
        }
    }
}
