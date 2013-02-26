#region License
//
// Command Line Library: ValueListParsingFixture.cs
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
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class ValueListAttributeParsingFixture : ParserBaseFixture
    {
        [Fact]
        public void Value_list_attribute_isolates_non_option_values()
        {
            var options = new SimpleOptionsWithValueList();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(
                new string[] { "--switch", "file1.ext", "file2.ext", "file3.ext", "-s", "out.ext" }, options);

            result.Should().BeTrue();

            options.Items[0].Should().Be("file1.ext");
            options.Items[1].Should().Be("file2.ext");
            options.Items[2].Should().Be("file3.ext");
            options.StringValue.Should().Be("out.ext");
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_inside_bounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "file.a", "file.b", "file.c" }, options);

            result.Should().BeTrue();

            options.InputFilenames[0].Should().Be("file.a");
            options.InputFilenames[1].Should().Be("file.b");
            options.InputFilenames[2].Should().Be("file.c");
            options.OutputFile.Should().BeNull();
            options.Overwrite.Should().BeFalse();
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_outside_bounds()
        {
            var options = new OptionsWithValueListMaximumThree();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(
                    new string[] { "file.a", "file.b", "file.c", "file.d" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Value_list_with_max_elem_set_to_zero_succeeds()
        {
            var options = new OptionsWithValueListMaximumZero();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { }, options);

            result.Should().BeTrue();

            options.Junk.Should().HaveCount(n => n == 0);
            Console.WriteLine(options);
        }

        [Fact]
        public void Value_list_with_max_elem_set_to_zero_failes()
        {
            var options = new OptionsWithValueListMaximumZero();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "some", "value" }, options);

            result.Should().BeFalse();
        }
    }
}

