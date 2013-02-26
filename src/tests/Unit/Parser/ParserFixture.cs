#region License
//
// Command Line Library: ParserFixture.cs
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
using System.Threading;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;
#endregion

namespace CommandLine.Tests.Unit.Parser
{
    public class ParserFixture : ParserBaseFixture
    {
        [Fact]
        public void Will_throw_exception_if_arguments_array_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLine.Parser().ParseArguments(null, new SimpleOptions()));
        }

        [Fact]
        public void Will_throw_exception_if_options_instance_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => new CommandLine.Parser().ParseArguments(new string[] {}, null));
        }

        [Fact]
        public void Parse_string_option()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-s", "something" }, options);
            
            result.Should().BeTrue();
            options.StringValue.Should().Be("something");
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_string_integer_bool_options()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(
                    new string[] { "-s", "another string", "-i100", "--switch" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("another string");
            options.IntegerValue.Should().Be(100);
            options.BooleanValue.Should().BeTrue();
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_short_adjacent_options()
        {
            var options = new BooleanSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-ca", "-d65" }, options);

            result.Should().BeTrue();
            options.BooleanThree.Should().BeTrue();
            options.BooleanOne.Should().BeTrue();
            options.BooleanTwo.Should().BeFalse();
            options.NonBooleanValue.Should().Be(65D);
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_short_long_options()
        {
            var options = new BooleanSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-b", "--double=9" }, options);

            result.Should().BeTrue();
            options.BooleanTwo.Should().BeTrue();
            options.BooleanOne.Should().BeFalse();
            options.BooleanThree.Should().BeFalse();
            options.NonBooleanValue.Should().Be(9D);
            Console.WriteLine(options);
        }
 
        [Fact]
        public void Parse_option_list()
        {
            var options = new SimpleOptionsWithOptionList();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {
                                "-k", "string1:stringTwo:stringIII", "-s", "test-file.txt" }, options);

            result.Should().BeTrue();
            options.SearchKeywords[0].Should().Be("string1");
            Console.WriteLine(options.SearchKeywords[0]);
            options.SearchKeywords[1].Should().Be("stringTwo");
            Console.WriteLine(options.SearchKeywords[1]);
            options.SearchKeywords[2].Should().Be("stringIII");
            Console.WriteLine(options.SearchKeywords[2]);
            options.StringValue.Should().Be("test-file.txt");
            Console.WriteLine(options.StringValue);
        }

        #region #BUG0000
        [Fact]
        public void Short_option_refuses_equal_token()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-i=10" }, options);
            result.Should().BeFalse();
            Console.WriteLine(options);
        }
        #endregion

        [Fact]
        public void Parse_enum_options()
        {
            var options = new SimpleOptionsWithEnum();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-s", "data.bin", "-a", "ReadWrite" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("data.bin");
            options.FileAccess.Should().Be(FileAccess.ReadWrite);
            Console.WriteLine(options);
        }

        [Fact]
        public void Parse_culture_specific_number()
        {
            //var actualCulture = Thread.CurrentThread.CurrentCulture;
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser(new ParserSettings { ParsingCulture = new CultureInfo("it-IT") });
            var result = parser.ParseArguments(new string[] { "-d", "10,986" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(10.986D);

            //Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Fact]
        public void Parse_culture_specific_nullable_number()
        {
            var actualCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("it-IT");
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--n-double", "12,32982" }, options);

            result.Should().BeTrue();
            options.NullableDoubleValue.Should().Be(12.32982D);

            Thread.CurrentThread.CurrentCulture = actualCulture;
        }

        [Fact]
        public void Parse_options_with_defaults()
        {
            var options = new SimpleOptionsWithDefaults();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] {}, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("str");
            options.IntegerValue.Should().Be(9);
            options.BooleanValue.Should().BeTrue();
        }

        [Fact]
        public void Parse_options_with_default_array()
        {
            var options = new SimpleOptionsWithDefaultArray();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new [] { "-y", "4", "5", "6" }, options);

            result.Should().BeTrue();
            options.StringArrayValue.Should().Equal(new [] { "a", "b", "c" });
            options.IntegerArrayValue.Should().Equal(new [] { 4, 5, 6 });
            options.DoubleArrayValue.Should().Equal(new [] { 1.1, 2.2, 3.3 });
        }

        [Fact]
        public void Parse_options_with_bad_defaults()
        {
            var options = new SimpleOptionsWithBadDefaults();
            Assert.Throws<ParserException>(
                () => new CommandLine.Parser().ParseArguments(new string[] {}, options));
        }

        #region #BUG0002
        [Fact]
        public void Parsing_non_existent_short_option_fails_without_throwing_an_exception()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-x" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Parsing_non_existent_long_option_fails_without_throwing_an_exception()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--extend" }, options);

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0000
        [Fact]
        public void Default_parsing_is_case_sensitive()
        {
            var parser = new CommandLine.Parser();
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-a", "alfa", "--beta-OPTION", "beta" }, options);

            result.Should().BeTrue();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }

        [Fact]
        public void Using_wrong_case_with_default_fails()
        {
            var parser = new CommandLine.Parser();
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Disabling_case_sensitive()
        {
            var parser = new CommandLine.Parser(new ParserSettings(false)); //Ref.: #DGN0001
            var options = new MixedCaseOptions();
            var result = parser.ParseArguments(new string[] { "-A", "alfa", "--Beta-Option", "beta" }, options);

            result.Should().BeTrue();
            options.AlfaValue.Should().Be("alfa");
            options.BetaValue.Should().Be("beta");
        }
        #endregion

        #region #BUG0003
        [Fact]
        public void Passing_no_value_to_a_string_type_long_option_fails()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--string" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_byte_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--byte" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_short_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--short" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_an_integer_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--int" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_long_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--long" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_float_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--float" }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_no_value_to_a_double_type_long_option_fails()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--double" }, options);

            result.Should().BeFalse();
        }
        #endregion

        #region #REQ0001
        [Fact]
        public void Allow_single_dash_as_option_input_value()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--string", "-" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("-");
        }

        [Fact]
        public void Allow_single_dash_as_non_option_value()
        {
            var options = new SimpleOptionsWithValueList();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-sparser.xml", "-", "--switch" }, options);

            result.Should().BeTrue();
            options.StringValue.Should().Be("parser.xml");
            options.BooleanValue.Should().BeTrue();
            options.Items.Count.Should().Be(1);
            options.Items[0].Should().Be("-");
        }
        #endregion

        #region #BUG0004
        [Fact]
        public void Parse_negative_integer_value()
        {
            var options = new SimpleOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-i", "-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle2()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-i-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle3()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--int", "-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }

        public void ParseNegativeIntegerValue_InputStyle4()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--int=-4096" }, options);

            result.Should().BeTrue();
            options.IntegerValue.Should().Be(-4096);
        }


        [Fact]
        public void Parse_negative_floating_point_value()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-d", "-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style2()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-d-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style3()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--double", "-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }

        [Fact]
        public void Parse_negative_floating_point_value_input_style4()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "--double=-4096.1024" }, options);

            result.Should().BeTrue();
            options.DoubleValue.Should().Be(-4096.1024D);
        }
        #endregion

        #region #BUG0005
        [Fact]
        public void Passing_short_value_to_byte_option_must_fail_gracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-b", short.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_integer_value_to_short_option_must_fail_gracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-s", int.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_long_value_to_integer_option_must_fail_gracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-i", long.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_float_value_to_long_option_must_fail_gracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-l", float.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }

        [Fact]
        public void Passing_double_value_to_float_option_must_fail_gracefully()
        {
            var options = new NumberSetOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new string[] { "-f", double.MaxValue.ToString(CultureInfo.InvariantCulture) }, options);

            result.Should().BeFalse();
        }
        #endregion

        #region ISSUE#15
        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/15
        /// </summary>
        [Fact]
        public void Parser_should_report_missing_value()
        {
            var options = new ComplexOptions();
            var parser = new CommandLine.Parser();
            var result = parser.ParseArguments(new[] { "-i", "-o" }, options);

            result.Should().BeFalse();

            options.LastParserState.Errors.Count.Should().BeGreaterThan(0);
        }
        #endregion
    }
}

