// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
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
            sut.ParseArguments<FakeOptionsWithRequired>(new string[] { });

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
            text.Should().NotBeEmpty();
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
            text.Should().NotBeEmpty();
            // Teardown
        }

        [Fact]
        public void Parse_options()
        {
            // Fixture setup
            var expectedOptions = new FakeOptions { StringValue = "strvalue", IntSequence = new[] { 1, 2, 3 } };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeOptions>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            ((Parsed<FakeOptions>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Theory]
        [InlineData("file", new[] { "-o", "file" })]
        [InlineData("file", new[] { "-ofile" })]
        [InlineData("hile", new[] { "-o", "hile" })]
        [InlineData("hile", new[] { "-ohile" })]
        public void Parse_options_with_short_name(string outputFile, string[] args)
        {
            // Fixture setup
            var expectedOptions = new FakeOptionsWithSwitches { OutputFile = outputFile };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeOptionsWithSwitches>(args);

            // Verify outcome
            ((Parsed<FakeOptionsWithSwitches>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_repeated_options_with_default_parser()
        {
            // Fixture setup
            var sut = Parser.Default;

            // Exercize system
            var result = sut.ParseArguments<FakeOptionsWithSwitches>(new[] { "-i", "-i", "-o", "file" });

            // Verify outcome
            Assert.IsType<NotParsed<FakeOptionsWithSwitches>>(result);
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
            var result =
                sut.ParseArguments<FakeOptionsWithValues>(
                    new[] { "--stringvalue", "astring", "--", "20", "--aaa", "-b", "--ccc", "30" });

            // Verify outcome
            ((Parsed<FakeOptionsWithValues>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_double_dash_in_verbs_scenario()
        {
            // Fixture setup
            var expectedOptions = new AddOptions { Patch = true, FileName = "--strange-fn" };
            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result = sut.ParseArguments(
                new[] { "add", "-p", "--", "--strange-fn" },
                typeof(AddOptions),
                typeof(CommitOptions),
                typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<AddOptions>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_verbs()
        {
            // Fixture setup
            var expectedOptions = new CloneOptions
                                  {
                                      Quiet = true,
                                      Urls =
                                          new[]
                                          {
                                              "http://gsscoder.github.com/",
                                              "http://yes-to-nooo.github.com/"
                                          }
                                  };
            var sut = new Parser();

            // Exercize system
            var result =
                sut.ParseArguments(
                    new[] { "clone", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" },
                    typeof(AddOptions),
                    typeof(CommitOptions),
                    typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<CloneOptions>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Theory]
        [InlineData("blabla", new[] { "commit", "-m", "blabla" })]
        [InlineData("blabla", new[] { "commit", "-mblabla" })]
        [InlineData("plapla", new[] { "commit", "-m", "plapla" })]
        [InlineData("plapla", new[] { "commit", "-mplapla" })]
        public void Parse_options_with_short_name_in_verbs_scenario(string message, string[] args)
        {
            // Fixture setup
            var expectedOptions = new CommitOptions() { Message = message };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments(
                args,
                typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<CommitOptions>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_repeated_options_with_default_parser_in_verbs_scenario()
        {
            // Fixture setup
            var sut = Parser.Default;

            // Exercize system
            var result = sut.ParseArguments(
                new[] { "clone", "-q", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" },
                typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions));

            // Verify outcome
            Assert.IsType<NotParsed<object>>(result);
            // Teardown
        }

        [Fact]
        public void Parse_verbs_using_generic_overload()
        {
            // Fixture setup
            var expectedOptions = new CloneOptions
                                  {
                                      Quiet = true,
                                      Urls =
                                          new[]
                                          {
                                              "http://gsscoder.github.com/",
                                              "http://yes-to-nooo.github.com/"
                                          }
                                  };
            var sut = new Parser();

            // Exercize system
            var result =
                sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(
                    new[] { "clone", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" });

            // Verify outcome
            Assert.IsType<CloneOptions>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_to_immutable_instance()
        {
            // Fixture setup
            var expectedOptions = new FakeImmutableOptions("strvalue", new[] { 1, 2, 3 }, default(bool), default(long));
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<FakeImmutableOptions>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            ((Parsed<FakeImmutableOptions>)result).Value.ShouldBeEquivalentTo(expectedOptions);
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
        public void Explicit_version_request_generates_version_info_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<FakeOptions>(new[] { "--version" });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines.Should().HaveCount(x => x == 1);
            lines[0].Should().StartWithEquivalent("CommandLine");
            // Teardown
        }

        [Fact]
        public void Implicit_help_screen_in_verb_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new string[] { });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("No verb selected.");
            lines[4].ShouldBeEquivalentTo("add        Add file contents to the index.");
            lines[5].ShouldBeEquivalentTo("commit     Record changes to the repository.");
            lines[6].ShouldBeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[7].ShouldBeEquivalentTo("help       Display more information on a specific command.");
            lines[8].ShouldBeEquivalentTo("version    Display version information.");
            // Teardown
        }

        [Fact]
        public void Double_dash_help_dispalys_verbs_index_in_verbs_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new[] { "--help" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("add        Add file contents to the index.");
            lines[3].ShouldBeEquivalentTo("commit     Record changes to the repository.");
            lines[4].ShouldBeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[5].ShouldBeEquivalentTo("help       Display more information on a specific command.");
            lines[6].ShouldBeEquivalentTo("version    Display version information.");
            // Teardown
        }

        [Theory]
        [InlineData("--version")]
        [InlineData("version")]
        public void Explicit_version_request_generates_version_info_screen_in_verbs_scenario(string command)
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new[] { command });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines.Should().HaveCount(x => x == 1);
            lines[0].Should().StartWithEquivalent("CommandLine");
            // Teardown
        }

        [Fact]
        public void Errors_of_type_MutuallyExclusiveSetError_are_properly_formatted()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<FakeOptionsWithTwoRequiredAndSets>(new[] { "--weburl=value.com", "--ftpurl=value.org" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Option: 'weburl' is not compatible with: 'ftpurl'.");
            lines[4].ShouldBeEquivalentTo("Option: 'ftpurl' is not compatible with: 'weburl'.");
            lines[5].ShouldBeEquivalentTo("--weburl     Required.");
            lines[6].ShouldBeEquivalentTo("--ftpurl     Required.");
            lines[7].ShouldBeEquivalentTo("-a");
            lines[8].ShouldBeEquivalentTo("--help       Display this help screen.");
            lines[9].ShouldBeEquivalentTo("--version    Display version information.");
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_specific_verb_generates_help_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new[] { "commit", "--help" });
            var result = help.ToString();

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            // Teardown
        }

        [Fact]
        public void Properly_formatted_help_screen_is_displayed_when_usage_is_defined_in_verb_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<AddOptionsWithUsage, CommitOptionsWithUsage, CloneOptionsWithUsage>(
                new[] { "clone", "--badoption=@bad?value" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Option 'badoption' is unknown.");
            lines[4].ShouldBeEquivalentTo("USAGE:");
            lines[5].ShouldBeEquivalentTo("Cloning quietly:");
            lines[6].ShouldBeEquivalentTo("git clone --quiet https://github.com/gsscoder/railwaysharp");
            lines[7].ShouldBeEquivalentTo("Cloning without hard links:");
            lines[8].ShouldBeEquivalentTo("git clone --no-hardlinks https://github.com/gsscoder/csharpx");
            lines[9].ShouldBeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[10].ShouldBeEquivalentTo("filesystem by copying files.");
            lines[11].ShouldBeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[12].ShouldBeEquivalentTo("--help            Display this help screen.");
            lines[13].ShouldBeEquivalentTo("--version         Display version information.");
            lines[14].ShouldBeEquivalentTo("URLS (pos. 0)     A list of url(s) to clone.");

            // Teardown
        }

        [Theory]
        [MemberData("IgnoreUnknownArgumentsData")]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing(
            string[] arguments,
            FakeOptions expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<FakeOptions>(arguments);

            // Verify outcome
            result.Tag.ShouldBeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.ShouldBeEquivalentTo(expected));

            // Teardown
        }

        [Theory]
        [MemberData("IgnoreUnknownArgumentsForVerbsData")]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing_for_verbs(
            string[] arguments,
            CommitOptions expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<AddOptions, CommitOptions, CloneOptions>(arguments);

            // Verify outcome
            result.Tag.ShouldBeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.ShouldBeEquivalentTo(expected));

            // Teardown
        }

        public static IEnumerable<object> IgnoreUnknownArgumentsData
        {
            get
            {
                yield return new object[] { new[] { "--stringvalue=strdata0", "--unknown=valid" }, new FakeOptions { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "1234", "--unknown", "-i", "1", "2", "3" }, new FakeOptions { StringValue = "strdata0", LongValue = 1234L, IntSequence = new[] { 1, 2, 3 } } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "-u" }, new FakeOptions { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
            }
        }

        public static IEnumerable<object> IgnoreUnknownArgumentsForVerbsData
        {
            get
            {
                yield return new object[] { new[] { "commit", "-up" }, new CommitOptions { Patch =  true } };
                yield return new object[] { new[] { "commit", "--amend", "--unknown", "valid" }, new CommitOptions { Amend = true } };
            }
        }
    }
}
