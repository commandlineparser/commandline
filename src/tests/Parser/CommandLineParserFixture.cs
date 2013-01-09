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
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class CommandLineParserFixture : CommandLineParserBaseFixture
    {
        public CommandLineParserFixture() : base()
        {
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowExceptionIfArgumentsArrayIsNull()
        {
            base.Parser.ParseArguments(null, new SimpleOptions());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WillThrowExceptionIfOptionsInstanceIsNull()
        {
            base.Parser.ParseArguments(new string[] { }, null);
        }

        [Test]
        public void ParseStringOption()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "-s", "something" }, options);
            
            ResultShouldBeTrue();
            options.StringValue.Should().Equal("something");
            Console.WriteLine(options);
        }

        [Test]
        public void ParseStringIntegerBoolOptions()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(
                    new string[] { "-s", "another string", "-i100", "--switch" }, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("another string");
            options.IntegerValue.Should().Equal(100);
            options.BooleanValue.Should().Be.True();
            Console.WriteLine(options);
        }

        [Test]
        public void ParseShortAdjacentOptions()
        {
            var options = new BooleanSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-ca", "-d65" }, options);

            ResultShouldBeTrue();
            options.BooleanThree.Should().Be.True();
            options.BooleanOne.Should().Be.True();
            options.BooleanTwo.Should().Be.False();
            options.NonBooleanValue.Should().Equal(65D);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseShortLongOptions()
        {
            var options = new BooleanSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-b", "--double=9" }, options);

            ResultShouldBeTrue();
            options.BooleanTwo.Should().Be.True();
            options.BooleanOne.Should().Be.False();
            options.BooleanThree.Should().Be.False();
            options.NonBooleanValue.Should().Equal(9D);
            Console.WriteLine(options);
        }
 
        [Test]
        public void ParseOptionList()
        {
            var options = new SimpleOptionsWithOptionList();
            Result = base.Parser.ParseArguments(new string[] {
                                "-k", "string1:stringTwo:stringIII", "-s", "test-file.txt" }, options);

            ResultShouldBeTrue();
            options.SearchKeywords[0].Should().Equal("string1");
            Console.WriteLine(options.SearchKeywords[0]);
            options.SearchKeywords[1].Should().Equal("stringTwo");
            Console.WriteLine(options.SearchKeywords[1]);
            options.SearchKeywords[2].Should().Equal("stringIII");
            Console.WriteLine(options.SearchKeywords[2]);
            options.StringValue.Should().Equal("test-file.txt");
            Console.WriteLine(options.StringValue);
        }

        #region #BUG0000
        [Test]
        public void ShortOptionRefusesEqualToken()
        {
            var options = new SimpleOptions();

            Result = base.Parser.ParseArguments(new string[] { "-i=10" }, options);
            ResultShouldBeFalse();
            Console.WriteLine(options);
        }
        #endregion

        [Test]
        public void ParseEnumOptions()
        {
            var options = new SimpleOptionsWithEnum();

            Result = base.Parser.ParseArguments(new string[] { "-s", "data.bin", "-a", "ReadWrite" }, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("data.bin");
            options.FileAccess.Should().Equal(FileAccess.ReadWrite);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseCultureSpecificNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-d", "10,986" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(10.986D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Test]
        public void ParseCultureSpecificNullableNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--n-double", "12,32982" }, options);

            ResultShouldBeTrue();
            options.NullableDoubleValue.Should().Equal(12.32982D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Test]
        public void ParseOptionsWithDefaults()
        {
            var options = new SimpleOptionsWithDefaults();
            Result = base.Parser.ParseArguments(new string[] {}, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("str");
            options.IntegerValue.Should().Equal(9);
            options.BooleanValue.Should().Be.True();
        }

        [Test]
        public void ParseOptionsWithDefaultArray()
        {
            var options = new SimpleOptionsWithDefaultArray();
            Result = base.Parser.ParseArguments(new string[] { "-y", "4", "5", "6" }, options);

            ResultShouldBeTrue();
            options.StringArrayValue.Should().Equal(new string[] { "a", "b", "c" });
            options.IntegerArrayValue.Should().Equal(new int[] { 4, 5, 6 });
            options.DoubleArrayValue.Should().Equal(new double[] { 1.1, 2.2, 3.3 });
        }

        [Test]
        [ExpectedException(typeof(CommandLineParserException))]
        public void ParseOptionsWithBadDefaults()
        {
            var options = new SimpleOptionsWithBadDefaults();
            base.Parser.ParseArguments(new string[] {}, options);
        }

        #region #BUG0002
        [Test]
        public void ParsingNonExistentShortOptionFailsWithoutThrowingAnException()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "-x" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void ParsingNonExistentLongOptionFailsWithoutThrowingAnException()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "--extend" }, options);

            ResultShouldBeFalse();
        }
        #endregion

        #region #REQ0000
        [Test]
        public void DefaultParsingIsCaseSensitive()
        {
            ICommandLineParser local = new CommandLineParser();
            var options = new MixedCaseOptions();
            Result = local.ParseArguments(new string[] { "-a", "alfa", "--beta-OPTION", "beta" }, options);

            ResultShouldBeTrue();
            options.AlfaValue.Should().Equal("alfa");
            options.BetaValue.Should().Equal("beta");
        }

        [Test]
        public void UsingWrongCaseWithDefaultFails()
        {
            ICommandLineParser local = new CommandLineParser();
            var options = new MixedCaseOptions();
            Result = local.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void DisablingCaseSensitive()
        {
            ICommandLineParser local = new CommandLineParser(new CommandLineParserSettings(false)); //Ref.: #DGN0001
            var options = new MixedCaseOptions();
            Result = local.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            ResultShouldBeTrue();
            options.AlfaValue.Should().Equal("alfa");
            options.BetaValue.Should().Equal("beta");
        }
        #endregion

        #region #BUG0003
        [Test]
        public void PassingNoValueToAStringTypeLongOptionFails()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "--string" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToAByteTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--byte" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToAShortTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--short" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToAnIntegerTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--int" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToALongTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--long" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToAFloatTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--float" }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingNoValueToADoubleTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--double" }, options);

            ResultShouldBeFalse();
        }
        #endregion

        #region #REQ0001
        [Test]
        public void AllowSingleDashAsOptionInputValue()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "--string", "-" }, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("-");
        }

        [Test]
        public void AllowSingleDashAsNonOptionValue()
        {
            var options = new SimpleOptionsWithValueList();
            Result = base.Parser.ParseArguments(new string[] { "-sparser.xml", "-", "--switch" }, options);

            ResultShouldBeTrue();
            options.StringValue.Should().Equal("parser.xml");
            options.BooleanValue.Should().Be.True();
            options.Items.Count.Should().Equal(1);
            options.Items[0].Should().Equal("-");
        }
        #endregion

        #region #BUG0004
        [Test]
        public void ParseNegativeIntegerValue()
        {
            var options = new SimpleOptions();
            Result = base.Parser.ParseArguments(new string[] { "-i", "-4096" }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Equal(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle2()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-i-4096" }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Equal(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle3()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--int", "-4096" }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Equal(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle4()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--int=-4096" }, options);

            ResultShouldBeTrue();
            options.IntegerValue.Should().Equal(-4096);
        }


        [Test]
        public void ParseNegativeFloatingPointValue()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-d", "-4096.1024" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(-4096.1024D);
        }

        [Test]
        public void ParseNegativeFloatingPointValue_InputStyle2()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-d-4096.1024" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(-4096.1024D);
        }

        [Test]
        public void ParseNegativeFloatingPointValue_InputStyle3()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--double", "-4096.1024" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(-4096.1024D);
        }

        [Test]
        public void ParseNegativeFloatingPointValue_InputStyle4()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "--double=-4096.1024" }, options);

            ResultShouldBeTrue();
            options.DoubleValue.Should().Equal(-4096.1024D);
        }
        #endregion

        #region #BUG0005
        [Test]
        public void PassingShortValueToByteOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingIntegerValueToShortOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingLongValueToIntegerOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingFloatValueToLongOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            ResultShouldBeFalse();
        }

        [Test]
        public void PassingDoubleValueToFloatOptionMustFailGracefully()
        {
            var options = new NumberSetOptions();
            Result = base.Parser.ParseArguments(new string[] { "-f", double.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            ResultShouldBeFalse();
        }
        #endregion
    }
}