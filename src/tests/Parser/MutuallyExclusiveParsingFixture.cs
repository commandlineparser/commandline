#region License
//
// Command Line Library: MutuallyExclusiveParsingFixture.cs
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
using CommandLine.Tests.Mocks;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    public sealed class MutuallyExclusiveParsingFixture : CommandLineParserBaseFixture
    {
        public MutuallyExclusiveParsingFixture() : base() {}

        public override void CreateInstance()
        {
            Parser = new CommandLineParser(new CommandLineParserSettings {MutuallyExclusive = true});
        }

        [Test]
        public void ParsingOneMutuallyExclusiveOptionSucceeds()
        {
            var options = new OptionsWithDefaultSet();
            Result = base.Parser.ParseArguments(new string[] { "--file=mystuff.xml" }, options);

            ResultShouldBeTrue();
            Assert.AreEqual("mystuff.xml", options.FileName);
        }

        [Test]
        public void ParsingTwoMutuallyExclusiveOptionsFails()
        {
            var options = new OptionsWithDefaultSet();
            Result = base.Parser.ParseArguments(new string[] { "-i", "1", "--file=mystuff.xml" }, options);
            
            ResultShouldBeFalse();
        }

        [Test]
        public void ParsingOneMutuallyExclusiveOptionWithAnotherOptionSucceeds()
        {
            var options = new OptionsWithDefaultSet();
            Result = base.Parser.ParseArguments(new string[] { "--file=mystuff.xml", "-v" }, options);
            
            ResultShouldBeTrue();
            Assert.AreEqual("mystuff.xml", options.FileName);
            Assert.AreEqual(true, options.Verbose);
        }

        [Test]
        public void ParsingTwoMutuallyExclusiveOptionsInTwoSetSucceeds()
        {
            var options = new OptionsWithMultipleSet();
            Result = base.Parser.ParseArguments(new string[] { "-g167", "--hue", "205" }, options);
            
            ResultShouldBeTrue();
            Assert.AreEqual(167, options.Green);
            Assert.AreEqual(205, options.Hue);
        }

        [Test]
        public void ParsingThreeMutuallyExclusiveOptionsInTwoSetFails()
        {
            var options = new OptionsWithMultipleSet();
            Result = base.Parser.ParseArguments(new string[] { "-g167", "--hue", "205", "--saturation=37" }, options);
            
            ResultShouldBeFalse();
        }

        [Test]
        public void ParsingMutuallyExclusiveOptionsAndRequiredOptionFails()
        {
            var options = new OptionsWithMultipleSetAndOneOption();
            Result = base.Parser.ParseArguments(new string[] { "-g167", "--hue", "205" }, options);
            
            ResultShouldBeFalse();
        }

        [Test]
        public void ParsingMutuallyExclusiveOptionsAndRequiredOptionSucceeds()
        {
            var options = new OptionsWithMultipleSetAndOneOption();
            Result = base.Parser.ParseArguments(new string[] { "-g100", "-h200", "-cRgbColorSet" }, options);
            
            ResultShouldBeTrue();
            Assert.AreEqual(100, options.Green);
            Assert.AreEqual(200, options.Hue);
            Assert.AreEqual(ColorSet.RgbColorSet, options.DefaultColorSet);
        }
    }
}