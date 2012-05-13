#region License
//
// Command Line Library: HelpOptionAttributeFixture.cs
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
using System.ComponentModel;
using System.IO;
using CommandLine.Text;
using NUnit.Framework;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class HelpOptionAttributeFixture : CommandLineParserBaseFixture
    {
        #region Mock Objects
        private class MockOptions
        {
            [Option("i", "input", Required = true, HelpText = "Input file with equations, xml format (see manual).")]
            public string InputFile { get; set; }

            [Option("o", "output", Required=false, HelpText="Output file with results, otherwise standard output.")]
            public string OutputFile {get;set;}

            [Option(null, "paralell", Required=false, HelpText="Paralellize processing in multiple threads.")]
            public bool ParalellizeProcessing{get;set;}

            [Option("v", null, Required=false, HelpText="Show detailed processing messages.")]
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

        [Test]
        public void CorrectInputNotActivatesHelp()
        {
            var options = new MockOptions();
            var writer = new StringWriter();

            bool result = base.Parser.ParseArguments(
                    new string[] { "-imath.xml", "-oresult.xml" }, options, writer);

            base.AssertParserSuccess(result);
            Assert.AreEqual(0, writer.ToString().Length);
        }

        [Test]
        public void BadInputActivatesHelp()
        {
            var options = new MockOptions();
            var writer = new StringWriter();

            bool result = base.Parser.ParseArguments(
                    new string[] { "math.xml", "-oresult.xml" }, options, writer);

            base.AssertParserFailure(result);

            string helpText = writer.ToString();
            Assert.IsTrue(helpText.Length > 0);

            Console.Write(helpText);
        }

        [Test]
        public void ExplicitHelpActivation()
        {
            var options = new MockOptions();
            var writer = new StringWriter();

            bool result = base.Parser.ParseArguments(
                    new string[] { "--help" }, options, writer);

            base.AssertParserFailure(result);

            string helpText = writer.ToString();
            Assert.IsTrue(helpText.Length > 0);
        }
    }
}