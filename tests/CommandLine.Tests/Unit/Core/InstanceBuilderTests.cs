// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.FSharp.Core;
using CommandLine.Core;
using CSharpx;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class InstanceBuilderTests
    {
        private static ParserResult<T> InvokeBuild<T>(string[] arguments)
            where T : new()
        {
            return InstanceBuilder.Build(
                Maybe.Just<Func<T>>(() => new T()),
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());
        }

        private static ParserResult<T> InvokeBuildImmutable<T>(string[] arguments)
        {
            return InstanceBuilder.Build(
                Maybe.Nothing<Func<T>>(),
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
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
            result.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Theory]
        [InlineData(new[] {"-123"}, -123L)]
        [InlineData(new[] { "-1" }, -1L)]
        [InlineData(new[] { "-9223372036854775807" }, -9223372036854775807)] // long.MaxValue * -1
        public void Parse_negative_long_value(string[] arguments, long expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.LongValue.ShouldBeEquivalentTo(expected);

            // Teardown
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
            ((Parsed<Simple_Options_With_Double_Value>)result).Value.DoubleValue.ShouldBeEquivalentTo(expected);

            // Teardown
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
            ((Parsed<Options_With_Sequence>)result).Value.IntSequence.ShouldBeEquivalentTo(expected);
            
            // Teardown
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
            ((Parsed<Simple_Options>)result).Value.IntSequence.ShouldBeEquivalentTo(expected);
            
            // Teardown
        }

        [Theory]
        [InlineData(new[] {"-s", "just-one"}, new[] {"just-one"})]
        [InlineData(new[] {"-sjust-one-samearg"}, new[] {"just-one-samearg"})]
        [InlineData(new[] {"-s", "also-two", "are-ok" }, new[] { "also-two", "are-ok" })]
        [InlineData(new[] { "--string-seq", "one", "two", "three" }, new[] { "one", "two", "three" })]
        [InlineData(new[] { "--string-seq=one", "two", "three", "4" }, new[] { "one", "two", "three", "4" })]
        public void Parse_string_sequence_with_only_min_constraint(string[] arguments, string[] expected)
        {
            // Fixture setup with attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint>(
                arguments);

            // Verify outcome
            ((Parsed<Options_With_Sequence_And_Only_Min_Constraint>)result).Value.StringSequence.ShouldBeEquivalentTo(expected);

            // Teardown
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
            ((Parsed<Options_With_Sequence_And_Only_Max_Constraint>)result).Value.StringSequence.ShouldBeEquivalentTo(expected);

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_gererates_MissingValueOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingValueOptionError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint>(
                new[] { "-s" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Min_Constraint>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Min_Constraint_For_Value>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Min_Constraint_For_Value>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Max_Constraint>(
                new[] { "--string-seq=one", "two", "three", "this-is-too-much" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Max_Constraint>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_And_Only_Max_Constraint_For_Value>(
                new[] { "one", "two", "three", "this-is-too-much" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_And_Only_Max_Constraint_For_Value>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Simple_Options_With_Enum>)result).Value.Colors);

            // Teardown
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
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            ((NotParsed<Simple_Options_With_Enum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            expectedResult.ShouldBeEquivalentTo(((Parsed<Simple_Options_With_Values>)result).Value);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Sequence_Without_Range_For_Value>)result).Value.LongSequence);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Sequence_Having_Separator_Set>)result).Value.LongSequence);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Sequence_Having_Separator_Set>)result).Value.StringSequence);

            // Teardown
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
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<Simple_Options_With_Values>)result).Value);

            // Teardown
        }

        [Fact]
        public void Parse_option_from_different_sets_gererates_MutuallyExclusiveSetError()
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
            ((NotParsed<Options_With_Two_Sets>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_both_are_true() {
            // Fixture setup
            var expectedResult = new Options_With_Required_Set_To_True_Within_Same_Set {
                FtpUrl = "str1",
                WebUrl = "str2"
            };
            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_Within_Same_Set>(
                new[] { "--ftpurl", "str1", "--weburl", "str2" });

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<Options_With_Required_Set_To_True_Within_Same_Set>)result).Value);
            // Teardown
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_none_are_true() {
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
            ((NotParsed<Options_With_Required_Set_To_True_Within_Same_Set>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Omitting_required_option_gererates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(new NameInfo("", "str")) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True>(
                new string[] { });

            // Verify outcome
            ((NotParsed<Options_With_Required_Set_To_True>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Wrong_range_in_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("i", "")) };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-i", "10" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_long_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("xyz") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "--stringvalue", "abc", "--xyz" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-z", "-x" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_in_option_group_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "-zx" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Theory]
        [InlineData(new[] {"--stringvalue", "this-value"}, "this-value")]
        [InlineData(new[] {"--stringvalue=this-other"}, "this-other")]
        public void Omitting_names_assumes_identifier_as_long_name(string[] arguments, string expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                arguments);

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.StringValue.ShouldBeEquivalentTo(expected);

            // Teardown
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
            ((NotParsed<Options_With_Required_Set_To_True_For_Values>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Simple_Options>)result).Value.StringValue);

            // Teardown
        }

        [Fact]
        public void Breaking_equal_min_max_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InvokeBuild<Options_With_Sequence_Having_Both_Min_And_Max_Equal>(
                new[] { "one", "two", "this-is-too-much" });

            // Verify outcome
            ((NotParsed<Options_With_Sequence_Having_Both_Min_And_Max_Equal>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Nullables>)result).Value.NullableInt);

            // Teardown
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
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Nullables>)result).Value.NullableLong);

            // Teardown
        }

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
                expectedValue.ShouldBeEquivalentTo(((Parsed<Options_With_FSharpOption>)result).Value.FileName.Value);
            }
            expectedSome.ShouldBeEquivalentTo(FSharpOption<string>.get_IsSome(((Parsed<Options_With_FSharpOption>)result).Value.FileName));

            // Teardown
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
                expectedValue.ShouldBeEquivalentTo(((Parsed<Options_With_FSharpOption>)result).Value.Offset.Value);
            }
            expectedSome.ShouldBeEquivalentTo(FSharpOption<int>.get_IsSome(((Parsed<Options_With_FSharpOption>)result).Value.Offset));

            // Teardown
        }

    
        [Fact]
        public void Min_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Min_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<ApplicationException>(test);
        }

        [Fact]
        public void Max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Max_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<ApplicationException>(test);
        }

        [Fact]
        public void Min_and_max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InvokeBuild<Options_With_Both_Min_And_Max_Set_To_Zero>(
                new string[] { });

            // Verify outcome
            Assert.Throws<ApplicationException>(test);
        }

        [Theory]
        [InlineData(new[] {"--weburl", "value.com", "--verbose"}, ParserResultType.Parsed, 0)]
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
        [InlineData(new[] { "--inputfile=file1.bin" }, "file1.bin")]
        [InlineData(new[] { "--inputfile", "file2.txt" }, "file2.txt")]
        public void Can_define_options_on_interface_properties(string[] arguments, string expected)
        {
            // Exercize system
            var result = InvokeBuild<Options_With_Interface>(
                arguments);

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Interface>)result).Value.InputFile);
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
        [MemberData("RequiredValueStringData")]
        public void Parse_string_scalar_with_required_constraint_as_value(string[] arguments, Options_With_Required_Set_To_True_For_Values expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Required_Set_To_True_For_Values>(
                arguments);

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Required_Set_To_True_For_Values>)result).Value);

            // Teardown
        }

        [Theory]
        [MemberData("ScalarSequenceStringAdjacentData")]
        public void Parse_string_scalar_and_sequence_adjacent(string[] arguments, Options_With_Scalar_Value_And_Adjacent_SequenceString expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuild<Options_With_Scalar_Value_And_Adjacent_SequenceString>(
                arguments);

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<Options_With_Scalar_Value_And_Adjacent_SequenceString>)result).Value);

            // Teardown
        }

        [Fact]
        public void Parse_to_mutable()
        {
            // Fixture setup
            var expectedResult = new Simple_Options { StringValue="strval0", IntSequence=new[] { 9, 7, 8 }, BoolValue = true,  LongValue = 9876543210L };

            // Exercize system 
            var result = InvokeBuild<Simple_Options>(
                new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" });

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<Simple_Options>)result).Value);

            // Teardown
        }

        [Theory]
        [InlineData(new string[] { }, 2)]
        [InlineData(new [] { "--str=val0" }, 1)]
        [InlineData(new [] { "--long=9" }, 1)]
        [InlineData(new [] { "--int=7" }, 2)]
        [InlineData(new [] { "--str", "val1", "--int=3" }, 1)]
        [InlineData(new [] { "--long", "9", "--int=11" }, 1)]
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
        [MemberData("ImmutableInstanceData")]
        public void Parse_to_immutable_instance(string[] arguments, Immutable_Simple_Options expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InvokeBuildImmutable<Immutable_Simple_Options>(
                arguments);

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<Immutable_Simple_Options>)result).Value);

            // Teardown
        }

        [Fact]
        public static void Parse_to_type_with_single_string_ctor_builds_up_correct_instance()
        {
            // Fixture setup
            var expectedResult = new Options_With_Uri_And_SimpleType { EndPoint = new Uri("http://localhost/test/"), MyValue = new MySimpleType("custom-value") };

            // Exercize system 
            var result = InvokeBuild<Options_With_Uri_And_SimpleType>(
                new[] { "--endpoint=http://localhost/test/", "custom-value" });

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<Options_With_Uri_And_SimpleType>)result).Value);

            // Teardown
        }

        public static IEnumerable<object> RequiredValueStringData
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

        public static IEnumerable<object> ScalarSequenceStringAdjacentData
        {
            get
            {
                yield return new object[] { new[] { "to-value" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new string[] {} } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0" } } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0", "to-seq-1" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0", "to-seq-1" } } };
                yield return new object[] { new[] { "-s", "cant-capture", "value-anymore" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringOptionSequence = new[] { "cant-capture", "value-anymore" } } };
                yield return new object[] { new[] { "-s", "just-one" }, new Options_With_Scalar_Value_And_Adjacent_SequenceString { StringOptionSequence = new[] { "just-one" } } };

            }
        }

        public static IEnumerable<object> ImmutableInstanceData
        {
            get
            {
                yield return new object[] { new string[] { }, new Immutable_Simple_Options("", new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "--stringvalue=strval0" }, new Immutable_Simple_Options("strval0", new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "-i", "9", "7", "8" }, new Immutable_Simple_Options("", new[] { 9, 7, 8 }, default(bool), default(long)) };
                yield return new object[] { new[] { "-x" }, new Immutable_Simple_Options("", new int[] { }, true, default(long)) };
                yield return new object[] { new[] { "9876543210" }, new Immutable_Simple_Options("", new int[] { }, default(bool), 9876543210L) };
                yield return new object[] { new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" }, new Immutable_Simple_Options("strval0", new[] { 9, 7, 8 }, true, 9876543210L) };
            }
        }
    }
}
