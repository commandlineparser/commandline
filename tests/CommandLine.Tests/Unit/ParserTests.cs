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
            sut.ParseArguments<Options_With_Required_Set_To_True>(new string[] { });

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
            sut.ParseArguments(new string[] { }, typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb));

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
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new string[] { });

            // Verify outcome
            var text = writer.ToString();
            text.Should().NotBeEmpty();
            // Teardown
        }

        [Fact]
        public void Parse_options()
        {
            // Fixture setup
            var expectedOptions = new Simple_Options { StringValue = "strvalue", IntSequence = new[] { 1, 2, 3 } };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Simple_Options>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            ((Parsed<Simple_Options>)result).Value.ShouldBeEquivalentTo(expectedOptions);
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
            var expectedOptions = new Options_With_Switches { OutputFile = outputFile };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Options_With_Switches>(args);

            // Verify outcome
            ((Parsed<Options_With_Switches>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_repeated_options_with_default_parser()
        {
            // Fixture setup
            var sut = Parser.Default;

            // Exercize system
            var result = sut.ParseArguments<Options_With_Switches>(new[] { "-i", "-i", "-o", "file" });

            // Verify outcome
            Assert.IsType<NotParsed<Options_With_Switches>>(result);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_double_dash()
        {
            // Fixture setup
            var expectedOptions = new Simple_Options_With_Values
                                  {
                                      StringValue = "astring",
                                      LongValue = 20L,
                                      StringSequence = new[] { "--aaa", "-b", "--ccc" },
                                      IntValue = 30
                                  };
            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result =
                sut.ParseArguments<Simple_Options_With_Values>(
                    new[] { "--stringvalue", "astring", "--", "20", "--aaa", "-b", "--ccc", "30" });

            // Verify outcome
            ((Parsed<Simple_Options_With_Values>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_double_dash_in_verbs_scenario()
        {
            // Fixture setup
            var expectedOptions = new Add_Verb { Patch = true, FileName = "--strange-fn" };
            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result = sut.ParseArguments(
                new[] { "add", "-p", "--", "--strange-fn" },
                typeof(Add_Verb),
                typeof(Commit_Verb),
                typeof(Clone_Verb));

            // Verify outcome
            Assert.IsType<Add_Verb>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_verbs()
        {
            // Fixture setup
            var expectedOptions = new Clone_Verb
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
                    typeof(Add_Verb),
                    typeof(Commit_Verb),
                    typeof(Clone_Verb));

            // Verify outcome
            Assert.IsType<Clone_Verb>(((Parsed<object>)result).Value);
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
            var expectedOptions = new Commit_Verb() { Message = message };
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments(
                args,
                typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb));

            // Verify outcome
            Assert.IsType<Commit_Verb>(((Parsed<object>)result).Value);
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
                typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb));

            // Verify outcome
            Assert.IsType<NotParsed<object>>(result);
            // Teardown
        }

        [Fact]
        public void Parse_verbs_using_generic_overload()
        {
            // Fixture setup
            var expectedOptions = new Clone_Verb
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
                sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                    new[] { "clone", "-q", "http://gsscoder.github.com/", "http://yes-to-nooo.github.com/" });

            // Verify outcome
            Assert.IsType<Clone_Verb>(((Parsed<object>)result).Value);
            ((Parsed<object>)result).Value.ShouldBeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_to_immutable_instance()
        {
            // Fixture setup
            var expectedOptions = new Immutable_Simple_Options("strvalue", new[] { 1, 2, 3 }, default(bool), default(long));
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Immutable_Simple_Options>(new[] { "--stringvalue=strvalue", "-i1", "2", "3" });

            // Verify outcome
            ((Parsed<Immutable_Simple_Options>)result).Value.ShouldBeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_immutable_instance_generates_help_requested_error()
        {
            // Fixture setup
            var expectedError = new HelpRequestedError();
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Immutable_Simple_Options>(new[] { "--help" });

            // Verify outcome
            ((NotParsed<Immutable_Simple_Options>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<Immutable_Simple_Options>)result).Errors.Should().ContainSingle(e => e.Equals(expectedError));
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_immutable_instance_generates_help_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Immutable_Simple_Options>(new[] { "--help" });
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
            var result = sut.ParseArguments<Simple_Options>(new[] { "--version" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<Simple_Options>)result).Errors.Should().ContainSingle(e => e.Equals(expectedError));
            // Teardown
        }

        [Fact]
        public void Explicit_version_request_generates_version_info_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Simple_Options>(new[] { "--version" });
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
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new string[] { });
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
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new[] { "--help" });
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
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new[] { command });
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
            sut.ParseArguments<Options_With_Two_Option_Required_Set_To_True_And_Two_Sets>(new[] { "--weburl=value.com", "--ftpurl=value.org" });
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
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new[] { "commit", "--help" });
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
            sut.ParseArguments<Add_Verb_With_Usage_Attribute, Commit_Verb_With_Usage_Attribute, Clone_Verb_With_Usage_Attribute>(
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

        [Fact]
        public void Specific_verb_help_screen_should_be_displayed_regardless_other_argument()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "help", "clone", "extra-arg" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[3].ShouldBeEquivalentTo("filesystem by copying files.");
            lines[4].ShouldBeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[5].ShouldBeEquivalentTo("--help            Display this help screen.");
            lines[6].ShouldBeEquivalentTo("--version         Display version information.");
            lines[7].ShouldBeEquivalentTo("value pos. 0");

            // Teardown
        }

        [Theory]
        [MemberData("IgnoreUnknownArgumentsData")]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing(
            string[] arguments,
            Simple_Options expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<Simple_Options>(arguments);

            // Verify outcome
            result.Tag.ShouldBeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.ShouldBeEquivalentTo(expected));

            // Teardown
        }

        [Theory]
        [MemberData("IgnoreUnknownArgumentsForVerbsData")]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing_for_verbs(
            string[] arguments,
            Commit_Verb expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(arguments);

            // Verify outcome
            result.Tag.ShouldBeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.ShouldBeEquivalentTo(expected));

            // Teardown
        }

        [Fact]
        public void Properly_formatted_help_screen_excludes_help_as_unknown_option()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "clone", "--bad-arg", "--help" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Option 'bad-arg' is unknown.");
            lines[4].ShouldBeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[5].ShouldBeEquivalentTo("filesystem by copying files.");
            lines[6].ShouldBeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[7].ShouldBeEquivalentTo("--help            Display this help screen.");
            lines[8].ShouldBeEquivalentTo("--version         Display version information.");
            lines[9].ShouldBeEquivalentTo("value pos. 0");

            // Teardown
        }

        public static IEnumerable<object> IgnoreUnknownArgumentsData
        {
            get
            {
                yield return new object[] { new[] { "--stringvalue=strdata0", "--unknown=valid" }, new Simple_Options { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "1234", "--unknown", "-i", "1", "2", "3" }, new Simple_Options { StringValue = "strdata0", LongValue = 1234L, IntSequence = new[] { 1, 2, 3 } } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "-u" }, new Simple_Options { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
            }
        }

        public static IEnumerable<object> IgnoreUnknownArgumentsForVerbsData
        {
            get
            {
                yield return new object[] { new[] { "commit", "-up" }, new Commit_Verb { Patch =  true } };
                yield return new object[] { new[] { "commit", "--amend", "--unknown", "valid" }, new Commit_Verb { Amend = true } };
            }
        }
    }
}
