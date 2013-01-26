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
using NUnit.Framework;
using FluentAssertions;
using CommandLine.Tests.Mocks;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class VerbsFixture : CommandLineParserBaseFixture
    {
        [Test]
        public void ParseVerbsCreateInstance()
        {
            var options = new OptionsWithVerbs();
            options.AddVerb.Should().BeNull();

            Result = Parser.ParseArguments(new string[] {"add", "-p", "untracked.bin"} , options);

            ResultShouldBeTrue();

            Parser.WasVerbOptionInvoked("add").Should().BeTrue();
            Parser.WasVerbOptionInvoked("commit").Should().BeFalse();
            Parser.WasVerbOptionInvoked("clone").Should().BeFalse();

            // Parser has built instance for us
            options.AddVerb.Should().NotBeNull();
            options.AddVerb.CreationProof.Should().NotHaveValue();
            options.AddVerb.Patch.Should().BeTrue();
            options.AddVerb.FileName[0].Should().Be("untracked.bin");
        }

        [Test]
        public void ParseVerbsUsingInstance()
        {
            var proof = new Random().Next(int.MaxValue);
            var options = new OptionsWithVerbs();
            options.CommitVerb.Should().NotBeNull();
            options.CommitVerb.CreationProof = proof;

            Result = Parser.ParseArguments(new string[] { "commit", "--amend" }, options);

            ResultShouldBeTrue();

            Parser.WasVerbOptionInvoked("add").Should().BeFalse();
            Parser.WasVerbOptionInvoked("commit").Should().BeTrue();
            Parser.WasVerbOptionInvoked("clone").Should().BeFalse();

            // Check if the instance is the one provider by us (not by the parser)
            options.CommitVerb.CreationProof.Should().Be(proof);
            options.CommitVerb.Amend.Should().BeTrue();
        }

        [Test]
        public void FailedParsingPrintsHelpIndex()
        {
            var options = new OptionsWithVerbs();
            var testWriter = new StringWriter();
            Result = Parser.ParseArguments(new string[] {}, options, testWriter);

            ResultShouldBeFalse();

            Parser.WasVerbOptionInvoked("add").Should().BeFalse();
            Parser.WasVerbOptionInvoked("commit").Should().BeFalse();
            Parser.WasVerbOptionInvoked("clone").Should().BeFalse();

            var helpText = testWriter.ToString();
            helpText.Should().Be("verbs help index");
        }

        [Test]
        public void FailedVerbParsingPrintsParticularHelpScreen()
        {
            var options = new OptionsWithVerbs();
            var testWriter = new StringWriter();
            Result = Parser.ParseArguments(new string[] {"clone", "--no_hardlinks"}, options, testWriter);

            ResultShouldBeFalse();

            Parser.WasVerbOptionInvoked("add").Should().BeFalse();
            Parser.WasVerbOptionInvoked("commit").Should().BeFalse();
            // The following returns true because also if the parser fail 'clone' was invoked.
            Parser.WasVerbOptionInvoked("clone").Should().BeTrue();

            var helpText = testWriter.ToString();
            helpText.Should().Be("help for: clone");
        }

        [Test]
        public void WasVerbOptionInvokedReturnsFalseWithEmptyArguments()
        {
            var options = new OptionsWithVerbs();
            Result = Parser.ParseArguments(new string[] {}, options);

            ResultShouldBeFalse();

            Parser.WasVerbOptionInvoked("add").Should().BeFalse();
            Parser.WasVerbOptionInvoked("commit").Should().BeFalse();
            Parser.WasVerbOptionInvoked("clone").Should().BeFalse();
        }

        [Test]
        public void WasVerbOptionInvokedReturnsFalseWithNullOrEmptyVerb()
        {
            var options = new OptionsWithVerbs();
            Result = Parser.ParseArguments(new string[] {"commit", "--amend"}, options);

            ResultShouldBeTrue();

            Parser.WasVerbOptionInvoked(null).Should().BeFalse();
            Parser.WasVerbOptionInvoked("").Should().BeFalse();
        }

        [Test]
        public void WasVerbOptionInvokedReturnsFalseWithOrdinaryOptions()
        {
            var options = new OptionsWithVerbs();
            Result = Parser.ParseArguments(new string[] {"commit", "--amend"}, options);

            ResultShouldBeTrue();

            Parser.WasVerbOptionInvoked("--commit").Should().BeFalse();
            Parser.WasVerbOptionInvoked("-commit").Should().BeFalse(); // <- pure fantasy
            Parser.WasVerbOptionInvoked("-c").Should().BeFalse();
            Parser.WasVerbOptionInvoked("---commit").Should().BeFalse(); // <- pure fantasy
            Parser.WasVerbOptionInvoked("--amend").Should().BeFalse();
            Parser.WasVerbOptionInvoked("-a").Should().BeFalse();
        }
    }
}