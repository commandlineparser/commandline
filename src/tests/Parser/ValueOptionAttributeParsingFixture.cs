using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Mocks;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests
{
    /// <summary>
    /// [Enhancement] https://github.com/gsscoder/commandline/issues/33
    /// </summary>
    
    public class ValueOptionAttributeParsingFixture : CommandLineParserBaseFixture
    {
        [Fact]
        public void ValueOptionAttributeIsolatesNonOptionValues()
        {
            var options = new SimpleOptionsWithValueOption();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(
                new string[] { "--switch", "file.ext", "1000", "0.1234", "-s", "out.ext" }, options);

            result.Should().BeTrue();

            options.BooleanValue.Should().BeTrue();
            options.StringItem.Should().Be("file.ext");
            options.IntegerItem.Should().Be(1000);
            options.NullableDoubleItem.Should().Be(0.1234D);
            options.StringValue.Should().Be("out.ext");
        }
        
        [Fact]
        public void ValueOptionAttributeValuesAreNotMandatory()
        {
            var options = new SimpleOptionsWithValueOption();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(
                new string[] { "--switch" }, options);

            result.Should().BeTrue();

            options.BooleanValue.Should().BeTrue();
            options.StringItem.Should().BeNull();
            options.IntegerItem.Should().Be(0);
            options.NullableDoubleItem.Should().NotHaveValue();
        }

        [Fact]
        public void ValueOptionTakesPrecedenceOnValueListRegardlessDeclarationOrder()
        {
            var options = new SimpleOptionsWithValueOptionAndValueList();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(
                new string[] { "ofvalueoption", "-1234", "4321", "forvaluelist1", "forvaluelist2", "forvaluelist3" }, options);

            result.Should().BeTrue();

            options.StringItem.Should().Be("ofvalueoption");
            options.NullableInteger.Should().Be(-1234);
            options.UnsignedIntegerItem.Should().Be(4321U);
            options.Items[0].Should().Be("forvaluelist1");
            options.Items[1].Should().Be("forvaluelist2");
            options.Items[2].Should().Be("forvaluelist3");
        }

        [Fact]
        public void BetweenValueOptionsOrderMatters()
        {
            var options = new SimpleOptionsWithValueOptionAndValueList();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(
                new string[] { "4321", "ofvalueoption", "-1234", "forvaluelist1", "forvaluelist2", "forvaluelist3" }, options);

            result.Should().BeFalse();
        }
    }
}
