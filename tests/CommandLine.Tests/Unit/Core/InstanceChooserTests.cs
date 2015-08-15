// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
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
        private static ParserResult<object> InvokeChoose(
            IEnumerable<Type> types,
            IEnumerable<string> arguments)
        {
            return InstanceChooser.Choose(
                (args, optionSpecs) => Tokenizer.ConfigureTokenizer(StringComparer.Ordinal, false, false)(args, optionSpecs),
                types,
                arguments,
                StringComparer.Ordinal,
                CultureInfo.InvariantCulture,
                Enumerable.Empty<ErrorType>());
        }

        [Fact]
        public void Parse_empty_array_returns_NullInstance()
        {
            // Fixture setup
            var expectedErrors = new[] { new NoVerbSelectedError() };

            // Exercize system 
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) },
                new string[] { });

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
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) },
                new[] { "help" });

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_for_a_valid_verb_generates_HelpVerbRequestedError_with_appropriate_data()
        {
            // Fixture setup
            var expectedErrors = new[] { new HelpVerbRequestedError("commit", typeof(Commit_Verb), true) };

            // Exercize system 
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) },
                new[] { "help", "commit" });

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
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) },
                new[] { "help", "earthunderalienattack" });

            // Verify outcome
            ((NotParsed<object>)result).Errors.ShouldBeEquivalentTo(expectedErrors);

            // Teardown
        }

        [Fact]
        public void Parse_existing_verb_returns_verb_instance()
        {
            // Fixture setup
            var expected = new Add_Verb { Patch = true, FileName = "dummy.bin"};

            // Exercize system 
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) },
                new[] { "add", "--patch", "dummy.bin" });

            // Verify outcome
            Assert.IsType<Add_Verb>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }

        [Fact]
        public void Parse_existing_verb_returns_verb_immutable_instance()
        {
            // Fixture setup
            var expected = new Immutable_Add_Verb(true, default(bool), "dummy.bin");

            // Exercize system 
            var result = InvokeChoose(
                new[] { typeof(Immutable_Add_Verb), typeof(Immutable_Commit_Verb), typeof(Immutable_Clone_Verb) },
                new[] { "add", "--patch", "dummy.bin" });

            // Verify outcome
            Assert.IsType<Immutable_Add_Verb>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }
    
        [Fact]
        public void Parse_sequence_verb_returns_verb_instance()
        {
            // Fixture setup
            var expected = new SequenceOptions { LongSequence = new long[] { }, StringSequence = new[] { "aa", "b" } };

            // Exercize system 
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb), typeof(SequenceOptions) },
                new[] { "sequence", "-s", "aa", "b" });

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
            var result = InvokeChoose(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb), typeof(SequenceOptions) },
                arguments);

            // Verify outcome
            Assert.IsType<SequenceOptions>(((Parsed<object>)result).Value);
            expected.ShouldBeEquivalentTo(((Parsed<object>)result).Value);
            // Teardown
        }
    }
}
