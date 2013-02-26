#region License
//
// Command Line Library: HelpOptionAttributeFixture.cs
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
using System.IO;
using CommandLine.Text;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Attributes
{
    public class HelpOptionAttributeFixture : ParserBaseFixture
    {
        #region Mock Objects
        private class MockOptions
        {
            [Option('i', "input", Required = true, HelpText = "Input file with equations, xml format (see manual).")]
            public string InputFile { get; set; }

            [Option('o', "output", Required=false, HelpText="Output file with results, otherwise standard output.")]
            public string OutputFile {get;set;}

            [Option("paralell", Required=false, HelpText="Paralellize processing in multiple threads.")]
            public bool ParalellizeProcessing{get;set;}

            [Option('v', null, Required=false, HelpText="Show detailed processing messages.")]
            public bool Verbose{get;set;}

            [HelpOption(HelpText="Display this screen.")]
            public string GetUsage()
            {
                var help = new HelpText(new HeadingInfo("MyProgram", "1.0"));
                help.Copyright = new CopyrightInfo("Authors, Inc.", 2007);
                help.AddPreOptionsLine("This software is under the terms of the XYZ License");
                help.AddPreOptionsLine("(http://license-text.org/show.cgi?xyz).");
                help.AddPreOptionsLine("Usage: myprog --input equations-file.xml -o result-file.xml");
                help.AddPreOptionsLine("       myprog -i equations-file.xml --paralell");
                help.AddPreOptionsLine("       myprog -i equations-file.xml -vo result-file.xml");
                help.AddOptions(this);
                return help;
            }
        }
        #endregion

        [Fact]
        public void Correct_input_not_activates_help()
        {
            var options = new MockOptions();
            var writer = new StringWriter();
            var parser = new CommandLine.Parser(with => with.HelpWriter = writer);
            var result = parser.ParseArguments(
                    new string[] { "-imath.xml", "-oresult.xml" }, options);

            result.Should().BeTrue();;
            writer.ToString().Length.Should().Be(0);
        }

        [Fact]
        public void Bad_input_activates_help()
        {
            var options = new MockOptions();
            var writer = new StringWriter();
            var parser = new CommandLine.Parser(with => with.HelpWriter = writer);
            var result = parser.ParseArguments(
                    new string[] { "math.xml", "-oresult.xml" }, options);

            result.Should().BeFalse();

            string helpText = writer.ToString();
            (helpText.Length > 0).Should().BeTrue();

            Console.Write(helpText);
        }

        [Fact]
        public void Explicit_help_activation()
        {
            var options = new MockOptions();
            var writer = new StringWriter();
            var parser = new CommandLine.Parser(with => with.HelpWriter = writer);
            var result = parser.ParseArguments(
                    new string[] { "--help" }, options);

            result.Should().BeFalse();

            string helpText = writer.ToString();
            (helpText.Length > 0).Should().BeTrue();
        }
    }
}
