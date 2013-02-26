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
#region Using Directives
using System;
using System.IO;
using System.Reflection;
using CommandLine.Tests;
using CommandLine.Tests.Fakes;
using CommandLine.Infrastructure;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Text
{
    public class VerbsHelpTextFixture : ParserBaseFixture
    {
        [Fact]
        public void Failed_parsing_prints_help_index()
        {
            DoCoreTestForIndex(new string[] {});
        }

        [Fact]
        public void Requesting_help_prints_help_index()
        {
            DoCoreTestForIndex(new string[] {"help"});
        }

        [Fact]
        public void Requesting_bad_help_prints_help_index()
        {
            DoCoreTestForIndex(new string[] { "help", "undefined" });
        }

        [Fact]
        public void Failed_verb_parsing_prints_particular_help_screen()
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            var options = new OptionsWithVerbsHelp();
            var testWriter = new StringWriter();
            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArguments(new string[] { "clone", "--no_hardlinks" }, options,
                (verb, subOptions) =>
                    {
                        invokedVerb = verb;
                        invokedVerbInstance = subOptions;
                    });

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Verify just significant output
            lines[5].Trim().Should().Be("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[6].Trim().Should().Be("filesystem by copying files.");
            lines[7].Trim().Should().Be("-q, --quiet       Suppress summary message.");

            invokedVerb.Should().Be("clone");
            invokedVerbInstance.Should().Be(null);
        }

        #region https://github.com/gsscoder/commandline/issues/45
        [Fact]
        public void Requesting_help_of_particular_verb_without_instance_should_work()
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            var options = new OptionsWithVerbsHelp();
            var testWriter = new StringWriter();
            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArguments(new string[] {"help", "add"}, options,
                (verb, subOptions) =>
                    {
                        invokedVerb = verb;
                        invokedVerbInstance = subOptions;
                    });

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            invokedVerb.Should().Be("help");
            invokedVerbInstance.Should().Be(null);
        }
        #endregion

        private void DoCoreTestForIndex(string[] args)
        {
            var options = new OptionsWithVerbsHelp();
            var testWriter = new StringWriter();
            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArguments(args, options,
                (_, __) =>
                    {
                    });

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Verify just significant output
            lines[5].Trim().Should().Be("add       Add file contents to the index.");
            lines[6].Trim().Should().Be("commit    Record changes to the repository.");
            lines[7].Trim().Should().Be("clone     Clone a repository into a new directory.");
        }
    }
}