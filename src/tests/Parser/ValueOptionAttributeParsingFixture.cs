using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
using Should.Fluent;

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

            options.BooleanValue.Should().Be.True();
            options.StringItem.Should().Equal("file.ext");
            options.IntegerItem.Should().Equal(1000);
            options.NullableDoubleItem.Should().Equal(0.1234D);
            options.StringValue.Should().Equal("out.ext");
        }
        
        [Test]
        public void ValueOptionAttributeValuesAreNotMandatory()
        {
            var options = new SimpleOptionsWithValueOption();
            Result = base.Parser.ParseArguments(
                new string[] { "--switch" }, options);

            ResultShouldBeTrue();

            options.BooleanValue.Should().Be.True();
            options.StringItem.Should().Be.Null();
            options.IntegerItem.Should().Equal(0);
            options.NullableDoubleItem.Should().Be.Null();
        }

        [Test]
        public void ValueOptionTakesPrecedenceOnValueListRegardlessDeclarationOrder()
        {
            var options = new SimpleOptionsWithValueOptionAndValueList();
            Result = base.Parser.ParseArguments(
                new string[] { "ofvalueoption", "-1234", "4321", "forvaluelist1", "forvaluelist2", "forvaluelist3" }, options);

            ResultShouldBeTrue();

            options.StringItem.Should().Equal("ofvalueoption");
            options.NullableInteger.Should().Equal(-1234);
            options.UnsignedIntegerItem.Should().Equal(4321U);
            options.Items[0].Should().Equal("forvaluelist1");
            options.Items[1].Should().Equal("forvaluelist2");
            options.Items[2].Should().Equal("forvaluelist3");
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
