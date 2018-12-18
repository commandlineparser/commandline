using System;
using System.Text;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests
{
    public class ParserTestsOfDefaultBadDataType
    {
       
        [Fact]
        public static void When_default_mismatch_datattype_it_should_fire_exception_with_msg_containing_option_property_name()
        {
            Exception ex = Assert.Throws<CommandLineException>(() =>
            {
                Parser.Default.ParseArguments<Simple_Options_with_default_of_bad_datatype>(new[] { "--ShortAndLong", "123" });
            });

            Assert.Contains("IntSequence", ex.Message);
        }
    }
}
