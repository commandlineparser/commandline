#region License
//
// Command Line Library: OptionArrayAttributeParsingFixture.cs
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
using System.Threading;
using System.Globalization;
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class OptionArrayAttributeParsingFixture : CommandLineParserBaseFixture
    {
        public OptionArrayAttributeParsingFixture() : base()
        {
        }

        [Test]
        public void ParseStringArrayOptionUsingShortName()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-z", "alfa", "beta", "gamma" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingLongName()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "--strarr", "alfa", "beta", "gamma" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingShortNameWithValueAdjacent()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-zapple", "kiwi" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "apple", "kiwi" }, options.StringArrayValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingLongNameWithEqualSign()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "--strarr=apple", "kiwi" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "apple", "kiwi" }, options.StringArrayValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingShortNameAndStringOptionAfter()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-z", "one", "two", "three", "-s", "after" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
            Assert.AreEqual("after", options.StringValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingShortNameAndStringOptionBefore()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-s", "before", "-z", "one", "two", "three" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("before", options.StringValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingShortNameWithOptionsBeforeAndAfter()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] {
                "-i", "191919", "-z", "one", "two", "three", "--switch", "--string=near" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(191919, options.IntegerValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
            Assert.IsTrue(options.BooleanValue);
            Assert.AreEqual("near", options.StringValue);
        }

        [Test]
        public void ParseStringArrayOptionUsingLongNameWithValueList()
        {
            var options = new SimpleOptionsWithArrayAndValueList();
            bool result = base.Parser.ParseArguments(new string[] {
                "-shere", "-i999", "--strarr=0", "1", "2", "3", "4", "5", "6", "7", "8", "9" , "--switch", "f1.xml", "f2.xml"}, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("here", options.StringValue);
            Assert.AreEqual(999, options.IntegerValue);
            base.AssertArrayItemEqual(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, options.StringArrayValue);
            Assert.IsTrue(options.BooleanValue);
            base.AssertArrayItemEqual(new string[] { "f1.xml", "f2.xml" }, options.Items);
        }

        [Test]
        public void PassingNoValueToAStringArrayOptionFails()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-z" }, options);

            base.AssertParserFailure(result);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] { "--strarr" }, options);

            base.AssertParserFailure(result);
        }

        /****************************************************************************************************/

        [Test]
        public void ParseIntegerArrayOptionUsingShortName()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-y", "1", "2", "3" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3 }, options.IntegerArrayValue);
        }

        [Test]
        public void PassingBadValueToAnIntegerArrayOptionFails()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-y", "one", "2", "3" }, options);

            base.AssertParserFailure(result);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] { "-yone", "2", "3" }, options);

            base.AssertParserFailure(result);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] { "--intarr", "1", "two", "3" }, options);

            base.AssertParserFailure(result);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] { "--intarr=1", "2", "three" }, options);

            base.AssertParserFailure(result);
        }


        [Test]
        public void PassingNoValueToAnIntegerArrayOptionFails()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-y" }, options);

            base.AssertParserFailure(result);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] { "--intarr" }, options);

            base.AssertParserFailure(result);
        }

        /****************************************************************************************************/

        [Test]
        public void ParseDoubleArrayOptionUsingShortName()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-q", "0.1", "2.3", "0.9" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new double[] { .1, 2.3, .9 }, options.DoubleArrayValue);
        }

        /****************************************************************************************************/

        [Test]
        public void ParseDifferentArraysTogether_CombinationOne()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] {
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four"
            }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
        }

        [Test]
        public void ParseDifferentArraysTogether_CombinationTwo()
        {
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string=after"
            }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            Assert.AreEqual("after", options.StringValue);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] {
                "--string", "before",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("before", options.StringValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-s", "near-the-center",
                "-z", "one", "two", "three", "four"
            }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            Assert.AreEqual("near-the-center", options.StringValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);

            options = new SimpleOptionsWithArray();
            result = base.Parser.ParseArguments(new string[] {
                "--switch",
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-i", "1234",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string", "I'm really playing with the parser!"
            }, options);

            base.AssertParserSuccess(result);
            Assert.IsTrue(options.BooleanValue);
            base.AssertArrayItemEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.AssertArrayItemEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            Assert.AreEqual(1234, options.IntegerValue);
            base.AssertArrayItemEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            Assert.AreEqual("I'm really playing with the parser!", options.StringValue);
        }

        /****************************************************************************************************/

        [Test]
        [ExpectedException(typeof(CommandLineParserException))]
        public void WillThrowExceptionIfOptionArrayAttributeBoundToStringWithShortName()
        {
            base.Parser.ParseArguments(new string[] { "-v", "a", "b", "c" }, new SimpleOptionsWithBadOptionArray());
        }

        [Test]
        [ExpectedException(typeof(CommandLineParserException))]
        public void WillThrowExceptionIfOptionArrayAttributeBoundToStringWithLongName()
        {
            base.Parser.ParseArguments(new string[] { "--bstrarr", "a", "b", "c" }, new SimpleOptionsWithBadOptionArray());
        }

        [Test]
        [ExpectedException(typeof(CommandLineParserException))]
        public void WillThrowExceptionIfOptionArrayAttributeBoundToIntegerWithShortName()
        {
            base.Parser.ParseArguments(new string[] { "-w", "1", "2", "3" }, new SimpleOptionsWithBadOptionArray());
        }

        [Test]
        [ExpectedException(typeof(CommandLineParserException))]
        public void WillThrowExceptionIfOptionArrayAttributeBoundToIntegerWithLongName()
        {
            base.Parser.ParseArguments(new string[] { "--bintarr", "1", "2", "3" }, new SimpleOptionsWithBadOptionArray());
        }

        [Test]
        public void ParseCultureSpecificNumber()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new SimpleOptionsWithArray();
            bool result = base.Parser.ParseArguments(new string[] { "-q", "1,2", "1,23", "1,234" }, options);

            base.AssertParserSuccess(result);
            base.AssertArrayItemEqual(new double[] { 1.2, 1.23, 1.234 }, options.DoubleArrayValue);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        /****************************************************************************************************/

        [Test]
        public void ParseTwoUIntConsecutiveArray()
        {
            var options = new OptionsWithUIntArray();
            var result = CommandLineParser.Default.ParseArguments(new string[] {
                "--somestr", "just a string",
                "--arrayone", "10", "20", "30", "40",
                "--arraytwo", "11", "22", "33", "44",
                "--somebool"
            }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("just a string", options.SomeStringValue);
            base.AssertArrayItemEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            base.AssertArrayItemEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            Assert.AreEqual(true, options.SomeBooleanValue);
        }

        [Test]
        public void ParseTwoUIntConsecutiveArrayUsingShortNames()
        {
            var options = new OptionsWithUIntArray();
            var result = CommandLineParser.Default.ParseArguments(new string[] {
                "-s", "just a string",
                "-o", "10", "20", "30", "40",
                "-t", "11", "22", "33", "44",
                "-b"
            }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("just a string", options.SomeStringValue);
            base.AssertArrayItemEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            base.AssertArrayItemEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            Assert.AreEqual(true, options.SomeBooleanValue);
        }
    }
}