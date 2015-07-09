// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit
{
    public class ParserTests
    {
        [Fact]
        public void When_HelpWriter_is_set_help_screen_is_generated()
        {
            // Fixture setup
            var writer = new StringWriter();
            var sut = new Parser(with => with.HelpWriter = writer);

            // Exercize system
            sut.ParseArguments<FakeOptionWithRequired>(new string[] { });

            // Verify outcome
            var text = writer.ToString();
            Assert.True(text.Length > 0);
            // Teardown
        }

        [Fact]
        public void When_HelpWriter_is_set_help_screen_is_generated_in_verbs_scenario()
        {
            // Fixture setup
            var writer = new StringWriter();
            var sut = new Parser(with => with.HelpWriter = writer);

            // Exercize system
            sut.ParseArguments(new string[] { }, typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions));

            // Verify outcome
            var text = writer.ToString();
            Assert.True(text.Length > 0);
            // Teardown
        }

        [Fact]
        public void When_HelpWriter_is_set_help_screen_is_generated_in_verbs_scenario_using_generic_overload()
        {
            // Fixture setup
            var writer = new StringWriter();
            var sut = new Parser(with => with.HelpWriter = writer);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new string[] { });

            // Verify outcome
            var text = writer.ToString();
            Assert.True(text.Length > 0);
            // Teardown
        }

        [Fact]
        public void Parse_options()
        {
            // Fixture setup
            var expectedOptions = new FakeOptions
                {
                    StringValue = "strvalue", IntSequence = new[] { 1, 2, 3 }
                };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeOptions>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            result.Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_double_dash()
        {
            // Fixture setup
            var expectedOptions = new FakeOptionsWithValues
                {
                    StringValue = "astring",
                    LongValue = 20L,
                    StringSequence = new[] { "--aaa", "-b", "--ccc" },
                    IntValue = 30
                };
            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result = sut.ParseArguments<FakeOptionsWithValues>(
                new[] { "--stringvalue", "astring", "--", "20", "--aaa", "-b", "--ccc", "30" });

            // Verify outcome
            result.Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_double_dash_in_verbs_scenario()
        {
            // Fixture setup
            var expectedOptions = new AddOptions
                {
                    Patch = true,
                    FileName = "--strange-fn"
                };
            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result = sut.ParseArguments(
                new[] { "add", "-p", "--", "--strange-fn" },
                typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<AddOptions>(result.Value);
            result.Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_verbs()
        {
            // Fixture setup
            var expectedOptions = new CloneOptions
                {
                    Quiet = true,
                    Urls = new[] { "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" }
                };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments(
                new[] { "clone", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" },
                typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<CloneOptions>(result.Value);
            result.Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_verbs_using_generic_overload()
        {
            // Fixture setup
            var expectedOptions = new CloneOptions
            {
                Quiet = true,
                Urls = new[] { "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" }
            };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(
                new[] { "clone", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" });

            // Verify outcome
            Assert.IsType<CloneOptions>(result.Value);
            result.Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_to_immutable_instance()
        {
            // Fixture setup
            var expectedOptions = new FakeImmutableOptions(
                "strvalue", new[] { 1, 2, 3 }, default(bool), default(long));
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeImmutableOptions>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            result.Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_immutable_instance_generates_help_requested_error()
        {
            // Fixture setup
            var expectedError = new HelpRequestedError();
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeImmutableOptions>(new[] { "--help" });

            // Verify outcome
            ((NotParsed<FakeImmutableOptions>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<FakeImmutableOptions>)result).Errors.Should().ContainSingle(e => e.Equals(expectedError));
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_immutable_instance_generates_help_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<FakeImmutableOptions>(new[] { "--help" });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            // Teardown
        }

        [Fact]
        public void Explicit_version_request_generates_version_requested_error()
        {
            // Fixture setup
            var expectedError = new VersionRequestedError();
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeOptions>(new[] { "--version" });

            // Verify outcome
            ((NotParsed<FakeOptions>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<FakeOptions>)result).Errors.Should().ContainSingle(e => e.Equals(expectedError));
            // Teardown
        }

        [Fact]
        public void Explicit_version_request_generates_help_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var version = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = version);
            // Creating value to compare
            new Parser(config => config.HelpWriter = help).ParseArguments<FakeOptions>(new[] { "--help" });
            var helpText = help.ToString();

            // Exercize system
            sut.ParseArguments<FakeOptions>(new[] { "--version" });
            var result = version.ToString();

            // Verify outcome
            result.Length.Should().BeLessThan(helpText.Length);
            // Teardown
        }

        [Fact]
        public void Implicit_help_screen_in_verb_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new string [] { });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            // Teardown
        }

        [Fact]
        public void Double_dash_help_dispalys_verbs_index_in_verbs_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new [] { "--help" });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("CommandLine 2.0.51-alpha");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("add       Add file contents to the index.");
            lines[3].ShouldBeEquivalentTo("commit    Record changes to the repository.");
            lines[4].ShouldBeEquivalentTo("clone     Clone a repository into a new directory.");
            lines[5].ShouldBeEquivalentTo("help      Display more information on a specific command.");
            // Teardown
        }
    }
}
