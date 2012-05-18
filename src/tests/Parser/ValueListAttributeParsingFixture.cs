#region License
//
// Command Line Library: ValueListParsingFixture.cs
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
using CommandLine.Tests.Mocks;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class ValueListAttributeParsingFixture : CommandLineParserBaseFixture
    {
        public ValueListAttributeParsingFixture() : base()
        {
        }

        [Test]
        public void ValueListAttributeIsolatesNonOptionValues()
        {
            var options = new SimpleOptionsWithValueList();
            bool result = base.Parser.ParseArguments(
                new string[] { "--switch", "file1.ext", "file2.ext", "file3.ext", "-s", "out.ext" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("file1.ext", options.Items[0]);
            Assert.AreEqual("file2.ext", options.Items[1]);
            Assert.AreEqual("file3.ext", options.Items[2]);
            Assert.AreEqual("out.ext", options.StringValue);
            Assert.IsTrue(options.BooleanValue);
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemInsideBounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            bool result = base.Parser.ParseArguments(new string[] { "file.a", "file.b", "file.c" }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual("file.a", options.InputFilenames[0]);
            Assert.AreEqual("file.b", options.InputFilenames[1]);
            Assert.AreEqual("file.c", options.InputFilenames[2]);
            Assert.IsNull(options.OutputFile);
            Assert.IsFalse(options.Overwrite);
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemOutsideBounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            bool result = base.Parser.ParseArguments(
                    new string[] { "file.a", "file.b", "file.c", "file.d" }, options);

            base.AssertParserFailure(result);
        }

        [Test]
        public void ValueListWithMaxElemSetToZeroSucceeds()
        {
            var options = new OptionsWithValueListMaximumZero();
            bool result = base.Parser.ParseArguments(new string[] { }, options);

            base.AssertParserSuccess(result);
            Assert.AreEqual(0, options.Junk.Count);
            Console.WriteLine(options);
        }

        [Test]
        public void ValueListWithMaxElemSetToZeroFailes()
        {
            var options = new OptionsWithValueListMaximumZero();

            Assert.IsFalse(base.Parser.ParseArguments(new string[] { "some", "value" }, options));
        }
    }
}