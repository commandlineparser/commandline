using System;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit
{
    #if NET7_0_OR_GREATER
    public class Issue862Tests
    {

        [Fact]
        public void Properties_with_required_modifier_are_required()
        {
            var arguments = Array.Empty<string>();
            var result = new Parser().ParseArguments<Options>(arguments);

            result.Errors.Should().ContainSingle(e => e.Tag == ErrorType.MissingRequiredOptionError);
            result.Tag.Should().Be(ParserResultType.NotParsed);
        }

        private class Options
        {
            [Option("cols")]
            public required int Cols { get; set; }
        }
    }

    #endif
}
