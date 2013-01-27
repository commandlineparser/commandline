#region License
//
// Command Line Library: CommandLineParserFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion
#region Using Directives
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using CommandLine.Tests.Mocks;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests
{
    public class CommandLineParserFixture : CommandLineParserBaseFixture
    {
        [Fact]
        public void WillThrowExceptionIfArgumentsArrayIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLineParser().ParseArguments(null, new SimpleOptions()));
        }

        [Fact]
        public void WillThrowExceptionIfOptionsInstanceIsNull()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLineParser().ParseArguments(new string[] {}, null));
        }

        [Fact]
        public void ParseStringOption()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-s", "something" }, options);
            
            result.Should().BeTrue();
            options.StringValue.Should().Be("something");
            Console.WriteLine(options);
        }

        [Fact]
        public void ParseStringIntegerBoolOptions()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(
                    new string[] { "-s", "another string", "-i100", "--switch" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("another string");
            options.IntegerValue.Should().Be(100);
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void ParseShortAdjacentOptions()
        {
            var options = new BooleanSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-ca", "-d65" }, options);

            result.Should().BeTrue();
            options.BooleanThree.Should().BeTrue();
            options.BooleanOne.Should().BeTrue();
            options.BooleanTwo.Should().BeFalse();
            options.NonBooleanValue.Should().Be(65D);
            Console.WriteLine(options);
        }

        [Fact]
        public void ParseShortLongOptions()
        {
            var options = new BooleanSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-b", "--double=9" }, options);

            result.Should().BeTrue();
            options.BooleanTwo.Should().BeTrue();
            options.BooleanOne.Should().BeFalse();
            options.BooleanThree.Should().BeFalse();
            options.NonBooleanValue.Should().Be(9D);
            Console.WriteLine(options);
        }
 
        [Fact]
        public void ParseOptionList()
        {
            var options = new SimpleOptionsWithOptionList();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] {
                                "-k", "string1:stringTwo:stringIII", "-s", "test-file.txt" }, options);

            result.Should().BeTrue();
            options.SearchKeywords[0].Should().Be("string1");
            Console.WriteLine(options.SearchKeywords[0]);
            options.SearchKeywords[1].Should().Be("stringTwo");
            Console.WriteLine(options.SearchKeywords[1]);
            options.SearchKeywords[2].Should().Be("stringIII");
            Console.WriteLine(options.SearchKeywords[2]);
            options.StringValue.Should().Be("test-file.txt");
            Console.WriteLine(options.StringValue);
        }

        #region #BUG0000
        [Fact]
        public void ShortOptionRefusesEqualToken()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i=10" }, options);
            result.Should().BeFalse();
            Console.WriteLine(options);
        }
        #endregion

        [Fact]
        public void ParseEnumOptions()
        {
            var options = new SimpleOptionsWithEnum();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-s", "data.bin", "-a", "ReadWrite" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("data.bin");
            options.FileAccess.Should().Be(FileAccess.ReadWrite);
            Console.WriteLine(options);
        }

        [Fact]
        public void ParseCultureSpecificNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-d", "10,986" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(10.986D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Fact]
        public void ParseCultureSpecificNullableNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--n-double", "12,32982" }, options);

            result.Should().BeTrue();
            options.NullableDoubleValue.Should().Be(12.32982D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Fact]
        public void ParseOptionsWithDefaults()
        {
            var options = new SimpleOptionsWithDefaults();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] {}, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("str");
            options.IntegerValue.Should().Be(9);
            options.BooleanValue.Should().BeTrue();
        }

        [Fact]
        public void ParseOptionsWithDefaultArray()
        {
            var options = new SimpleOptionsWithDefaultArray();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new [] { "-y", "4", "5", "6" }, options);

            result.Should().BeTrue();
            options.StringArrayValue.Should().Equal(new [] { "a", "b", "c" });
            options.IntegerArrayValue.Should().Equal(new [] { 4, 5, 6 });
            options.DoubleArrayValue.Should().Equal(new [] { 1.1, 2.2, 3.3 });
        }

        [Fact]
        public void ParseOptionsWithBadDefaults()
        {
            var options = new SimpleOptionsWithBadDefaults();
            Assert.Throws<CommandLineParserException>(
                () => new CommandLineParser().ParseArguments(new string[] {}, options));
        }

        #region #BUG0002
        [Fact]
        public void ParsingNonExistentShortOptionFailsWithoutThrowingAnException()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-x" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParsingNonExistentLongOptionFailsWithoutThrowingAnException()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--extend" }, options);

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0000
        [Fact]
        public void DefaultParsingIsCaseSensitive()
        {
            var parser = new CommandLineParser();
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-a", "alfa", "--beta-OPTION", "beta" }, options);

            result.Should().BeTrue();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }

        [Fact]
        public void UsingWrongCaseWithDefaultFails()
        {
            var parser = new CommandLineParser();
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void DisablingCaseSensitive()
        {
            var parser = new CommandLineParser(new CommandLineParserSettings(false)); //Ref.: #DGN0001
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            result.Should().BeTrue();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }
        #endregion

        #region #BUG0003
        [Fact]
        public void PassingNoValueToAStringTypeLongOptionFails()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--string" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToAByteTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--byte" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToAShortTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--short" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToAnIntegerTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--int" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToALongTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--long" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToAFloatTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--float" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingNoValueToADoubleTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--double" }, options);

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0001
        [Fact]
        public void AllowSingleDashAsOptionInputValue()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--string", "-" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("-");
        }

        [Fact]
        public void AllowSingleDashAsNonOptionValue()
        {
            var options = new SimpleOptionsWithValueList();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-sparser.xml", "-", "--switch" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("parser.xml");
            options.BooleanValue.Should().BeTrue();
            options.Items.Count.Should().Be(1);
            options.Items[0].Should().Be("-");
        }
        #endregion

        #region #BUG0004
        [Fact]
        public void ParseNegativeIntegerValue()
        {
            var options = new SimpleOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i", "-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle2()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle3()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--int", "-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle4()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--int=-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }


        [Fact]
        public void ParseNegativeFloatingPointValue()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-d", "-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void ParseNegativeFloatingPointValue_InputStyle2()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-d-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void ParseNegativeFloatingPointValue_InputStyle3()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--double", "-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void ParseNegativeFloatingPointValue_InputStyle4()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "--double=-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }
        #endregion

        #region #BUG0005
        [Fact]
        public void PassingShortValueToByteOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingIntegerValueToShortOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingLongValueToIntegerOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingFloatValueToLongOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void PassingDoubleValueToFloatOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLineParser();
            var result = parser.ParseArguments(new string[] { "-f", double.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }
        #endregion
    }
}