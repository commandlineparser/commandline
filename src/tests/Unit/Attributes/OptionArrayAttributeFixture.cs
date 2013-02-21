using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using CommandLine.Tests.Fakes;
using FluentAssertions;

namespace CommandLine.Tests.Unit.Attributes
{
    public class OptionArrayAttributeFixture
    {
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/6
        /// </summary>
        [Fact]
        public void Should_correctly_parse_two_consecutive_arrays()
        {
            // Given
            var options = new OptionsWithTwoArrays();
            var parser = new Parser();
            var argumets = new[] { "--source", @"d:/document.docx", "--output", @"d:/document.xlsx",
                    "--headers", "1", "2", "3", "4",              // first array
                    "--content", "5", "6", "7", "8", "--verbose"  // second array
                };

            // When
            var result = parser.ParseArguments(argumets, options);

            // Than
            result.Should().BeTrue();
            options.Headers.Should().HaveCount(c => c == 4);
            options.Headers.Should().ContainInOrder(new uint[] { 1, 2, 3, 4 });
            options.Content.Should().HaveCount(c => c == 4);
            options.Content.Should().ContainInOrder(new uint[] { 5, 6, 7, 8 });
        }
    }
}
