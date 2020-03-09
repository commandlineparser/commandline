using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

//Issue #591
//When options class is only having explicit interface declarations, it should be detected as mutable.

namespace CommandLine.Tests.Unit
{
    public class Issue591ests
    {
        [Fact]
        public void Parse_option_with_only_explicit_interface_implementation()
        {
            string actual = string.Empty;

            var arguments = new[] { "--inputfile", "file2.txt" };
            var result = Parser.Default.ParseArguments<Options_With_Only_Explicit_Interface>(arguments);
            result.WithParsed(options => {
                actual = ((IInterface_With_Two_Scalar_Options)options).InputFile;
            });

            actual.Should().Be("file2.txt");
        }
    }
}
