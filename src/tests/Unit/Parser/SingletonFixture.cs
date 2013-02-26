#region License
//
// Command Line Library: SingletonFixture.cs
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
using System.Globalization;
using System.IO;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class SingletonFixture
    {
        [Fact]
        public void Parse_string_integer_bool_options()
        {
            var options = new SimpleOptions();
            bool result = CommandLine.Parser.Default.ParseArguments(
                    new string[] { "-s", "another string", "-i100", "--switch" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("another string");
            options.IntegerValue.Should().Be(100);
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void Default_doesnt_support_mutually_exclusive_options()
        {
            var options = new OptionsWithMultipleSet();
            bool result = CommandLine.Parser.Default.ParseArguments(
                new string[] { "-r1", "-g2", "-b3", "-h4", "-s5", "-v6" }, options);

            result.Should().BeTrue(); // enabling MutuallyExclusive option it would fails
        }

        [Fact]
        public void Default_parsing_culture_is_invariant()
        {
            CommandLine.Parser.Default.Settings.ParsingCulture.Should().Be(CultureInfo.InvariantCulture);
        }
    }
}

