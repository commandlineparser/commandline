#region License
//
// Command Line Library: ParserSettingsFixture.cs
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
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    using System;
    using System.Globalization;

    public class ParserSettingsFixture
    {
        [Fact]
        public void Setting_help_writer_using_constructor()
        {
            var writer = new StringWriter();
            var parser = new CommandLine.Parser(new ParserSettings(writer));
            var options = new SimpleOptionsWithHelpOption();
            
            bool success = parser.ParseArguments(new string[] {"--help"}, options);

            success.Should().BeFalse();
            writer.ToString().Should().Be("MockOptions::GetUsage()");
        }

        [Fact]
        public void Setting_help_writer_using_property()
        {
            var writer = new StringWriter();
            var settings = new ParserSettings();
            settings.HelpWriter = writer;
            var parser = new CommandLine.Parser(settings);
            var options = new SimpleOptionsWithHelpOption();

            bool success = parser.ParseArguments(new string[] { "--help" }, options);

            success.Should().BeFalse();
            writer.ToString().Should().Be("MockOptions::GetUsage()");
        }

        [Fact]
        public void Setting_instance_is_not_reusable()
        {
            var settings = new ParserSettings(helpWriter: Console.Out);

            var parser = new CommandLine.Parser(settings);

            Assert.ThrowsDelegate act = () => { var parser2 = new CommandLine.Parser(settings); };

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void Setting_instance_became_immutable_after_being_consumed()
        {
            var settings = new ParserSettings { ParsingCulture = new CultureInfo("it-IT") };

            var parser = new CommandLine.Parser(settings);

            Assert.ThrowsDelegate act = () => { settings.HelpWriter = Console.Out; };

            Assert.Throws<InvalidOperationException>(act);
        }

        //[Fact]
        //public void Setting_help_writer_using_argument()
        //{
        //    var writer = new StringWriter();
        //    IParser parser = new CommandLine.Parser(new ParserSettings());
        //    var options = new SimpleOptionsWithHelpOption();

        //    bool success = parser.ParseArguments(new string[] { "--help" }, options, writer);

        //    success.Should().BeFalse();
        //    writer.ToString().Should().Be("MockOptions::GetUsage()");
        //}
    }
}

