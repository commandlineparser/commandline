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
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.IsType<NullInstance>(result.Value);
            Assert.True(expectedErrors.SequenceEqual(result.Errors));
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
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.IsType<NullInstance>(result.Value);
            Assert.True(expectedErrors.SequenceEqual(result.Errors));
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
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.IsType<NullInstance>(result.Value);
            Assert.True(expectedErrors.SequenceEqual(result.Errors));
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
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.IsType<NullInstance>(result.Value);
            Assert.True(expectedErrors.SequenceEqual(result.Errors));
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
                CultureInfo.InvariantCulture);

            // Verify outcome
            Assert.IsType<AddOptions>(result.Value);
            expected.ShouldHave().AllRuntimeProperties().EqualTo(result.Value);
            // Teardown
        }    
    }
}
