using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Unit.Attributes
{
    public class OptionAttributeFixture
    {
        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var options = new OptionsWithImplicitLongName();
            var parser = new CommandLine.Parser();
            var arguments = new[] {
                "--download", "something",
                "--up-load", "this",
                "-b", "1024"
            };

            // When
            var result = parser.ParseArguments(arguments, options);

            // Than
            result.Should().Be(true);
            options.Download.Should().Be("something");
            options.Upload.Should().Be("this");
            options.Bytes.Should().Be(1024);
        }
    }
}
