// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using Microsoft.FSharp.Core;
using CommandLine.Core;
using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;

using CSharpx;

using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class InstanceBuilderTests
    {
        private static ParserResult<T> InvokeBuild<T>(string[] arguments, bool autoHelp = true, bool autoVersion = true, bool multiInstance = false)
            where T : new()
        {
            return InstanceBuilder.Build(
                Maybe.Just<Func<T>>(() => new T()),
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                arguments,
                StringComparer.Ordinal,
                false,
                CultureInfo.InvariantCulture,
                autoHelp,
                autoVersion,
                multiInstance,
                Enumerable.Empty<ErrorType>());
        }

        private static ParserResult<T> InvokeBuildEnumValuesCaseIgnore<T>(string[] arguments)
            where T : new()
        {
            return InstanceBuilder.Build(
                Maybe.Just<Func<T>>(() => new T()),
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                arguments,
                StringComparer.Ordinal,
                true,
                CultureInfo.InvariantCulture,
                true,
                true,
                Enumerable.Empty<ErrorType>());
        }

        private static ParserResult<T> InvokeBuildImmutable<T>(string[] arguments)
        {
            return InstanceBuilder.Build(
                Maybe.Nothing<Func<T>>(),
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                arguments,
                StringComparer.Ordinal,
                false,
                CultureInfo.InvariantCulture,
                true,
                true,
                Enumerable.Empty<ErrorType>());
        }

        [Fact]
        public void Explicit_help_request_generates_help_requested_error()
        {
            // Fixture setup
            var expectedResult = new NotParsed<Simple_Options>(
                TypeInfo.Create(typeof(Simple_Options)), new Error[] { new HelpRequestedError() });

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "--help" });

            // Verify outcome
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new[] { "-123" }, -123L)]
        [InlineData(new[] { "-1" }, -1L)]
        [InlineData(new[] { "-9223372036854775807" }, -9223372036854775807)] // long.MaxValue * -1
        public void Parse_negative_long_value(string[] arguments, long expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.LongValue.Should().Be(expected);
        }

        [Theory]
        [InlineData(new[] { "0.123" }, .123D)]
        [InlineData(new[] { "-0.123" }, -0.123D)]
        [InlineData(new[] { "1.0123456789" }, 1.0123456789D)]
        [InlineData(new[] { "-1.0123456789" }, -1.0123456789D)]
        [InlineData(new[] { "0" }, 0D)]
        public void Parse_double_value(string[] arguments, double expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Double_Value>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options_With_Double_Value>)result).Value.DoubleValue.Should().Be(expected);
        }

        [Theory]
        [InlineData(new[] { "--int-seq", "1", "20", "300", "4000" }, new[] { 1, 20, 300, 4000 })]
        [InlineData(new[] { "--int-seq=1", "20", "300", "4000" }, new[] { 1, 20, 300, 4000 })]
        [InlineData(new[] { "--int-seq", "2147483647" }, new[] { 2147483647 })]
        [InlineData(new[] { "--int-seq=2147483647" }, new[] { 2147483647 })]
        [InlineData(new[] { "--int-seq", "-1", "20", "-3", "0" }, new[] { -1, 20, -3, 0 })]
        [InlineData(new[] { "--int-seq=-1", "20", "-3", "0" }, new[] { -1, 20, -3, 0 })]
        public void Parse_int_sequence(string[] arguments, int[] expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence>(
                arguments);

            // Verify outcome
            ((Parsed<Options_With_Sequence>)result).Value.IntSequence.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(new[] { "-i", "10", "20", "30" }, new[] { 10, 20, 30 })]
        [InlineData(new[] { "-i", "10", "20", "30", "40" }, new[] { 10, 20, 30, 40 })]
        [InlineData(new[] { "-i10", "20", "30" }, new[] { 10, 20, 30 })]
        [InlineData(new[] { "-i10", "20", "30", "40" }, new[] { 10, 20, 30, 40 })]
        public void Parse_int_sequence_with_range(string[] arguments, int[] expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.IntSequence.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(new[] { "-s", "just-one" }, new[] { "just-one" })]
        [InlineData(new[] { "-sjust-one-samearg" }, new[] { "just-one-samearg" })]
        [InlineData(new[] { "-s", "also-two", "are-ok" }, new[] { "also-two", "are-ok" })]
        [InlineData(new[] { "--string-seq", "one", "two", "three" }, new[] { "one", "two", "three" })]
        [InlineData(new[] { "--string-seq=one", "two", "three", "4" }, new[] { "one", "two", "three", "4" })]
        public void Parse_string_sequence_with_only_min_constraint(string[] arguments, string[] expected)
        {
            // Fixture setup with attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint>(
                arguments);

            // Verify outcome
            ((Parsed<Options_With_Sequence_And_Only_Min_Constraint>)result).Value.StringSequence.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(new[] { "-s", "just-one" }, new[] { "just-one" })]
        [InlineData(new[] { "-sjust-one-samearg" }, new[] { "just-one-samearg" })]
        [InlineData(new[] { "-s", "also-two", "are-ok" }, new[] { "also-two", "are-ok" })]
        [InlineData(new[] { "--string-seq", "one", "two", "three" }, new[] { "one", "two", "three" })]
        public void Parse_string_sequence_with_only_max_constraint(string[] arguments, string[] expected)
        {
            // Fixture setup with attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Max_Constraint>(
                arguments);

            // Verify outcome
            ((Parsed<Options_With_Sequence_And_Only_Max_Constraint>)result).Value.StringSequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_generates_MissingValueOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingValueOptionError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint>(
                new[] { "-s" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Min_Constraint>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_as_value_generates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint_For_Value>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Min_Constraint_For_Value>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_does_not_generate_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { "one", "two", "three" };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Max_Constraint>(
                new[] { "--string-seq=one", "two", "three", "this-is-too-much" });

            // Verify outcome
            ((Parsed<Options_With_Sequence_And_Only_Max_Constraint>)result).Value.StringSequence.Should().BeEquivalentTo(expectedResult);
            // The "this-is-too-much" arg would end up assigned to a Value; since there is no Value, it is silently dropped
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_as_value_generates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Max_Constraint_For_Value>(
                new[] { "one", "two", "three", "this-is-too-much" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Max_Constraint_For_Value>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new[] { "--colors", "Red" }, Colors.Red)]
        [InlineData(new[] { "--colors", "Green" }, Colors.Green)]
        [InlineData(new[] { "--colors", "Blue" }, Colors.Blue)]
        [InlineData(new[] { "--colors", "0" }, Colors.Red)]
        [InlineData(new[] { "--colors", "1" }, Colors.Green)]
        [InlineData(new[] { "--colors", "2" }, Colors.Blue)]
        public void Parse_enum_value(string[] arguments, Colors expected)
        {
            // Fixture setup in attribute

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Enum>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Simple_Options_With_Enum>)result).Value.Colors);
        }

        [Theory]
        [InlineData(new[] { "--colors", "red" }, Colors.Red)]
        [InlineData(new[] { "--colors", "green" }, Colors.Green)]
        [InlineData(new[] { "--colors", "blue" }, Colors.Blue)]
        [InlineData(new[] { "--colors", "0" }, Colors.Red)]
        [InlineData(new[] { "--colors", "1" }, Colors.Green)]
        [InlineData(new[] { "--colors", "2" }, Colors.Blue)]
        public void Parse_enum_value_ignore_case(string[] arguments, Colors expected)
        {
            // Fixture setup in attribute

            // Exercize system 
            var result = InvokeBuildEnumValuesCaseIgnore<Simple_Options_With_Enum>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Simple_Options_With_Enum>)result).Value.Colors);
        }

        [Fact]
        public void Parse_enum_value_with_wrong_index_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Enum>(
                new[] { "--colors", "3" });

            // Verify outcome
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Enum>(
                new[] { "--colors", "Yellow" });

            // Verify outcome
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_case_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Enum>(
                new[] { "--colors", "RED" });

            // Verify outcome
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_values_partitioned_between_sequence_and_scalar()
        {
            // Fixture setup
            var expectedResult = new Simple_Options_With_Values
            {
                StringValue = string.Empty,
                LongValue = 10L,
                StringSequence = new[] { "a", "b", "c" },
                IntValue = 20
            };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Values>(
                new[] { "10", "a", "b", "c", "20" });

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Simple_Options_With_Values>)result).Value);
        }

        [Theory]
        [InlineData(new[] { "987654321" }, new[] { 987654321L })]
        [InlineData(new[] { "1", "2", "3", "4", "5", "6" }, new[] { 1L, 2L, 3L, 4L, 5L, 6L })]
        [InlineData(new string[] { }, new long[] { })]
        [InlineData(new[] { "-1", "2", "9876543210", "-4", "0" }, new[] { -1L, 2L, 9876543210L, -4L, 0L })]
        [InlineData(new[] { "0", "200000", "300000", "400000", "-500000", "600000", "700000", "800000", "900000", "-99999999" }, new[] { 0L, 200000L, 300000L, 400000L, -500000L, 600000L, 700000L, 800000L, 900000L, -99999999L })]
        public void Parse_sequence_value_without_range_constraints(string[] arguments, long[] expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_Without_Range_For_Value>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Sequence_Without_Range_For_Value>)result).Value.LongSequence);
        }

        [Theory]
        [InlineData(new[] { "--long-seq", "1;1234;59678" }, new[] { 1L, 1234L, 59678L })]
        [InlineData(new[] { "--long-seq=1;1234;59678" }, new[] { 1L, 1234L, 59678L })]
        [InlineData(new[] { "--long-seq", "-978;1234;59678;0" }, new[] { -978L, 1234L, 59678L, 0L })]
        [InlineData(new[] { "--long-seq=-978;1234;59678;0" }, new[] { -978L, 1234L, 59678L, 0L })]
        public void Parse_long_sequence_with_separator(string[] arguments, long[] expected)
        {
            // Fixture setup in attributes

            // Exercize system
            var result = InvokeBuild<Options_With_Sequence_Having_Separator_Set>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Sequence_Having_Separator_Set>)result).Value.LongSequence);
        }

        [Theory]
        [InlineData(new[] { "-s", "here-one-elem-but-no-sep" }, new[] { "here-one-elem-but-no-sep" })]
        [InlineData(new[] { "-shere-one-elem-but-no-sep" }, new[] { "here-one-elem-but-no-sep" })]
        [InlineData(new[] { "-s", "eml1@xyz.com,test@unit.org,xyz@srv.it" }, new[] { "eml1@xyz.com", "test@unit.org", "xyz@srv.it" })]
        [InlineData(new[] { "-sInlineData@iscool.org,test@unit.org,xyz@srv.it,another,the-last-one" }, new[] { "InlineData@iscool.org", "test@unit.org", "xyz@srv.it", "another", "the-last-one" })]
        public void Parse_string_sequence_with_separator(string[] arguments, string[] expected)
        {
            // Fixture setup in attributes

            // Exercize system
            var result = InvokeBuild<Options_With_Sequence_Having_Separator_Set>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Sequence_Having_Separator_Set>)result).Value.StringSequence);
        }

        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/31
        /// </summary>
        [Fact]
        public void Double_dash_force_subsequent_arguments_as_values()
        {
            // Fixture setup
            var expectedResult = new Simple_Options_With_Values
            {
                StringValue = "str1",
                LongValue = 10L,
                StringSequence = new[] { "-a", "--bee", "-c" },
                IntValue = 20
            };
            var arguments = new[] { "--stringvalue", "str1", "--", "10", "-a", "--bee", "-c", "20" };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<Simple_Options_With_Values>>(() => new Simple_Options_With_Values()),
                (a, optionSpecs) =>
                    Tokenizer.PreprocessDashDash(a,
                        args => Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, StringComparer.Ordinal))),
                arguments,
                StringComparer.Ordinal,
                false,
                CultureInfo.InvariantCulture,
                true,
                true,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Simple_Options_With_Values>)result).Value);
        }

        [Fact]
        public void Parse_option_from_different_sets_generates_MutuallyExclusiveSetError()
        {
            // Fixture setup
            var expectedResult = new[]
                {
                    new MutuallyExclusiveSetError(new NameInfo("", "weburl"), string.Empty),
                    new MutuallyExclusiveSetError(new NameInfo("", "ftpurl"), string.Empty)
                };

            // Exercize system 
            var result = InvokeBuild<Options_With_Two_Sets>(
                new[] { "--weburl", "http://mywebsite.org/", "--ftpurl", "fpt://ftpsite.org/" });

            // Verify outcome
            ((NotParsed<Options_With_Two_Sets>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_both_are_true()
        {
            // Fixture setup
            var expectedResult = new Options_With_Required_Set_To_True_Within_Same_Set
            {
                FtpUrl = "str1",
                WebUrl = "str2"
            };
            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_Within_Same_Set>(
                new[] { "--ftpurl", "str1", "--weburl", "str2" });

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Options_With_Required_Set_To_True_Within_Same_Set>)result).Value);
            // Teardown
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_none_are_true()
        {
            // Fixture setup
            var expectedResult = new[]
            {
                new MissingRequiredOptionError(new NameInfo("", "ftpurl")),
                new MissingRequiredOptionError(new NameInfo("", "weburl"))
            };
            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_Within_Same_Set>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Required_Set_To_True_Within_Same_Set>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Omitting_required_option_generates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(new NameInfo("", "str")) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Required_Set_To_True>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Wrong_range_in_sequence_generates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("i", "")) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-i", "10" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_unknown_long_option_generates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("xyz") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "--stringvalue", "abc", "--xyz" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_unknown_short_option_generates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-z", "-x" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_unknown_short_option_in_option_group_generates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-zx" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new[] { "--stringvalue", "this-value" }, "this-value")]
        [InlineData(new[] { "--stringvalue=this-other" }, "this-other")]
        public void Omitting_names_assumes_identifier_as_long_name(string[] arguments, string expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.StringValue.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Breaking_required_constraint_in_string_scalar_as_value_generates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_For_Values>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Required_Set_To_True_For_Values>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new[] { "--stringvalue", "中文" }, "中文")] // Chinese
        [InlineData(new[] { "--stringvalue=中文" }, "中文")]
        [InlineData(new[] { "--stringvalue", "日本人" }, "日本人")] // Japanese
        [InlineData(new[] { "--stringvalue=日本人" }, "日本人")]
        public void Parse_utf8_string_correctly(string[] arguments, string expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Simple_Options>)result).Value.StringValue);
        }

        [Fact]
        public void Breaking_equal_min_max_constraint_in_string_sequence_as_value_generates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_Having_Both_Min_And_Max_Equal>(
                new[] { "one", "two", "this-is-too-much" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_Having_Both_Min_And_Max_Equal>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData(new[] { "-i", "10" }, 10)]
        [InlineData(new string[] { }, null)]
        [InlineData(new[] { "-i9999" }, 9999)]
        [InlineData(new[] { "--nullable-int=-1" }, -1)]
        public void Parse_nullable_int(string[] arguments, int? expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Nullables>(
                arguments);

            // Verify outcome
            expected.Should().Be(((Parsed<Options_With_Nullables>)result).Value.NullableInt);
        }

        [Theory]
        [InlineData(new[] { "10" }, 10L)]
        [InlineData(new string[] { }, null)]
        [InlineData(new[] { "9999" }, 9999L)]
        [InlineData(new[] { "-1" }, -1L)]
        public void Parse_nullable_long(string[] arguments, long? expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Nullables>(
                arguments);

            // Verify outcome
            expected.Should().Be(((Parsed<Options_With_Nullables>)result).Value.NullableLong);
        }

#if !SKIP_FSHARP
        [Theory]
        [InlineData(new[] { "--filename", "log-20150626.txt" }, "log-20150626.txt", true)]
        [InlineData(new string[] { }, null, false)]
        public void Parse_fsharp_option_string(string[] arguments, string expectedValue, bool expectedSome)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_FSharpOption>(
                arguments);

            // Verify outcome
            if (((Parsed<Options_With_FSharpOption>)result).Value.FileName != null)
            {
                expectedValue.Should().BeEquivalentTo(((Parsed<Options_With_FSharpOption>)result).Value.FileName.Value);
            }
            expectedSome.Should().Be(FSharpOption<string>.get_IsSome(((Parsed<Options_With_FSharpOption>)result).Value.FileName));
        }

        [Theory]
        [InlineData(new[] { "1234567" }, 1234567, true)]
        [InlineData(new string[] { }, default(int), false)]
        public void Parse_fsharp_option_int(string[] arguments, int expectedValue, bool expectedSome)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_FSharpOption>(
                arguments);

            // Verify outcome
            if (((Parsed<Options_With_FSharpOption>)result).Value.Offset != null)
            {
                expectedValue.Should().Be(((Parsed<Options_With_FSharpOption>)result).Value.Offset.Value);
            }
            expectedSome.Should().Be(FSharpOption<int>.get_IsSome(((Parsed<Options_With_FSharpOption>)result).Value.Offset));
        }
