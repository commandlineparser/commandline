#region License
//
// Command Line Library: VerbsFixture.cs
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
#if UNIT_TESTS_VERBS
#region Using Directives
using System;
using System.IO;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace CommandLine.Tests.Text
{
    [TestFixture]
    public class VerbsHelpTextFixture : CommandLineParserBaseFixture
    {
        [Test]
        public void FailedParsingPrintsHelpIndex()
        {
            DoCoreTestForIndex(new string[] {});
        }

        [Test]
        public void RequestingHelpPrintsHelpIndex()
        {
            DoCoreTestForIndex(new string[] {"help"});
        }

        [Test]
        public void RequestingBadHelpPrintsHelpIndex()
        {
            DoCoreTestForIndex(new string[] { "help", "undefined" });
        }

        [Test]
        public void FailedVerbParsingPrintsParticularHelpScreen()
        {
            var options = new OptionsWithVerbsHelp();
            var testWriter = new StringWriter();
            Result = Parser.ParseArguments(new string[] { "clone", "--no_hardlinks" }, options, testWriter);

            ResultShouldBeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Verify just significant output
            lines[5].Trim().Should().Equal("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[6].Trim().Should().Equal("filesystem by copying files.");
            lines[7].Trim().Should().Equal("-q, --quiet       Suppress summary message.");
        }

        private void DoCoreTestForIndex(string[] args)
        {
            var options = new OptionsWithVerbsHelp();
            var testWriter = new StringWriter();
            Result = Parser.ParseArguments(args, options, testWriter);

            ResultShouldBeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Verify just significant output
            lines[5].Trim().Should().Equal("add       Add file contents to the index.");
            lines[6].Trim().Should().Equal("commit    Record changes to the repository.");
            lines[7].Trim().Should().Equal("clone     Clone a repository into a new directory.");
        }
    }
}
#endif