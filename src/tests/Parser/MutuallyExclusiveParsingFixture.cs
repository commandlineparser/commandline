#region License
//
// Command Line Library: MutuallyExclusiveParsingFixture.cs
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
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Tests
{
    public class MutuallyExclusiveParsingFixture : CommandLineParserBaseFixture
    {
        [Fact]
        public void ParsingOneMutuallyExclusiveOptionSucceeds()
        {
            var options = new OptionsWithDefaultSet();
            var parser = new CommandLineParser(new CommandLineParserSettings {MutuallyExclusive = true});
            var result = parser.ParseArguments(new string[] { "--file=mystuff.xml" }, options);

            result.Should().BeTrue();
            options.FileName.Should().Be("mystuff.xml");
        }

        [Fact]
        public void ParsingTwoMutuallyExclusiveOptionsFails()
        {
            var parser = new CommandLineParser(new CommandLineParserSettings { MutuallyExclusive = true });
            var options = new OptionsWithDefaultSet();
            var result = parser.ParseArguments(new string[] { "-i", "1", "--file=mystuff.xml" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParsingOneMutuallyExclusiveOptionWithAnotherOptionSucceeds()
        {
            var options = new OptionsWithDefaultSet();
            var parser = new CommandLineParser(new CommandLineParserSettings { MutuallyExclusive = true });
            var result = parser.ParseArguments(new string[] { "--file=mystuff.xml", "-v" }, options);
            
            result.Should().BeTrue();
            options.FileName.Should().Be("mystuff.xml");
            options.Verbose.Should().Be(true);
        }

        [Fact]
        public void ParsingTwoMutuallyExclusiveOptionsInTwoSetSucceeds()
        {
            var options = new OptionsWithMultipleSet();
            var parser = new CommandLineParser(new CommandLineParserSettings { MutuallyExclusive = true });
            var result = parser.ParseArguments(new string[] { "-g167", "--hue", "205" }, options);
            
            result.Should().BeTrue();
            options.Green.Should().Be((byte)167);
            options.Hue.Should().Be((short)205);
        }

        [Fact]
        public void ParsingThreeMutuallyExclusiveOptionsInTwoSetFails()
        {
            var parser = new CommandLineParser(new CommandLineParserSettings {MutuallyExclusive = true});
            var options = new OptionsWithMultipleSet();
            var result = parser.ParseArguments(new string[] { "-g167", "--hue", "205", "--saturation=37" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void ParsingMutuallyExclusiveOptionsAndRequiredOptionFails()
        {
            var options = new OptionsWithMultipleSetAndOneOption();
            var parser = new CommandLineParser(new CommandLineParserSettings { MutuallyExclusive = true });
            var result = parser.ParseArguments(new string[] { "-g167", "--hue", "205" }, options);
            
            result.Should().BeFalse();
        }

        [Fact]
        public void ParsingMutuallyExclusiveOptionsAndRequiredOptionSucceeds()
        {
            var options = new OptionsWithMultipleSetAndOneOption();
            var parser = new CommandLineParser(new CommandLineParserSettings { MutuallyExclusive = true });
            var result = parser.ParseArguments(new string[] { "-g100", "-h200", "-cRgbColorSet" }, options);
            
            result.Should().BeTrue();
            options.Green.Should().Be((byte)100);
            options.Hue.Should().Be((short)200);
            options.DefaultColorSet.Should().Be(ColorSet.RgbColorSet);
        }
    }
}