#endif


        [Fact]
        public void Min_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Min_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<InvalidOperationException>(test);
        }

        [Fact]
        public void Max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Max_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<InvalidOperationException>(test);
        }

        [Fact]
        public void Min_and_max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Both_Min_And_Max_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<InvalidOperationException>(test);
        }

        [Theory]
        [InlineData(new[] { "--weburl", "value.com", "--verbose" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--ftpurl", "value.org", "--interactive" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--weburl", "value.com", "--verbose", "--interactive" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--ftpurl=fvalue", "--weburl=wvalue" }, ParserResultType.NotParsed, 2)]
        [InlineData(new[] { "--interactive", "--weburl=wvalue", "--verbose", "--ftpurl=wvalue" }, ParserResultType.NotParsed, 2)]
        public void Empty_set_options_allowed_with_mutually_exclusive_sets(string[] arguments, ParserResultType type, int expected)
        {
            // Exercize system
            var result = InvokeBuild<Options_With_Named_And_Empty_Sets>(
                arguments);

            // Verify outcome
            if (type == ParserResultType.NotParsed)
            {
                ((NotParsed<Options_With_Named_And_Empty_Sets>)result).Errors.Should().HaveCount(x => x == expected);
            }
            else if (type == ParserResultType.Parsed)
            {
                result.Should().BeOfType<Parsed<Options_With_Named_And_Empty_Sets>>();
            }
        }

        [Theory]
        [InlineData(new[] { "--stringvalue", "abc", "--stringvalue", "def" }, 1)]
        public void Specifying_options_two_or_more_times_generates_RepeatedOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == expected);
        }

        [Theory]
        [InlineData(new[] { "-s", "abc", "-s", "def" }, 1)]
        public void Specifying_options_two_or_more_times_with_short_options_generates_RepeatedOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == expected);
        }

        [Theory]
        [InlineData(new[] { "--shortandlong", "abc", "--shortandlong", "def" }, 1)]
        public void Specifying_options_two_or_more_times_with_long_options_generates_RepeatedOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == expected);
        }

        [Theory]
        [InlineData(new[] { "-s", "abc", "--shortandlong", "def" }, 1)]
        public void Specifying_options_two_or_more_times_with_mixed_short_long_options_generates_RepeatedOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == expected);
        }

        [Theory]
        [InlineData(new[] { "--inputfile=file1.bin" }, "file1.bin")]
        [InlineData(new[] { "--inputfile", "file2.txt" }, "file2.txt")]
        public void Can_define_options_on_explicit_interface_properties(string[] arguments, string expected) 
            {
            // Exercize system
            var result = InvokeBuild<Options_With_Only_Explicit_Interface>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((IInterface_With_Two_Scalar_Options)((Parsed<Options_With_Only_Explicit_Interface>)result).Value).InputFile);
        }


        [Theory]
        [InlineData(new[] { "--inputfile=file1.bin" }, "file1.bin")]
        [InlineData(new[] { "--inputfile", "file2.txt" }, "file2.txt")]
        public void Can_define_options_on_interface_properties(string[] arguments, string expected)
        {
            // Exercize system
            var result = InvokeBuild<Options_With_Interface>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Interface>)result).Value.InputFile);
        }

        [Theory]
        [InlineData(new[] { "--weburl=value.com" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--ftpurl=value.org" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--weburl=value.com", "-a" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--ftpurl=value.org", "-a" }, ParserResultType.Parsed, 0)]
        [InlineData(new[] { "--weburl=value.com", "--ftpurl=value.org" }, ParserResultType.NotParsed, 2)]
        [InlineData(new[] { "--weburl=value.com", "--ftpurl=value.org", "-a" }, ParserResultType.NotParsed, 2)]
        [InlineData(new string[] { }, ParserResultType.NotParsed, 2)]
        public void Enforce_required_within_mutually_exclusive_set_only(string[] arguments, ParserResultType type, int expected)
        {
            // Exercize system
            var result = InvokeBuild<Options_With_Two_Option_Required_Set_To_True_And_Two_Sets>(
                arguments);

            // Verify outcome
            if (type == ParserResultType.NotParsed)
            {
                ((NotParsed<Options_With_Two_Option_Required_Set_To_True_And_Two_Sets>)result).Errors.Should().HaveCount(x => x == expected);
            }
            else if (type == ParserResultType.Parsed)
            {
                result.Should().BeOfType<Parsed<Options_With_Two_Option_Required_Set_To_True_And_Two_Sets>>();
            }
        }

        [Theory]
        [MemberData(nameof(RequiredValueStringData))]
        public void Parse_string_scalar_with_required_constraint_as_value(string[] arguments, Options_With_Required_Set_To_True_For_Values expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_For_Values>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Required_Set_To_True_For_Values>)result).Value);
        }

        [Theory]
        [MemberData(nameof(ScalarSequenceStringAdjacentData))]
        public void Parse_string_scalar_and_sequence_adjacent(string[] arguments, Options_With_Scalar_Value_And_Adjacent_SequenceString expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Scalar_Value_And_Adjacent_SequenceString>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Scalar_Value_And_Adjacent_SequenceString>)result).Value);
        }

        [Fact]
        public void Parse_to_mutable()
        {
            // Fixture setup
            var expectedResult = new Simple_Options { StringValue = "strval0", IntSequence = new[] { 9, 7, 8 }, BoolValue = true, LongValue = 9876543210L };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" });

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Simple_Options>)result).Value);
        }

        [Theory]
        [InlineData(new string[] { }, 2)]
        [InlineData(new[] { "--str=val0" }, 1)]
        [InlineData(new[] { "--long=9" }, 1)]
        [InlineData(new[] { "--int=7" }, 2)]
        [InlineData(new[] { "--str", "val1", "--int=3" }, 1)]
        [InlineData(new[] { "--long", "9", "--int=11" }, 1)]
        public void Breaking_required_constraint_generate_MissingRequiredOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InvokeBuild<Options_With_Two_Options_Having_Required_Set_To_True>(
                arguments);

            // Verify outcome
            var errors = ((NotParsed<Options_With_Two_Options_Having_Required_Set_To_True>)result).Errors;
            errors.OfType<MissingRequiredOptionError>().Should().HaveCount(x => x == expected);
        }

        [Theory]
        [MemberData(nameof(ImmutableInstanceData))]
        public void Parse_to_immutable_instance(string[] arguments, Immutable_Simple_Options expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuildImmutable<Immutable_Simple_Options>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Immutable_Simple_Options>)result).Value);
        }

        [Theory]
        [MemberData(nameof(ImmutableInstanceDataArgs))]
        [Trait("Category", "Immutable")]
        public void Parse_to_immutable_instance_with_Invalid_Ctor_Args(string[] arguments)
        {
            // Fixture setup in attributes

            // Exercize system 
            Action act = () => InvokeBuildImmutable<Immutable_Simple_Options_Invalid_Ctor_Args>(
                arguments);

            // Verify outcome
            var expectedMsg =
                "Type CommandLine.Tests.Fakes.Immutable_Simple_Options_Invalid_Ctor_Args appears to be Immutable with invalid constructor. Check that constructor arguments have the same name and order of their underlying Type.  Constructor Parameters can be ordered as: '(stringvalue, intsequence, boolvalue, longvalue)'";
            act.Should().Throw<InvalidOperationException>().WithMessage(expectedMsg);
        }

        [Fact]
        public void Parse_to_type_with_single_string_ctor_builds_up_correct_instance()
        {
            // Fixture setup
            var expectedResult = new Options_With_Uri_And_SimpleType { EndPoint = new Uri("http://localhost/test/"), MyValue = new MySimpleType("custom-value") };

            // Exercize system 
            var result = InvokeBuild<Options_With_Uri_And_SimpleType>(
                new[] { "--endpoint=http://localhost/test/", "custom-value" });

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Options_With_Uri_And_SimpleType>)result).Value);
        }

        [Fact]
        public void Parse_option_with_exception_thrown_from_setter_generates_SetValueExceptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new SetValueExceptionError(new NameInfo("e", ""), new ArgumentException(), "bad") };

            // Exercize system 
            var result = InvokeBuild<Options_With_Property_Throwing_Exception>(
                new[] { "-e", "bad" });

            // Verify outcome
            ((NotParsed<Options_With_Property_Throwing_Exception>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_default_bool_type_string_SetValueExceptionError()
        {
            // Fixture setup
            string name = nameof(Options_With_InvalidDefaults.FileName).ToLower();
            var expectedResult = new[] { new SetValueExceptionError(new NameInfo("", name),
                new ArgumentException(InvalidAttributeConfigurationError.ErrorMessage), "bad") };

            // Exercize system 
            var result = InvokeBuild<Options_With_InvalidDefaults>(
                new[] { name, "bad" });

            // Verify outcome
            ((NotParsed<Options_With_InvalidDefaults>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }


        [Theory]
        [InlineData(new[] { "--stringvalue", "x-" }, "x-")]
        [InlineData(new[] { "--stringvalue", "x--" }, "x--")]
        [InlineData(new[] { "--stringvalue", "x---" }, "x---")]
        [InlineData(new[] { "--stringvalue=x-x" }, "x-x")]
        [InlineData(new[] { "--stringvalue=x--x" }, "x--x")]
        [InlineData(new[] { "--stringvalue=x---x" }, "x---x")]
        [InlineData(new[] { "--stringvalue", "5366ebc4-7aa7-4d5a-909c-a415a291a5ad" }, "5366ebc4-7aa7-4d5a-909c-a415a291a5ad")]
        [InlineData(new[] { "--stringvalue=5366ebc4-7aa7-4d5a-909c-a415a291a5ad" }, "5366ebc4-7aa7-4d5a-909c-a415a291a5ad")]
        public void Parse_string_with_dashes_except_in_beginning(string[] arguments, string expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Simple_Options>)result).Value.StringValue);
        }

        [Theory]
        [InlineData(new[] { "--help" }, ErrorType.UnknownOptionError)]
        public void Parse_without_auto_help_should_not_recognize_help_option(string[] arguments, ErrorType errorType)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(arguments, autoHelp: false);

            // Verify outcome
            result.Should().BeOfType<NotParsed<Simple_Options>>()
                .Which.Errors.Should().ContainSingle()
                .Which.Tag.Should().Be(errorType);
        }

        [Theory]
        [InlineData(new[] { "--help" }, true)]
        [InlineData(new[] { "-h" }, true)]
        [InlineData(new[] { "-x" }, false)]
        public void Parse_with_custom_help_option(string[] arguments, bool isHelp)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Custom_Help_Option>(arguments, autoHelp: false);

            // Verify outcome
            result.Should().BeOfType<Parsed<Options_With_Custom_Help_Option>>()
                .Which.Value.Help.Should().Be(isHelp);
        }

        [Theory]
        [InlineData(new[] { "--version" }, ErrorType.UnknownOptionError)]
        public void Parse_without_auto_version_should_not_recognize_version_option(string[] arguments, ErrorType errorType)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(arguments, autoVersion: false);

            // Verify outcome
            result.Should().BeOfType<NotParsed<Simple_Options>>()
                .Which.Errors.Should().ContainSingle()
                .Which.Tag.Should().Be(errorType);
        }

        [Theory]
        [InlineData(new[] { "--version" }, true)]
        [InlineData(new[] { "-v" }, true)]
        [InlineData(new[] { "-s", "s" }, false)]
        public void Parse_with_custom_version_option(string[] arguments, bool isVersion)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Custom_Version_Option>(arguments, autoVersion: false);

            // Verify outcome
            result.Should().BeOfType<Parsed<Options_With_Custom_Version_Option>>()
                .Which.Value.MyVersion.Should().Be(isVersion);
        }

        [Theory]
        [MemberData(nameof(GuidData))]
        public void Parse_Guid(string[] arguments, Options_With_Guid expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Guid>(
                arguments);

            // Verify outcome
            expected.Should().BeEquivalentTo(((Parsed<Options_With_Guid>)result).Value);
        }

        [Fact]
        public void Parse_TimeSpan()
        {
            // Fixture setup
            var expectedResult = new Options_With_TimeSpan { Duration = TimeSpan.FromMinutes(42) };

            // Exercize system 
            var result = InvokeBuild<Options_With_TimeSpan>(
                new[] { "--duration=00:42:00" });

            // Verify outcome
            expectedResult.Should().BeEquivalentTo(((Parsed<Options_With_TimeSpan>)result).Value);
        }

        #region Issue 579
        [Fact]
        public void Should_not_parse_quoted_TimeSpan()
        {
            // Exercize system 
            var result = InvokeBuild<Options_With_TimeSpan>(new[] { "--duration=\"00:42:00\"" });

            var outcome = result as NotParsed<Options_With_TimeSpan>;

            // Verify outcome
            outcome.Should().NotBeNull();
            outcome.Errors.Should().NotBeNullOrEmpty()
                .And.HaveCount(1)
                .And.OnlyContain(e => e.GetType().Equals(typeof(BadFormatConversionError)));
        }
        #endregion

        [Fact]
        public void OptionClass_IsImmutable_HasNoCtor()
        {
            Action act = () => InvokeBuild<ValueWithNoSetterOptions>(new string[] { "Test" }, false, false);

            act.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Be("Type CommandLine.Tests.Unit.Core.InstanceBuilderTests+ValueWithNoSetterOptions appears to be immutable, but no constructor found to accept values.");
        }

        [Fact]
        public void OptionClass_IsImmutable_HasNoCtor_HelpRequested()
        {
            Action act = () => InvokeBuild<ValueWithNoSetterOptions>(new string[] { "--help" });

            act.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Be("Type CommandLine.Tests.Unit.Core.InstanceBuilderTests+ValueWithNoSetterOptions appears to be immutable, but no constructor found to accept values.");
        }

        [Fact]
        public void Options_In_Group_With_No_Values_Generates_MissingGroupOptionError()
        {
            // Fixture setup
            var optionNames = new List<NameInfo>
            {
                new NameInfo("", "option1"),
                new NameInfo("", "option2")
            };
            var expectedResult = new[] { new MissingGroupOptionError("err-group", optionNames) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Group>(
                new[] { "-v 10.42" });

            // Verify outcome
            ((NotParsed<Options_With_Group>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Options_In_Group_With_No_Values_Generates_MissingGroupOptionErrors()
        {
            // Fixture setup
            var optionNames1 = new List<NameInfo>
            {
                new NameInfo("", "option11"),
                new NameInfo("", "option12")
            };
            var optionNames2 = new List<NameInfo>
            {
                new NameInfo("", "option21"),
                new NameInfo("", "option22")
            };
            var expectedResult = new[]
            {
                new MissingGroupOptionError("err-group", optionNames1),
                new MissingGroupOptionError("err-group2", optionNames2)
            };

            // Exercize system 
            var result = InvokeBuild<Options_With_Multiple_Groups>(
                new[] { "-v 10.42" });

            // Verify outcome
            ((NotParsed<Options_With_Multiple_Groups>)result).Errors.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [InlineData("-v", "10.5", "--option1", "test1", "--option2", "test2")]
        [InlineData("-v", "10.5", "--option1", "test1")]
        [InlineData("-v", "10.5", "--option2", "test2")]
        public void Options_In_Group_With_Values_Does_Not_Generate_MissingGroupOptionError(params string[] args)
        {
            // Exercize system 
            var result = InvokeBuild<Options_With_Group>(args);

            // Verify outcome
            result.Should().BeOfType<Parsed<Options_With_Group>>();
        }

        [Fact]
        public void Options_In_Group_WithRequired_Does_Not_Generate_RequiredError()
        {
            // Fixture setup
            var optionNames = new List<NameInfo>
            {
                new NameInfo("", "stingvalue"),
                new NameInfo("s", "shortandlong")
            };
            var expectedResult = new[] { new MissingGroupOptionError("string-group", optionNames) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_Required_OptionGroup>(new string[] { "-x" });

            // Verify outcome
            result.Should().BeOfType<NotParsed<Simple_Options_With_Required_OptionGroup>>();
            var errors = ((NotParsed<Simple_Options_With_Required_OptionGroup>)result).Errors;

            errors.Should().HaveCount(1);
            errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Options_In_Group_Ignore_Option_Group_If_Option_Group_Name_Empty()
        {
            var expectedResult = new[]
            {
                new MissingRequiredOptionError(new NameInfo("", "stringvalue")),
                new MissingRequiredOptionError(new NameInfo("s", "shortandlong"))
            };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_OptionGroup_WithDefaultValue>(new string[] { "-x" });

            // Verify outcome
            result.Should().BeOfType<NotParsed<Simple_Options_With_OptionGroup_WithDefaultValue>>();
            var errors = ((NotParsed<Simple_Options_With_OptionGroup_WithDefaultValue>)result).Errors;

            errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Options_In_Group_Use_Option_Default_Value_When_Available()
        {
            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_OptionGroup_WithOptionDefaultValue>(new string[] { "-x" });

            // Verify outcome
            result.Should().BeOfType<Parsed<Simple_Options_With_OptionGroup_WithOptionDefaultValue>>();
        }

        [Fact]
        public void Options_In_Group_Do_Not_Allow_Mutually_Exclusive_Set()
        {
            var expectedResult = new[]
            {
                new GroupOptionAmbiguityError(new NameInfo("", "stringvalue")),
                new GroupOptionAmbiguityError(new NameInfo("s", "shortandlong"))
            };

            // Exercize system 
            var result = InvokeBuild<Simple_Options_With_OptionGroup_MutuallyExclusiveSet>(new string[] { "-x" });

            // Verify outcome
            result.Should().BeOfType<NotParsed<Simple_Options_With_OptionGroup_MutuallyExclusiveSet>>();
            var errors = ((NotParsed<Simple_Options_With_OptionGroup_MutuallyExclusiveSet>)result).Errors;

            errors.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Parse_int_sequence_with_multi_instance()
        {
            var expected = new[] { 1, 2, 3 };
            var result = InvokeBuild<Options_With_Sequence>(
                new[] { "--int-seq", "1", "2", "--int-seq", "3" },
                multiInstance: true);

            ((Parsed<Options_With_Sequence>)result).Value.IntSequence.Should().BeEquivalentTo(expected);
        }

        #region custom types 
       

        [Theory]
        [InlineData(new[] { "-c", "localhost:8080" }, "localhost", 8080)]
        public void Parse_custom_struct_type(string[] arguments, string expectedServer, int expectedPort)
        {
            //Arrange

            // Act
            var result = InvokeBuild<CustomStructOptions>(arguments);

            // Assert
            var customValue = ((Parsed<CustomStructOptions>)result).Value.Custom;
            customValue.Server.Should().Be(expectedServer);
            customValue.Port.Should().Be(expectedPort);
            customValue.Input.Should().Be(arguments[1]);
        }

        [Theory]
        [InlineData(new[] { "-c", "localhost:8080" }, "localhost", 8080)]
        public void Parse_custom_class_type(string[] arguments, string expectedServer, int expectedPort)
        {
            //Arrange

            // Act
            var result = InvokeBuild<CustomClassOptions>(arguments);

            // Assert
            var customValue = ((Parsed<CustomClassOptions>)result).Value.Custom;
            customValue.Server.Should().Be(expectedServer);
            customValue.Port.Should().Be(expectedPort);
            customValue.Input.Should().Be(arguments[1]);
        }

        #endregion
        private class ValueWithNoSetterOptions
        {
            [Value(0, MetaName = "Test", Default = 0)]
            public int TestValue { get; }
        }


        public static IEnumerable<object[]> RequiredValueStringData
        {
            get
            {
                yield return new object[] { new[] { "value-string" }, new Options_With_Required_Set_To_True_For_Values { StringValue = "value-string" } };
                yield return new object[] { new[] { "another-string", "999" }, new Options_With_Required_Set_To_True_For_Values { StringValue = "another-string", IntValue = 999 } };
                yield return new object[] { new[] { "str with spaces", "-1234567890" }, new Options_With_Required_Set_To_True_For_Values { StringValue = "str with spaces", IntValue = -1234567890 } };
                yield return new object[] { new[] { "1234567890", "1234567890" }, new Options_With_Required_Set_To_True_For_Values { StringValue = "1234567890", IntValue = 1234567890 } };
                yield return new object[] { new[] { "-1234567890", "1234567890" }, new Options_With_Required_Set_To_True_For_Values { StringValue = "-1234567890", IntValue = 1234567890 } };
            }
        }

        public static IEnumerable<object[]> ScalarSequenceStringAdjacentData
        {
            get
            {
                yield return new object[] { new[] { "to-value" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new string[] { } } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0" } } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0", "to-seq-1" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0", "to-seq-1" } } };
                yield return new object[] { new[] { "-s", "cant-capture", "value-anymore" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringOptionSequence = new[] { "cant-capture", "value-anymore" } } };
                yield return new object[] { new[] { "-s", "just-one" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringOptionSequence = new[] { "just-one" } } };

            }
        }

        public static IEnumerable<object[]> ImmutableInstanceData
        {
            get
            {
                yield return new object[] { new string[] { }, new Immutable_Simple_Options(null, new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "--stringvalue=strval0" }, new Immutable_Simple_Options("strval0", new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "-i", "9", "7", "8" }, new Immutable_Simple_Options(null, new[] { 9, 7, 8 }, default(bool), default(long)) };
                yield return new object[] { new[] { "-x" }, new Immutable_Simple_Options(null, new int[] { }, true, default(long)) };
                yield return new object[] { new[] { "9876543210" }, new Immutable_Simple_Options(null, new int[] { }, default(bool), 9876543210L) };
                yield return new object[] { new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" }, new Immutable_Simple_Options("strval0", new[] { 9, 7, 8 }, true, 9876543210L) };
            }
        }
        public static IEnumerable<object[]> ImmutableInstanceDataArgs
        {
            get
            {
                yield return   new object[] { new string[] { } } ;
                yield return new object[] {new [] {"--stringvalue=strval0"}};
                yield return new object[] { new[] { "-i", "9", "7", "8" } };
                yield return new object[] { new[] { "-x" }};
                yield return new object[] { new[] { "9876543210" }};
                yield return new object[] { new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" }};
            }
        }

        public static IEnumerable<object[]> GuidData
        {
            get
            {
                var guid0 = Guid.NewGuid();
                var guid1 = Guid.NewGuid();
                yield return new object[] { new[] { "--txid", guid0.ToStringInvariant() }, new Options_With_Guid { TransactionId = guid0 } };
                yield return new object[] { new[] { "--txid=" + guid1.ToStringInvariant() }, new Options_With_Guid { TransactionId = guid1 } };
                yield return new object[] { new[] { "-t", guid0.ToStringInvariant() }, new Options_With_Guid { TransactionId = guid0 } };
                yield return new object[] { new[] { "-t" + guid1.ToStringInvariant() }, new Options_With_Guid { TransactionId = guid1 } };
            }
        }
    }
}
