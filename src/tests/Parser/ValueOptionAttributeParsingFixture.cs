using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
using FluentAssertions;

namespace CommandLine.Tests
{
    /// <summary>
    /// [Enhancement] https://github.com/gsscoder/commandline/issues/33
    /// </summary>
    [TestFixture]
    public sealed class ValueOptionAttributeParsingFixture : CommandLineParserBaseFixture
    {
        [Test]
        public void ValueOptionAttributeIsolatesNonOptionValues()
        {
            var options = new SimpleOptionsWithValueOption();
            Result = base.Parser.ParseArguments(
                new string[] { "--switch", "file.ext", "1000", "0.1234", "-s", "out.ext" }, options);

            ResultShouldBeTrue();

            options.BooleanValue.Should().BeTrue();
            options.StringItem.Should().Be("file.ext");
            options.IntegerItem.Should().Be(1000);
            options.NullableDoubleItem.Should().Be(0.1234D);
            options.StringValue.Should().Be("out.ext");
        }
        
        [Test]
        public void ValueOptionAttributeValuesAreNotMandatory()
        {
            var options = new SimpleOptionsWithValueOption();
            Result = base.Parser.ParseArguments(
                new string[] { "--switch" }, options);

            ResultShouldBeTrue();

            options.BooleanValue.Should().BeTrue();
            options.StringItem.Should().BeNull();
            options.IntegerItem.Should().Be(0);
            options.NullableDoubleItem.Should().NotHaveValue();
        }

        [Test]
        public void ValueOptionTakesPrecedenceOnValueListRegardlessDeclarationOrder()
        {
            var options = new SimpleOptionsWithValueOptionAndValueList();
            Result = base.Parser.ParseArguments(
                new string[] { "ofvalueoption", "-1234", "4321", "forvaluelist1", "forvaluelist2", "forvaluelist3" }, options);

            ResultShouldBeTrue();

            options.StringItem.Should().Be("ofvalueoption");
            options.NullableInteger.Should().Be(-1234);
            options.UnsignedIntegerItem.Should().Be(4321U);
            options.Items[0].Should().Be("forvaluelist1");
            options.Items[1].Should().Be("forvaluelist2");
            options.Items[2].Should().Be("forvaluelist3");
        }

        [Test]
        public void BetweenValueOptionsOrderMatters()
        {
            var options = new SimpleOptionsWithValueOptionAndValueList();
            Result = base.Parser.ParseArguments(
                new string[] { "4321", "ofvalueoption", "-1234", "forvaluelist1", "forvaluelist2", "forvaluelist3" }, options);

            ResultShouldBeFalse();
        }
    }
}
