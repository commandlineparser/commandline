// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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

        [Fact]
        public void Parse_negative_int_value()
        {
            // Fixture setup
            var expectedResult = -123;

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new[] { "-123" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.Equal(expectedResult, result.Value.LongValue);

            // Teardown
        }

        [Fact]
        public void Parse_double_value()
        {
            // Fixture setup
            var expectedResult = .123D;

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithDouble(),
                new[] { "0.123" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.Equal(expectedResult, result.Value.DoubleValue);

            // Teardown
        }

        [Fact]
        public void Parse_negative_double_value()
        {
            // Fixture setup
            var expectedResult = -.123D;

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithDouble(),
                new[] { "-0.123" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.Equal(expectedResult, result.Value.DoubleValue);

            // Teardown
        }

        [Fact]
        public void Parse_int_sequence_with_range()
        {
            // Fixture setup
            var expectedResult = new[] { 10, 20, 30, 40 };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptions(),
                new[] { "-i", "10", "20", "30", "40" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.True(expectedResult.SequenceEqual(result.Value.IntSequence));

            // Teardown
        }

        [Fact]
        public void Parse_enum_value()
        {
            // Fixture setup
            var expectedResult = new FakeOptionsWithEnum
                {
                    Colors = Colors.Green
                };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithEnum(),
                new[] { "--colors", "Green" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldHave().AllProperties().EqualTo(result.Value);

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
            expectedResult.ShouldHave().AllProperties().EqualTo(result.Value);

            // Teardown
        }

        [Fact]
        public void Parse_sequence_value_without_range_constraints()
        {
            // Fixture setup
            var expectedResult = new FakeOptionsWithSequenceWithoutRange
            {
                LongSequence = new[] { 1L, 2L, 3L, 4L, 5L, 6L }
            };

            // Exercize system 
            var result = InstanceBuilder.Build(
                () => new FakeOptionsWithSequenceWithoutRange(),
                new[] { "1", "2", "3", "4", "5", "6" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture);

            // Verify outcome
            expectedResult.ShouldHave().AllProperties().EqualTo(result.Value);

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
            expectedResult.ShouldHave().AllProperties().EqualTo(result.Value);

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
    }
}
