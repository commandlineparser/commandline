#region License
//
// Command Line Library: CommandLineParserSettingsFixture.cs
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
using System.IO;
using CommandLine.Tests.Mocks;
using NUnit.Framework;
using Should.Fluent;
#endregion

namespace CommandLine.Tests
{
    [TestFixture]
    public sealed class CommandLineParserSettingsFixture
    {
        [Test]
        public void SettingHelpWriterUsingConstructor()
        {
            var writer = new StringWriter();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(writer));
            var options = new SimpleOptionsWithHelpOption();
            
            bool success = parser.ParseArguments(new string[] {"--help"}, options);

            success.Should().Be.False();
            writer.ToString().Should().Equal("MockOptions::GetUsage()");
        }

        [Test]
        public void SettingHelpWriterUsingProperty()
        {
            var writer = new StringWriter();
            var settings = new CommandLineParserSettings();
            settings.HelpWriter = writer;
            ICommandLineParser parser = new CommandLineParser(settings);
            var options = new SimpleOptionsWithHelpOption();

            bool success = parser.ParseArguments(new string[] { "--help" }, options);

            success.Should().Be.False();
            writer.ToString().Should().Equal("MockOptions::GetUsage()");
        }

        [Test]
        public void SettingHelpWriterUsingArgument()
        {
            var writer = new StringWriter();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings());
            var options = new SimpleOptionsWithHelpOption();

            bool success = parser.ParseArguments(new string[] { "--help" }, options, writer);

            success.Should().Be.False();
            writer.ToString().Should().Equal("MockOptions::GetUsage()");
        }
    }
}