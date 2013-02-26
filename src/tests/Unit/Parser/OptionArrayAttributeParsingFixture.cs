#region License
//
// Command Line Library: OptionArrayAttributeParsingFixture.cs
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
using System.Threading;
using System.Globalization;
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
    public class OptionArrayAttributeParsingFixture : ParserBaseFixture
    {
        [Fact]
        public void Parse_string_array_option_using_short_name()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-z", "alfa", "beta", "gamma" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_string_array_option_using_long_name()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--strarr", "alfa", "beta", "gamma" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "alfa", "beta", "gamma" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_with_value_adjacent()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-zapple", "kiwi" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "apple", "kiwi" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_string_array_option_using_long_name_with_equal_sign()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--strarr=apple", "kiwi" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "apple", "kiwi" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_and_string_option_after()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-z", "one", "two", "three", "-s", "after" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
            options.StringValue.Should().Be("after");
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_and_string_option_before()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-s", "before", "-z", "one", "two", "three" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("before");
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_string_array_option_using_short_name_with_options_before_and_after()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {
                "-i", "191919", "-z", "one", "two", "three", "--switch", "--string=near" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(191919);
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three" }, options.StringArrayValue);
            options.BooleanValue.Should().BeTrue();
            options.StringValue.Should().Be("near");
        }

        [Fact]
        public void Parse_string_array_option_using_long_name_with_value_list()
        {
            var options = new SimpleOptionsWithArrayAndValueList();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {
                "-shere", "-i999", "--strarr=0", "1", "2", "3", "4", "5", "6", "7", "8", "9" , "--switch", "f1.xml", "f2.xml"}, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("here");
            options.IntegerValue.Should().Be(999);
            base.ElementsShouldBeEqual(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, options.StringArrayValue);
            options.BooleanValue.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "f1.xml", "f2.xml" }, options.Items);
        }

        [Fact]
        public void Passing_no_value_to_a_string_array_option_fails()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-z" }, options);

            result.Should().BeFalse();

            options = new SimpleOptionsWithArray();
            result = parser.ParseArguments(new string[] { "--strarr" }, options);

            result.Should().BeFalse();
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_integer_array_option_using_short_name()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-y", "1", "2", "3" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3 }, options.IntegerArrayValue);
        }

        [Fact]
        public void Passing_bad_value_to_an_integer_array_option_fails()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-y", "one", "2", "3" }, options);

            result.Should().BeFalse();

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] { "-yone", "2", "3" }, options);

            result.Should().BeFalse();

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] { "--intarr", "1", "two", "3" }, options);

            result.Should().BeFalse();

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] { "--intarr=1", "2", "three" }, options);

            result.Should().BeFalse();
        }


        [Fact]
        public void Passing_no_value_to_an_integer_array_option_fails()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-y" }, options);

            result.Should().BeFalse();

            options = new SimpleOptionsWithArray();
            result = parser.ParseArguments(new string[] { "--intarr" }, options);

            result.Should().BeFalse();
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_double_array_option_using_short_name()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-q", "0.1", "2.3", "0.9" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new double[] { .1, 2.3, .9 }, options.DoubleArrayValue);
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_different_arrays_together__combination_one()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] {
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four"
            }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
        }

        [Fact]
        public void Parse_different_arrays_together__combination_two()
        {
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string=after"
            }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringValue.Should().Be("after");

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] {
                "--string", "before",
                "-y", "1", "2", "3", "4",
                "-z", "one", "two", "three", "four",
                "-q", "0.1", "0.2", "0.3", "0.4"
            }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("before");
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] {
                "-q", "0.1", "0.2", "0.3", "0.4",
                "-y", "1", "2", "3", "4",
                "-s", "near-the-center",
                "-z", "one", "two", "three", "four"
            }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            options.StringValue.Should().Be("near-the-center");
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);

            options = new SimpleOptionsWithArray();
            parser = new CommandLine.Parser();
            result = parser.ParseArguments(new string[] {
                "--switch",
                "-z", "one", "two", "three", "four",
                "-y", "1", "2", "3", "4",
                "-i", "1234",
                "-q", "0.1", "0.2", "0.3", "0.4",
                "--string", "I'm really playing with the parser!"
            }, options);

            result.Should().BeTrue();
            options.BooleanValue.Should().BeTrue();
            base.ElementsShouldBeEqual(new string[] { "one", "two", "three", "four" }, options.StringArrayValue);
            base.ElementsShouldBeEqual(new int[] { 1, 2, 3, 4 }, options.IntegerArrayValue);
            options.IntegerValue.Should().Be(1234);
            base.ElementsShouldBeEqual(new double[] { .1, .2, .3, .4 }, options.DoubleArrayValue);
            options.StringValue.Should().Be("I'm really playing with the parser!");
        }

        /****************************************************************************************************/

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_string_with_short_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments(new string[] { "-v", "a", "b", "c" }, new SimpleOptionsWithBadOptionArray()));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_string_with_long_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments(new string[] { "--bstrarr", "a", "b", "c" }, new SimpleOptionsWithBadOptionArray()));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_integer_with_short_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments(new string[] { "-w", "1", "2", "3" }, new SimpleOptionsWithBadOptionArray()));
        }

        [Fact]
        public void Will_throw_exception_if_option_array_attribute_bound_to_integer_with_long_name()
        {
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments(new string[] { "--bintarr", "1", "2", "3" }, new SimpleOptionsWithBadOptionArray()));
        }

        [Fact]
        public void Parse_culture_specific_number()
        {
            //var actualCulture = Thread.CurrentThread.CurrentCulture;
            ////Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new SimpleOptionsWithArray();
            var parser = new CommandLine.Parser(new ParserSettings { ParsingCulture = new CultureInfo("it-IT") });
            var result = parser.ParseArguments(new string[] { "-q", "1,2", "1,23", "1,234" }, options);

            result.Should().BeTrue();
            base.ElementsShouldBeEqual(new double[] { 1.2, 1.23, 1.234 }, options.DoubleArrayValue);

            //Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        /****************************************************************************************************/

        [Fact]
        public void Parse_two_uint_consecutive_array()
        {
            var options = new OptionsWithUIntArray();
            var parser = new CommandLine.Parser();
            var result = CommandLine.Parser.Default.ParseArguments(new string[] {
                "--somestr", "just a string",
                "--arrayone", "10", "20", "30", "40",
                "--arraytwo", "11", "22", "33", "44",
                "--somebool"
            }, options);

            result.Should().BeTrue();
            options.SomeStringValue.Should().Be("just a string");
            base.ElementsShouldBeEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            base.ElementsShouldBeEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            options.SomeBooleanValue.Should().BeTrue();
        }

        [Fact]
        public void Parse_two_uint_consecutive_array_using_short_names()
        {
            var options = new OptionsWithUIntArray();
            var result = CommandLine.Parser.Default.ParseArguments(new string[] {
                "-s", "just a string",
                "-o", "10", "20", "30", "40",
                "-t", "11", "22", "33", "44",
                "-b"
            }, options);

            result.Should().BeTrue();
            options.SomeStringValue.Should().Be("just a string");
            base.ElementsShouldBeEqual(new uint[] {10, 20, 30, 40}, options.ArrayOne);
            base.ElementsShouldBeEqual(new uint[] {11, 22, 33, 44}, options.ArrayTwo);
            options.SomeBooleanValue.Should().BeTrue();
        }
    }
}

