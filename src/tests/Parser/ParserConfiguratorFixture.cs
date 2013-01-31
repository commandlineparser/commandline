using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests
{
    public class ParserConfiguratorFixture
    {
        [Fact]
        public void Should_be_configured_with_configurator()
        {
            // Given when
            var helpWriter = new StringWriter();
            var parser = new CommandLine.Parser(with =>
                {
                    with.NoCaseSensitive();
                    with.HelpWriter(helpWriter);
                    with.EnableMutuallyExclusive();
                    with.IgnoreUnknownArguments();
                });

            // Than
            parser.Settings.CaseSensitive.Should().BeFalse();
            parser.Settings.HelpWriter.Should().Be(helpWriter);
            parser.Settings.MutuallyExclusive.Should().BeTrue();
            parser.Settings.IgnoreUnknownArguments.Should().BeTrue();
        }
    }
}
