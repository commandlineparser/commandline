#region License
//
// Command Line Library: NullableTypesParsingFixture.cs
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
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class NullableTypesParsingFixture : CommandLineParserBaseFixture
    {
        public NullableTypesParsingFixture() : base()
        {
        }

        [Test]
        public void ParseNullableIntegerOption()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-i", "99" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(99, options.IntegerValue);

            options = new NullableTypesOptions();
            result = base.Parser.ParseArguments(new string[] { }, options);

            base.AssertParserSuccess(result);
            Assert.IsNull(options.IntegerValue);
        }

        [Test]
        public void PassingBadValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-i", "string-value" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToANullableIntegerOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-int" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void ParseNullableEnumerationOption()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--enum=ReadWrite" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(FileAccess.ReadWrite, options.EnumValue);

            options = new NullableTypesOptions();
            result = base.Parser.ParseArguments(new string[] { }, options);

            base.AssertParserSuccess(result);
            Assert.IsNull(options.EnumValue);
        }

        [Test]
        public void PassingBadValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-e", "Overwrite" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToANullableEnumerationOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--enum" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void ParseNullableDoubleOption()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-d9.999" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(9.999, options.DoubleValue);

            options = new NullableTypesOptions();
            result = base.Parser.ParseArguments(new string[] { }, options);

            base.AssertParserSuccess(result);
            Assert.IsNull(options.DoubleValue);
        }

        [Test]
        public void PassingBadValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--double", "9,999" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void PassingNoValueToANullableDoubleOptionFails()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "-d" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void ParseStringOptionAndNullableValueTypes()
        {
            var options = new NullableTypesOptions();
            bool result = base.Parser.ParseArguments(new string[] { "--string", "alone" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("alone", options.StringValue);

            options = new NullableTypesOptions();
            result = base.Parser.ParseArguments(
                new string[] { "-d1.789", "--int", "10099", "-stogether", "--enum", "Read" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(1.789, options.DoubleValue);
            Assert.AreEqual(10099, options.IntegerValue);
            Assert.AreEqual("together", options.StringValue);
            Assert.AreEqual(FileAccess.Read, options.EnumValue);
        }

    }
}