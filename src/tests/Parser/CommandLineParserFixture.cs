#region License
//
// Command Line Library: CommandLineParserFixture.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2012 Giacomo Stelluti Scala
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
using System.Globalization;


#endregion
#region Using Directives
using System;
using System.IO;
using System.Threading;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
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
            bool result = base.Parser.ParseArguments(new string[] { "-s", "something" }, options);
            
            base.AssertParserSuccess(result);
            Assert.AreEqual("something", options.StringValue);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseStringIntegerBoolOptions()
        {
            var options = new SimpleOptions();
            bool result = base.Parser.ParseArguments(
                    new string[] { "-s", "another string", "-i100", "--switch" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("another string", options.StringValue);
            Assert.AreEqual(100, options.IntegerValue);
            Assert.AreEqual(true, options.BooleanValue);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseShortAdjacentOptions()
        {
            var options = new BooleanSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-ca", "-d65" }, options);

            base.AssertParserSuccess(result);
            Assert.IsTrue(options.BooleanThree);
            Assert.IsTrue(options.BooleanOne);
            Assert.IsFalse(options.BooleanTwo);
            Assert.AreEqual(65, options.NonBooleanValue);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseShortLongOptions()
        {
            var options = new BooleanSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-b", "--double=9" }, options);

            base.AssertParserSuccess(result);
            Assert.IsTrue(options.BooleanTwo);
            Assert.IsFalse(options.BooleanOne);
            Assert.IsFalse(options.BooleanThree);
            Assert.AreEqual(9, options.NonBooleanValue);
            Console.WriteLine(options);
        }
 
        [Test]
        public void ParseOptionList()
        {
            var options = new SimpleOptionsWithOptionList();
            bool result = base.Parser.ParseArguments(new string[] {
                                "-k", "string1:stringTwo:stringIII", "-s", "test-file.txt" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("string1", options.SearchKeywords[0]);
            Console.WriteLine(options.SearchKeywords[0]);
            Assert.AreEqual("stringTwo", options.SearchKeywords[1]);
            Console.WriteLine(options.SearchKeywords[1]);
            Assert.AreEqual("stringIII", options.SearchKeywords[2]);
            Console.WriteLine(options.SearchKeywords[2]);
            Assert.AreEqual("test-file.txt", options.StringValue);
            Console.WriteLine(options.StringValue);
        }

        #region #BUG0000
        [Test]
        public void ShortOptionRefusesEqualToken()
        {
            var options = new SimpleOptions();

            Assert.IsFalse(base.Parser.ParseArguments(new string[] { "-i=10" }, options));
            Console.WriteLine(options);
        }
        #endregion

        [Test]
        public void ParseEnumOptions()
        {
            var options = new SimpleOptionsWithEnum();

            bool result = base.Parser.ParseArguments(new string[] { "-s", "data.bin", "-a", "ReadWrite" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("data.bin", options.StringValue);
            Assert.AreEqual(FileAccess.ReadWrite, options.FileAccess);
            Console.WriteLine(options);
        }

        [Test]
        public void ParseCultureSpecificNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-d", "10,986" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(10.986, options.DoubleValue);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Test]
        public void ParseCultureSpecificNullableNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--n-double", "12,32982" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(12.32982, options.NullableDoubleValue);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Test]
        public void ParseOptionsWithDefaults()
        {
            var options = new SimpleOptionsWithDefaults();
            bool result = base.Parser.ParseArguments(new string[] {}, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("str", options.StringValue);
            Assert.AreEqual(9, options.IntegerValue);
            Assert.AreEqual(true, options.BooleanValue);
        }

        [Test]
        public void ParseOptionsWithDefaultArray()
        {
            var options = new SimpleOptionsWithDefaultArray();
            bool result = base.Parser.ParseArguments(new string[] { "-y", "4", "5", "6" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(new string[] { "a", "b", "c" }, options.StringArrayValue);
            Assert.AreEqual(new int[] { 4, 5, 6 }, options.IntegerArrayValue);
            Assert.AreEqual(new double[] { 1.1, 2.2, 3.3 }, options.DoubleArrayValue);
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
            bool result = base.Parser.ParseArguments(new string[] { "-x" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void ParsingNonExistentLongOptionFailsWithoutThrowingAnException()
        {
            var options = new SimpleOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--extend" }, options);

            base.AssertParserFailure(result);
        }
        #endregion

        #region #REQ0000
        [Test]
        public void DefaultParsingIsCaseSensitive()
        {
            ICommandLineParser local = new CommandLineParser();
            var options = new MixedCaseOptions();
            bool result = local.ParseArguments(new string[] { "-a", "alfa", "--beta-OPTION", "beta" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("alfa", options.AlfaValue);
            Assert.AreEqual("beta", options.BetaValue);
        }

        [Test]
        public void UsingWrongCaseWithDefaultFails()
        {
            ICommandLineParser local = new CommandLineParser();
            var options = new MixedCaseOptions();
            bool result = local.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void DisablingCaseSensitive()
        {
            ICommandLineParser local = new CommandLineParser(new CommandLineParserSettings(false)); //Ref.: #DGN0001
            var options = new MixedCaseOptions();
            bool result = local.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("alfa", options.AlfaValue);
            Assert.AreEqual("beta", options.BetaValue);
        }
        #endregion

        #region #BUG0003
        [Test]
        public void PassingNoValueToAStringTypeLongOptionFails()
        {
            var options = new SimpleOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--string" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToAByteTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--byte" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToAShortTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--short" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToAnIntegerTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--int" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToALongTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--long" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToAFloatTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--float" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToADoubleTypeLongOptionFails()
        {
            var options = new NumberSetOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--double" }, options);

            base.AssertParserFailure(result);
        }
        #endregion

        #region #REQ0001
        [Test]
        public void AllowSingleDashAsOptionInputValue()
        {
            var options = new SimpleOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--string", "-" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("-", options.StringValue);
        }

        [Test]
        public void AllowSingleDashAsNonOptionValue()
        {
            var options = new SimpleOptionsWithValueList();
            bool result = base.Parser.ParseArguments(new string[] { "-sparser.xml", "-", "--switch" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("parser.xml", options.StringValue);
            Assert.AreEqual(true, options.BooleanValue);
            Assert.AreEqual(1, options.Items.Count);
            Assert.AreEqual("-", options.Items[0]);
        }
        #endregion
    }
}