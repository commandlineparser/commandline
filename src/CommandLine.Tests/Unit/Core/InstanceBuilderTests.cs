// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using CommandLine.Core;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class InstanceBuilderTests
    {
        [Fact]
        public void Explicit_help_request_generates_help_requested_error()
        {
            // Fixture setup
            var fakeOptions = new FakeOptions();
            var expectedResult = ParserResult.Create(
                ParserResultType.Options,
                fakeOptions, new Error[] { new HelpRequestedError() });

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => fakeOptions,
                new[] { "--help" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.Equals(result));

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
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.Equal(expected, result.Value.LongValue);

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithDouble(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.Equal(expected, result.Value.DoubleValue);

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequence(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expected.SequenceEqual(result.Value.IntSequence));

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
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expected.SequenceEqual(result.Value.IntSequence));

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMinConstraint(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expected.SequenceEqual(result.Value.StringSequence));

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMaxConstraint(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expected.SequenceEqual(result.Value.StringSequence));

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_gererates_MissingValueOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingValueOptionError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMinConstraint(),
                new[] { "-s" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMinConstraintAsValue(),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }


        [Fact]
        public void Breaking_max_constraint_in_string_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMaxConstraint(),
                new[] { "--string-seq=one", "two", "three", "this-is-too-much" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndOnlyMaxConstraintAsValue(),
                new[] { "one", "two", "three", "this-is-too-much" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithEnum(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expected.ShouldBeEquivalentTo(result.Value.Colors);

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_index_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithEnum(),
                new[] { "--colors", "3" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithEnum(),
                new[] { "--colors", "Yellow" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_case_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithEnum(),
                new[] { "--colors", "RED" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_values_partitioned_between_sequence_and_scalar()
        {
            // Fixture setup
            var expectedResult = new FakeOptionsWithValues
                {
                    StringValue = string.Empty,
                    LongValue = 10L,
                    StringSequence = new[] { "a", "b", "c" },
                    IntValue = 20
                };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithValues(),
                new[] { "10", "a", "b", "c", "20" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(result.Value);

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceWithoutRange(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expected.ShouldBeEquivalentTo(result.Value.LongSequence);

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndSeparator(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expected.ShouldBeEquivalentTo(result.Value.LongSequence);

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
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceAndSeparator(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expected.ShouldBeEquivalentTo(result.Value.StringSequence);

            // Teardown
        }

        /// <summary>
        /// https://github.com/gsscoder/commandline/issues/31
        /// </summary>
        [Fact]
        public void Double_dash_force_subsequent_arguments_as_values()
        {
            // Fixture setup
            var expectedResult = new FakeOptionsWithValues
                {
                    StringValue = "str1",
                    LongValue = 10L,
                    StringSequence = new[] { "-a", "--bee", "-c" },
                    IntValue = 20
                };
            var arguments = new[] { "--stringvalue", "str1", "--", "10", "-a", "--bee", "-c", "20" };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithValues(),
                (a, optionSpecs) =>
                    Tokenizer.PreprocessDashDash(a,
                        args => Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, StringComparer.Ordinal))),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(result.Value);

            // Teardown
        }

        [Fact]
        public void Parse_option_from_different_sets_gererates_MutuallyExclusiveSetError()
        {
            // Fixture setup
            var expectedResult = new[]
                {
                    new MutuallyExclusiveSetError(new NameInfo("", "weburl")),
                    new MutuallyExclusiveSetError(new NameInfo("", "ftpurl"))
                };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSets(),
                new[] { "--weburl", "http://mywebsite.org/", "--ftpurl", "fpt://ftpsite.org/" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_one_is_true() {
            // Fixture setup
            var expectedResult = new FakeOptionWithRequiredAndSet {
                FtpUrl = "str1",
                WebUrl = null
            };
            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionWithRequiredAndSet(),
                new[] { "--ftpurl", "str1"},
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(result.Value);
            // Teardown
        }


        [Fact]
        public void Two_required_options_at_the_same_set_and_both_are_true() {
            // Fixture setup
            var expectedResult = new FakeOptionWithRequiredAndSet {
                FtpUrl = "str1",
                WebUrl = "str2"
            };
            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionWithRequiredAndSet(),
                new[] { "--ftpurl", "str1", "--weburl", "str2" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(result.Value);
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
            var result = InstanceBuilder.Build(
                () => new FakeOptionWithRequiredAndSet(),
                new[] {""},
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));
            // Teardown
        }

        [Fact]
        public void Omitting_required_option_gererates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(new NameInfo("", "str")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionWithRequired(),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Wrong_range_in_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("i", "")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new [] { "-i", "10" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_unknown_long_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("xyz") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new[] { "--stringvalue", "abc", "--xyz" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new[] { "-z", "-x" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_in_option_group_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new[] { "-zx" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Errors));

            // Teardown
        }

        [Theory]
        [InlineData(new[] {"--stringvalue", "this-value"}, "this-value")]
        [InlineData(new[] {"--stringvalue=this-other"}, "this-other")]
        public void Omitting_names_assumes_identifier_as_long_name(string[] arguments, string expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expected.Equals(result.Value.StringValue));

            // Teardown
        }
    }
}
