#region License
//
// Command Line Library: StrictFixture.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Tests.Fakes;
using CommandLine.Infrastructure;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class StrictFixture : ParserBaseFixture
    {
        [Fact]
        public void Parse_strict_bad_input_fails_and_exits()
        {
            var options = new SimpleOptions();
            var testWriter = new StringWriter();

            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArgumentsStrict(new string[] {"--bad", "--input"}, options,
                () => Console.WriteLine("fake fail"));

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Did we really produced all help?
            lines.Should().HaveCount(n => n == 8);
            // Verify just significant output
            lines[5].Trim().Should().Be("-s, --string");
            lines[6].Trim().Should().Be("-i");
            lines[7].Trim().Should().Be("--switch");
        }

        [Fact]
        public void Parse_strict_bad_input_fails_and_exits_when_get_usage_is_defined()
        {
            var options = new SimpleOptionsForStrict();
            var testWriter = new StringWriter();

            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArgumentsStrict(new string[] { "--bad", "--input" }, options,
                () => Console.WriteLine("fake fail"));

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Did we really called user help method?
            lines.Should().HaveCount(n => n == 1);
            // Verify just significant output
            lines[0].Trim().Should().Be("SimpleOptionsForStrict (user defined)");
        }

        [Fact]
        public void Parse_strict_bad_input_fails_and_exits_with_verbs()
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            var options = new OptionsWithVerbsNoHelp();
            var testWriter = new StringWriter();

            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArgumentsStrict(new string[] { "bad", "input" }, options,
                (verb, subOptions) =>
                {
                    invokedVerb = verb;
                    invokedVerbInstance = subOptions;
                },
                () => Console.WriteLine("fake fail"));

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Did we really produced all help?
            lines.Should().HaveCount(n => n == 8);
            // Verify just significant output
            lines[5].Trim().Should().Be("add       Add file contents to the index.");
            lines[6].Trim().Should().Be("commit    Record changes to the repository.");
            lines[7].Trim().Should().Be("clone     Clone a repository into a new directory.");

            invokedVerb.Should().Be("bad");
            invokedVerbInstance.Should().BeNull();
        }

        [Fact]
        public void Parse_strict_bad_input_fails_and_exits_with_verbs_when_get_usage_is_defined()
        {
            string invokedVerb = null;
            object invokedVerbInstance = null;

            var options = new OptionsWithVerbs();
            var testWriter = new StringWriter();

            ReflectionHelper.AssemblyFromWhichToPullInformation = Assembly.GetExecutingAssembly();
            var parser = new CommandLine.Parser(with => with.HelpWriter = testWriter);
            var result = parser.ParseArgumentsStrict(new string[] { "bad", "input" }, options,
                (verb, subOptions) =>
                {
                    invokedVerb = verb;
                    invokedVerbInstance = subOptions;
                },
                () => Console.WriteLine("fake fail"));

            result.Should().BeFalse();

            var helpText = testWriter.ToString();
            Console.WriteLine(helpText);
            var lines = helpText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Did we really produced all help?
            lines.Should().HaveCount(n => n == 1);
            // Verify just significant output
            lines[0].Trim().Should().Be("verbs help index");

            invokedVerb.Should().Be("bad");
            invokedVerbInstance.Should().BeNull();
        }

        //private static Parser SetTestDelegate(Parser parser)
        //{
        //    parser.SetOnExit(code => Console.WriteLine(
        //        "UNIT_TESTS symbol enabled.\nSimulating 'Environment.Exit({0})'.",
        //        code));
        //    return parser;
        //}
    }
}

