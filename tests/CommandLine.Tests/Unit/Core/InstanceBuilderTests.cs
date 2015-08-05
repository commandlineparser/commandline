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
        [Fact]
        public void Explicit_help_request_generates_help_requested_error()
        {
            // Fixture setup
            var fakeOptions = new FakeOptions();
            var expectedResult = new NotParsed<FakeOptions>(
                TypeInfo.Create(typeof(FakeOptions)), new Error[] { new HelpRequestedError() });

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => fakeOptions),
                new[] { "--help" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((Parsed<FakeOptions>)result).Value.LongValue.ShouldBeEquivalentTo(expected);

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
                Maybe.Just<Func<FakeOptionsWithDouble>>(() => new FakeOptionsWithDouble()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((Parsed<FakeOptionsWithDouble>)result).Value.DoubleValue.ShouldBeEquivalentTo(expected);

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
                Maybe.Just<Func<FakeOptionsWithSequence>>(() => new FakeOptionsWithSequence()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());
            
            // Verify outcome
            ((Parsed<FakeOptionsWithSequence>)result).Value.IntSequence.ShouldBeEquivalentTo(expected);
            
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
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((Parsed<FakeOptions>)result).Value.IntSequence.ShouldBeEquivalentTo(expected);
            
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
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMinConstraint>>(() => new FakeOptionsWithSequenceAndOnlyMinConstraint()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());
            
            // Verify outcome
            ((Parsed<FakeOptionsWithSequenceAndOnlyMinConstraint>)result).Value.StringSequence.ShouldBeEquivalentTo(expected);

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
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMaxConstraint>>(() => new FakeOptionsWithSequenceAndOnlyMaxConstraint()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((Parsed<FakeOptionsWithSequenceAndOnlyMaxConstraint>)result).Value.StringSequence.ShouldBeEquivalentTo(expected);

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_gererates_MissingValueOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingValueOptionError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMinConstraint>>(() => new FakeOptionsWithSequenceAndOnlyMinConstraint()),
                new[] { "-s" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSequenceAndOnlyMinConstraint>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_min_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMinConstraintAsValue>>(() => new FakeOptionsWithSequenceAndOnlyMinConstraintAsValue()),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSequenceAndOnlyMinConstraintAsValue>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("s", "string-seq")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMaxConstraint>>(() => new FakeOptionsWithSequenceAndOnlyMaxConstraint()),
                new[] { "--string-seq=one", "two", "three", "this-is-too-much" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSequenceAndOnlyMaxConstraint>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Breaking_max_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSequenceAndOnlyMaxConstraintAsValue>>(() => new FakeOptionsWithSequenceAndOnlyMaxConstraintAsValue()),
                new[] { "one", "two", "three", "this-is-too-much" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSequenceAndOnlyMaxConstraintAsValue>)result).Errors.ShouldBeEquivalentTo(expectedResult);

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
                Maybe.Just<Func<FakeOptionsWithEnum>>(() => new FakeOptionsWithEnum()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithEnum>)result).Value.Colors);

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_index_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithEnum>>(() => new FakeOptionsWithEnum()),
                new[] { "--colors", "3" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithEnum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithEnum>>(() => new FakeOptionsWithEnum()),
                new[] { "--colors", "Yellow" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithEnum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_enum_value_with_wrong_item_name_case_generates_BadFormatConversionError()
        {
            // Fixture setup
            var expectedResult = new[] { new BadFormatConversionError(new NameInfo("", "colors")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithEnum>>(() => new FakeOptionsWithEnum()),
                new[] { "--colors", "RED" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithEnum>)result).Errors.ShouldBeEquivalentTo(expectedResult);

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
                Maybe.Just<Func<FakeOptionsWithValues>>(() => new FakeOptionsWithValues()),
                new[] { "10", "a", "b", "c", "20" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithValues>)result).Value);

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
                Maybe.Just<Func<FakeOptionsWithSequenceWithoutRange>>(() => new FakeOptionsWithSequenceWithoutRange()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithSequenceWithoutRange>)result).Value.LongSequence);

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
                Maybe.Just<Func<FakeOptionsWithSequenceAndSeparator>>(() => new FakeOptionsWithSequenceAndSeparator()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithSequenceAndSeparator>)result).Value.LongSequence);

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
                Maybe.Just<Func<FakeOptionsWithSequenceAndSeparator>>(() => new FakeOptionsWithSequenceAndSeparator()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithSequenceAndSeparator>)result).Value.StringSequence);

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
                Maybe.Just<Func<FakeOptionsWithValues>>(() => new FakeOptionsWithValues()),
                (a, optionSpecs) =>
                    Tokenizer.PreprocessDashDash(a,
                        args => Tokenizer.Tokenize(args, name => NameLookup.Contains(name, optionSpecs, StringComparer.Ordinal))),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithValues>)result).Value);

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSets>>(() => new FakeOptionsWithSets()),
                new[] { "--weburl", "http://mywebsite.org/", "--ftpurl", "fpt://ftpsite.org/" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSets>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Two_required_options_at_the_same_set_and_both_are_true() {
            // Fixture setup
            var expectedResult = new FakeOptionsWithRequiredAndSet {
                FtpUrl = "str1",
                WebUrl = "str2"
            };
            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithRequiredAndSet>>(() => new FakeOptionsWithRequiredAndSet()),
                new[] { "--ftpurl", "str1", "--weburl", "str2" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithRequiredAndSet>)result).Value);
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
                Maybe.Just<Func<FakeOptionsWithRequiredAndSet>>(() => new FakeOptionsWithRequiredAndSet()),
                new[] {""},
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithRequiredAndSet>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Omitting_required_option_gererates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(new NameInfo("", "str")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithRequired>>(() => new FakeOptionsWithRequired()),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithRequired>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Wrong_range_in_sequence_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(new NameInfo("i", "")) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                new [] { "-i", "10" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_long_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("xyz") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                new[] { "--stringvalue", "abc", "--xyz" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                new[] { "-z", "-x" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.ShouldBeEquivalentTo(expectedResult);

            // Teardown
        }

        [Fact]
        public void Parse_unknown_short_option_in_option_group_gererates_UnknownOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new UnknownOptionError("z") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                new[] { "-zx" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.ShouldBeEquivalentTo(expectedResult);

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
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((Parsed<FakeOptions>)result).Value.StringValue.ShouldBeEquivalentTo(expected);

            // Teardown
        }

        [Fact]
        public void Breaking_required_constraint_in_string_scalar_as_value_generates_MissingRequiredOptionError()
        {
            // Fixture setup
            var expectedResult = new[] { new MissingRequiredOptionError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithRequiredValue>>(() => new FakeOptionsWithRequiredValue()),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithRequiredValue>)result).Errors.ShouldBeEquivalentTo(expectedResult);

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptions>)result).Value.StringValue);

            // Teardown
        }

        [Fact]
        public void Breaking_equal_min_max_constraint_in_string_sequence_as_value_gererates_SequenceOutOfRangeError()
        {
            // Fixture setup
            var expectedResult = new[] { new SequenceOutOfRangeError(NameInfo.EmptyName) };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSequenceMinMaxEqual>>(() => new FakeOptionsWithSequenceMinMaxEqual()),
                new[] { "one", "two", "this-is-too-much" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptionsWithSequenceMinMaxEqual>)result).Errors.ShouldBeEquivalentTo(expectedResult);

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithNullables>>(() => new FakeOptionsWithNullables()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithNullables>)result).Value.NullableInt);

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithNullables>>(() => new FakeOptionsWithNullables()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithNullables>)result).Value.NullableLong);

            // Teardown
        }

        [Theory]
        [InlineData(new[] { "--filename", "log-20150626.txt" }, "log-20150626.txt", true)]
        [InlineData(new string[] { }, null, false)]
        public void Parse_fsharp_option_string(string[] arguments, string expectedValue, bool expectedSome)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithFSharpOption>>(() => new FakeOptionsWithFSharpOption()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            if (((Parsed<FakeOptionsWithFSharpOption>)result).Value.FileName != null)
            {
                expectedValue.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithFSharpOption>)result).Value.FileName.Value);
            }
            expectedSome.ShouldBeEquivalentTo(FSharpOption<string>.get_IsSome(((Parsed<FakeOptionsWithFSharpOption>)result).Value.FileName));

            // Teardown
        }

        [Theory]
        [InlineData(new[] { "1234567" }, 1234567, true)]
        [InlineData(new string[] { }, default(int), false)]
        public void Parse_fsharp_option_int(string[] arguments, int expectedValue, bool expectedSome)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithFSharpOption>>(() => new FakeOptionsWithFSharpOption()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            if (((Parsed<FakeOptionsWithFSharpOption>)result).Value.Offset != null)
            {
                expectedValue.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithFSharpOption>)result).Value.Offset.Value);
            }
            expectedSome.ShouldBeEquivalentTo(FSharpOption<int>.get_IsSome(((Parsed<FakeOptionsWithFSharpOption>)result).Value.Offset));

            // Teardown
        }

    
        [Fact]
        public void Min_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithMinZero>>(() => new FakeOptionsWithMinZero()),
                new string[] {},
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.Throws<ApplicationException>(test);
        }

        [Fact]
        public void Max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithMaxZero>>(() => new FakeOptionsWithMaxZero()),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.Throws<ApplicationException>(test);
        }

        [Fact]
        public void Min_and_max_constraint_set_to_zero_throws_exception()
        {
            // Exercize system 
            Action test = () => InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithMinMaxZero>>(() => new FakeOptionsWithMinMaxZero()),
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithNamedAndEmptySets>>(() => new FakeOptionsWithNamedAndEmptySets()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            if (type == ParserResultType.NotParsed)
            {
                ((NotParsed<FakeOptionsWithNamedAndEmptySets>)result).Errors.Should().HaveCount(x => x == expected);
            }
            else if (type == ParserResultType.Parsed)
            {
                result.Should().BeOfType<Parsed<FakeOptionsWithNamedAndEmptySets>>();
            }
        }

        [Theory]
        [InlineData(new[] { "--stringvalue", "abc", "--stringvalue", "def" }, 1)]
        public void Specifying_options_two_or_more_times_generates_RepeatedOptionError(string[] arguments, int expected)
        {
            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.Should().HaveCount(x => x == expected);
        }

        [Theory]
        [InlineData(new[] { "--inputfile=file1.bin" }, "file1.bin")]
        [InlineData(new[] { "--inputfile", "file2.txt" }, "file2.txt")]
        public void Can_define_options_on_interface_properties(string[] arguments, string expected)
        {
            // Exercize system
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeInterfaceOptions>>(() => new FakeInterfaceOptions()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeInterfaceOptions>)result).Value.InputFile);
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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithTwoRequiredAndSets>>(() => new FakeOptionsWithTwoRequiredAndSets()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            if (type == ParserResultType.NotParsed)
            {
                ((NotParsed<FakeOptionsWithTwoRequiredAndSets>)result).Errors.Should().HaveCount(x => x == expected);
            }
            else if (type == ParserResultType.Parsed)
            {
                result.Should().BeOfType<Parsed<FakeOptionsWithTwoRequiredAndSets>>();
            }
        }

        [Theory]
        [MemberData("RequiredValueStringData")]
        public void Parse_string_scalar_with_required_constraint_as_value(string[] arguments, FakeOptionsWithRequiredValue expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithRequiredValue>>(() => new FakeOptionsWithRequiredValue()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithRequiredValue>)result).Value);

            // Teardown
        }

        [Theory]
        [MemberData("ScalarSequenceStringAdjacentData")]
        public void Parse_string_scalar_and_sequence_adjacent(string[] arguments, FakeOptionsWithScalarValueAndSequenceStringAdjacent expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithScalarValueAndSequenceStringAdjacent>>(() => new FakeOptionsWithScalarValueAndSequenceStringAdjacent()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithScalarValueAndSequenceStringAdjacent>)result).Value);

            // Teardown
        }

        [Fact]
        public void Parse_to_mutable()
        {
            // Fixture setup
            var expectedResult = new FakeOptions { StringValue="strval0", IntSequence=new[] { 9, 7, 8 }, BoolValue = true,  LongValue = 9876543210L };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptions>>(() => new FakeOptions()),
                new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<FakeOptions>)result).Value);

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
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithTwoRequired>>(() => new FakeOptionsWithTwoRequired()),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            var errors = ((NotParsed<FakeOptionsWithTwoRequired>)result).Errors;
            errors.OfType<MissingRequiredOptionError>().Should().HaveCount(x => x == expected);
        }

        [Theory]
        [MemberData("ImmutableInstanceData")]
        public void Parse_to_immutable_instance(string[] arguments, FakeImmutableOptions expected)
        {
            // Fixture setup in attributes

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Nothing<Func<FakeImmutableOptions>>(),
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expected.ShouldBeEquivalentTo(((Parsed<FakeImmutableOptions>)result).Value);

            // Teardown
        }

        [Fact]
        public static void Parse_to_type_with_single_string_ctor_builds_up_correct_instance()
        {
            // Fixture setup
            var expectedResult = new FakeOptionsWithSimpleType { EndPoint = new Uri("http://localhost/test/"), MyValue = new MySimpleType("custom-value") };

            // Exercize system 
            var result = InstanceBuilder.Build(
                Maybe.Just<Func<FakeOptionsWithSimpleType>>(() => new FakeOptionsWithSimpleType()),
                new[] { "--endpoint=http://localhost/test/", "custom-value" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            expectedResult.ShouldBeEquivalentTo(((Parsed<FakeOptionsWithSimpleType>)result).Value);

            // Teardown
        }

        public static IEnumerable<object> RequiredValueStringData
        {
            get
            {
                yield return new object[] { new[] { "value-string" }, new FakeOptionsWithRequiredValue { StringValue = "value-string" } };
                yield return new object[] { new[] { "another-string", "999" }, new FakeOptionsWithRequiredValue { StringValue = "another-string", IntValue = 999 } };
                yield return new object[] { new[] { "str with spaces", "-1234567890" }, new FakeOptionsWithRequiredValue { StringValue = "str with spaces", IntValue = -1234567890 } };
                yield return new object[] { new[] { "1234567890", "1234567890" }, new FakeOptionsWithRequiredValue { StringValue = "1234567890", IntValue = 1234567890 } };
                yield return new object[] { new[] { "-1234567890", "1234567890" }, new FakeOptionsWithRequiredValue { StringValue = "-1234567890", IntValue = 1234567890 } };
            }
        }

        public static IEnumerable<object> ScalarSequenceStringAdjacentData
        {
            get
            {
                yield return new object[] { new[] { "to-value" }, new FakeOptionsWithScalarValueAndSequenceStringAdjacent { StringValueWithIndexZero = "to-value", StringOptionSequence = new string[] {} } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0" }, new FakeOptionsWithScalarValueAndSequenceStringAdjacent { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0" } } };
                yield return new object[] { new[] { "to-value", "-s", "to-seq-0", "to-seq-1" }, new FakeOptionsWithScalarValueAndSequenceStringAdjacent { StringValueWithIndexZero = "to-value", StringOptionSequence = new[] { "to-seq-0", "to-seq-1" } } };
                yield return new object[] { new[] { "-s", "cant-capture", "value-anymore" }, new FakeOptionsWithScalarValueAndSequenceStringAdjacent { StringOptionSequence = new[] { "cant-capture", "value-anymore" } } };
                yield return new object[] { new[] { "-s", "just-one" }, new FakeOptionsWithScalarValueAndSequenceStringAdjacent { StringOptionSequence = new[] { "just-one" } } };

            }
        }

        public static IEnumerable<object> ImmutableInstanceData
        {
            get
            {
                yield return new object[] { new string[] { }, new FakeImmutableOptions("", new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "--stringvalue=strval0" }, new FakeImmutableOptions("strval0", new int[] { }, default(bool), default(long)) };
                yield return new object[] { new[] { "-i", "9", "7", "8" }, new FakeImmutableOptions("", new[] { 9, 7, 8 }, default(bool), default(long)) };
                yield return new object[] { new[] { "-x" }, new FakeImmutableOptions("", new int[] { }, true, default(long)) };
                yield return new object[] { new[] { "9876543210" }, new FakeImmutableOptions("", new int[] { }, default(bool), 9876543210L) };
                yield return new object[] { new[] { "--stringvalue=strval0", "-i", "9", "7", "8", "-x", "9876543210" }, new FakeImmutableOptions("strval0", new[] { 9, 7, 8 }, true, 9876543210L) };
            }
        }
    }
}
