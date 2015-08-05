// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Globalization;
using System.Linq;
using CommandLine.Core;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class InstanceChooserTests
    {        
        [Fact]
        public void Parse_empty_array_returns_NullInstance()
        {
            // Fixture setup
            var expectedErrors = new[] { new NoVerbSelectedError() };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) },
                new string[] { },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_generates_HelpVerbRequestedError()
        {
            // Fixture setup
            var expectedErrors = new[] { new HelpVerbRequestedError(null, null, false) };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) },
                new[] { "help" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_for_a_valid_verb_generates_HelpVerbRequestedError_with_appropriate_data()
        {
            // Fixture setup
            var expectedErrors = new[] { new HelpVerbRequestedError("commit", typeof(CommitOptions), true) };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) },
                new[] { "help", "commit" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_for_an_invalid_verb_generates_HelpVerbRequestedError_with_Matched_set_to_false()
        {
            // Fixture setup
            var expectedErrors = new[] { new HelpVerbRequestedError(null, null, false) };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) },
                new[] { "help", "earthunderalienattack" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Parse_existing_verb_returns_verb_instance()
        {
            // Fixture setup
            var expected = new AddOptions { Patch = true, FileName = "dummy.bin"};

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) },
                new[] { "add", "--patch", "dummy.bin" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.IsType<AddOptions>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }

        [Fact]
        public void Parse_existing_verb_returns_verb_immutable_instance()
        {
            // Fixture setup
            var expected = new ImmutableAddOptions(true, default(bool), "dummy.bin");

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(ImmutableAddOptions), typeof(ImmutableCommitOptions), typeof(ImmutableCloneOptions) },
                new[] { "add", "--patch", "dummy.bin" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.IsType<ImmutableAddOptions>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }
    
        [Fact]
        public void Parse_sequence_verb_returns_verb_instance()
        {
            // Fixture setup
            var expected = new SequenceOptions { LongSequence = new long[] { }, StringSequence = new[] { "aa", "b" } };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions), typeof(SequenceOptions) },
                new[] { "sequence", "-s", "aa", "b" },
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.IsType<SequenceOptions>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }

        [Theory]
        [InlineData(new[] { "sequence", "-s", "here-one-elem-but-no-sep" }, new[] { "here-one-elem-but-no-sep" })]
        [InlineData(new[] { "sequence", "-shere-one-elem-but-no-sep" }, new[] { "here-one-elem-but-no-sep" })]
        [InlineData(new[] { "sequence", "-s", "eml1@xyz.com,test@unit.org,xyz@srv.it" }, new[] { "eml1@xyz.com", "test@unit.org", "xyz@srv.it" })]
        [InlineData(new[] { "sequence", "-sInlineData@iscool.org,test@unit.org,xyz@srv.it,another,the-last-one" }, new[] { "InlineData@iscool.org", "test@unit.org", "xyz@srv.it", "another", "the-last-one" })]
        public void Parse_sequence_verb_with_separator_returns_verb_instance(string[] arguments, string[] expectedString)
        {
            // Fixture setup
            var expected = new SequenceOptions { LongSequence = new long[] { }, StringSequence = expectedString };

            // Exercize system 
            var result = InstanceChooser.Choose(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions), typeof(SequenceOptions) },
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());

            // Verify outcome
            Assert.IsType<SequenceOptions>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }
    }
}
