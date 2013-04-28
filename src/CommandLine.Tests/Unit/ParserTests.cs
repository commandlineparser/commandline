// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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
            result.Value.ShouldHave().AllProperties().EqualTo(expectedOptions);
            Assert.False(result.Errors.Any());
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
            result.Value.ShouldHave().AllProperties().EqualTo(expectedOptions);
            Assert.False(result.Errors.Any());
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
            result.Value.ShouldHave().AllRuntimeProperties().EqualTo(expectedOptions);
            Assert.False(result.Errors.Any());
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
            result.Value.ShouldHave().AllRuntimeProperties().EqualTo(expectedOptions);
            Assert.False(result.Errors.Any());
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
            result.Value.ShouldHave().AllRuntimeProperties().EqualTo(expectedOptions);
            Assert.False(result.Errors.Any());
            // Teardown
        }
    }
}
