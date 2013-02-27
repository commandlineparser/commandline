using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Fakes;

using FluentAssertions;

using Xunit;

namespace CommandLine.Tests.Unit.Attributes
{
    public class OptionListAttributeFixture
    {

        [Fact]
        public void Should_use_property_name_as_long_name_if_omitted()
        {
            // Given
            var options = new OptionsWithImplicitLongName();
            var parser = new CommandLine.Parser();
            var arguments = new[] {
                "--segments", "header.txt:body.txt:footer.txt"
            };

            // When
            var result = parser.ParseArguments(arguments, options);

            // Than
            result.Should().Be(true);
            options.Segments.Should().HaveCount(c => c == 3);
            options.Segments.Should().ContainInOrder(new[] { "header.txt", "body.txt", "footer.txt" });
        }
    }
}
