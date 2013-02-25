using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
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
                    with.UseHelpWriter(helpWriter);
                    with.EnableMutuallyExclusive();
                    with.IgnoreUnknownArguments();
                    with.UseCulture(new CultureInfo("ja-JP"));
                });

            // Than
            parser.Settings.CaseSensitive.Should().BeFalse();
            parser.Settings.HelpWriter.Should().Be(helpWriter);
            parser.Settings.MutuallyExclusive.Should().BeTrue();
            parser.Settings.IgnoreUnknownArguments.Should().BeTrue();
            parser.Settings.ParsingCulture.Should().Be(new CultureInfo("ja-JP"));
        }
    }
}
