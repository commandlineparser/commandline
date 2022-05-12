// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;
using CommandLine.Text;
using CommandLine.Tests.Fakes;

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
            ((Parsed<Simple_Options>)result).Value.Should().BeEquivalentTo(expectedOptions);
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
            ((Parsed<Options_With_Switches>)result).Value.Should().BeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Theory]
        [InlineData(new string[0], 0, 0)]
        [InlineData(new[] { "-v" }, 1, 0)]
        [InlineData(new[] { "-vv" }, 2, 0)]
        [InlineData(new[] { "-v", "-v" }, 2, 0)]
        [InlineData(new[] { "-v", "-v", "-v" }, 3, 0)]
        [InlineData(new[] { "-v", "-vv" }, 3, 0)]
        [InlineData(new[] { "-vv", "-v" }, 3, 0)]
        [InlineData(new[] { "-vvv" }, 3, 0)]
        [InlineData(new[] { "-v", "-s", "-v", "-v" }, 3, 1)]
        [InlineData(new[] { "-v", "-ss", "-v", "-v" }, 3, 2)]
        [InlineData(new[] { "-v", "-s", "-sv", "-v" }, 3, 2)]
        [InlineData(new[] { "-vsvv" }, 3, 1)]
        [InlineData(new[] { "-vssvv" }, 3, 2)]
        [InlineData(new[] { "-vsvsv" }, 3, 2)]
        public void Parse_FlagCounter_options_with_short_name(string[] args, int verboseCount, int silentCount)
        {
            // Fixture setup
            var expectedOptions = new Options_With_FlagCounter_Switches { Verbose = verboseCount, Silent = silentCount };
            var sut = new Parser(with => with.AllowMultiInstance = true);

            // Exercize system
            var result = sut.ParseArguments<Options_With_FlagCounter_Switches>(args);

            // Verify outcome
            // ((NotParsed<Options_With_FlagCounter_Switches>)result).Errors.Should().BeEmpty();
            ((Parsed<Options_With_FlagCounter_Switches>)result).Value.Should().BeEquivalentTo(expectedOptions);
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
            // NOTE: Once GetoptMode becomes the default, it will imply MultiInstance and the above check will fail because it will be Parsed.
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
            ((Parsed<Simple_Options_With_Values>)result).Value.Should().BeEquivalentTo(expectedOptions);
            // Teardown
        }

        [Fact]
        public void Parse_options_with_repeated_value_in_values_sequence_and_option()
        {
            var text = "x1 x2 x3 -c x1"; // x1 is the same in -c option and first value
            var args = text.Split();
            var parser = new Parser(with =>
            {
                with.HelpWriter = Console.Out;
            });
            var result = parser.ParseArguments<Options_With_Value_Sequence_And_Normal_Option>(args);
			var options= (result as Parsed<Options_With_Value_Sequence_And_Normal_Option>).Value;
            options.Compress.Should().BeEquivalentTo(new[] { "x1" });
            options.InputDirs.Should().BeEquivalentTo(new[] { "x1","x2","x3" });
        }

        [Fact]
        public void Parse_options_with_double_dash_and_option_sequence()
        {
            var expectedOptions = new Options_With_Option_Sequence_And_Value_Sequence
            {
                OptionSequence = new[] { "option1", "option2", "option3" },
                ValueSequence = new[] { "value1", "value2", "value3" }
            };

            var sut = new Parser(with => with.EnableDashDash = true);

            // Exercize system
            var result =
                sut.ParseArguments<Options_With_Option_Sequence_And_Value_Sequence>(
                    new[] { "--option-seq", "option1", "option2", "option3", "--", "value1", "value2", "value3" });

            // Verify outcome
            ((Parsed<Options_With_Option_Sequence_And_Value_Sequence>)result).Value.Should().BeEquivalentTo(expectedOptions);
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
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_options_with_single_dash()
        {
            // Fixture setup
            var args = new[] {"-"};
            var expectedOptions = new Options_With_Switches();
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Options_With_Switches>(args);

            // Verify outcome
            ((Parsed<Options_With_Switches>)result).Value.Should().BeEquivalentTo(expectedOptions);
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
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
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
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
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
            // NOTE: Once GetoptMode becomes the default, it will imply MultiInstance and the above check will fail because it will be Parsed.
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
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
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
            ((Parsed<Immutable_Simple_Options>)result).Value.Should().BeEquivalentTo(expectedOptions);
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
            lines[0].Should().Be(HeadingInfo.Default.ToString());
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
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("No verb selected.");
            lines[4].Should().BeEquivalentTo("add        Add file contents to the index.");
            lines[5].Should().BeEquivalentTo("commit     Record changes to the repository.");
            lines[6].Should().BeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[7].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[8].Should().BeEquivalentTo("version    Display version information.");
            // Teardown
        }
       
        [Fact]
        public void Help_screen_in_default_verb_scenario()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercise system
            sut.ParseArguments<Add_Verb_As_Default, Commit_Verb, Clone_Verb>(new string[] {"--help" });
            var result = help.ToString();
         
            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("add        (Default Verb) Add file contents to the index.");
            lines[3].Should().BeEquivalentTo("commit     Record changes to the repository.");
            lines[4].Should().BeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[5].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[6].Should().BeEquivalentTo("version    Display version information.");
            
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
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("add        Add file contents to the index.");
            lines[3].Should().BeEquivalentTo("commit     Record changes to the repository.");
            lines[4].Should().BeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[5].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[6].Should().BeEquivalentTo("version    Display version information.");
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
            lines[0].Should().Be(HeadingInfo.Default.ToString());
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
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("Option: 'weburl' is not compatible with: 'ftpurl'.");
            lines[4].Should().BeEquivalentTo("Option: 'ftpurl' is not compatible with: 'weburl'.");
            lines[5].Should().BeEquivalentTo("--weburl     Required.");
            lines[6].Should().BeEquivalentTo("--ftpurl     Required.");
            lines[7].Should().BeEquivalentTo("-a");
            lines[8].Should().BeEquivalentTo("--help       Display this help screen.");
            lines[9].Should().BeEquivalentTo("--version    Display version information.");
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
            var sut = new Parser(config =>
            {
                config.HelpWriter = help;
                config.MaximumDisplayWidth = 80;
            });

            // Exercize system
            sut.ParseArguments<Add_Verb_With_Usage_Attribute, Commit_Verb_With_Usage_Attribute, Clone_Verb_With_Usage_Attribute>(
                new[] { "clone", "--badoption=@bad?value" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("Option 'badoption' is unknown.");
            lines[4].Should().BeEquivalentTo("USAGE:");
            lines[5].Should().BeEquivalentTo("Basic cloning:");
            lines[6].Should().BeEquivalentTo("git clone https://github.com/gsscoder/csharpx");
            lines[7].Should().BeEquivalentTo("Cloning quietly:");
            lines[8].Should().BeEquivalentTo("git clone --quiet https://github.com/gsscoder/railwaysharp");
            lines[9].Should().BeEquivalentTo("Cloning without hard links:");
            lines[10].Should().BeEquivalentTo("git clone --no-hardlinks https://github.com/gsscoder/csharpx");
            lines[11].Should().BeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[12].Should().BeEquivalentTo("filesystem by copying files.");
            lines[13].Should().BeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[14].Should().BeEquivalentTo("--help            Display this help screen.");
            lines[15].Should().BeEquivalentTo("--version         Display version information.");
            lines[16].Should().BeEquivalentTo("URLS (pos. 0)     A list of url(s) to clone.");

            // Teardown
        }

         [Fact]
        public void Properly_formatted_help_screen_is_displayed_when_there_is_a_hidden_verb()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Secert_Verb, Add_Verb_With_Usage_Attribute>(new string[] { });
            var result = help.ToString();
            
            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("No verb selected.");
            lines[4].Should().BeEquivalentTo("add        Add file contents to the index.");
            lines[5].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[6].Should().BeEquivalentTo("version    Display version information.");

            // Teardown
        }

        [Fact]
        public void Properly_formatted_help_screen_is_displayed_when_there_is_a_hidden_verb_selected_usage_displays_with_hidden_option()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Secert_Verb, Add_Verb_With_Usage_Attribute>(new string[] { "secert", "--help" });
            var result = help.ToString();
            
            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("-f, --force    Allow adding otherwise ignored files.");
            lines[3].Should().BeEquivalentTo("--help         Display this help screen.");
            lines[4].Should().BeEquivalentTo("--version      Display version information.");

            // Teardown
        }
        
        [Fact]
        public void Parse_options_when_given_hidden_verb()
        {
            // Fixture setup
            var expectedOptions = new Secert_Verb { Force = true, SecertOption = null};
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            var result = sut.ParseArguments<Secert_Verb, Add_Verb_With_Usage_Attribute>(new string[] { "secert", "--force" });
            

            // Verify outcome
            result.Tag.Should().BeEquivalentTo(ParserResultType.Parsed);
            result.GetType().Should().Be<Parsed<object>>();
            result.TypeInfo.Current.Should().Be<Secert_Verb>();
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Parse_options_when_given_hidden_verb_with_hidden_option()
        {
            // Fixture setup
            var expectedOptions = new Secert_Verb { Force = true, SecertOption = "shhh" };
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            var result = sut.ParseArguments<Secert_Verb, Add_Verb_With_Usage_Attribute>(new string[] { "secert", "--force", "--secert-option", "shhh" });
            
            // Verify outcome
            result.Tag.Should().BeEquivalentTo(ParserResultType.Parsed);
            result.GetType().Should().Be<Parsed<object>>();
            result.TypeInfo.Current.Should().Be<Secert_Verb>();
            ((Parsed<object>)result).Value.Should().BeEquivalentTo(expectedOptions, o => o.RespectingRuntimeTypes());
            // Teardown
        }

        [Fact]
        public void Specific_verb_help_screen_should_be_displayed_regardless_other_argument()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config =>
            {
                config.HelpWriter = help;
                config.MaximumDisplayWidth = 80;
            });

            // Exercize system
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "help", "clone", "extra-arg" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[3].Should().BeEquivalentTo("filesystem by copying files.");
            lines[4].Should().BeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[5].Should().BeEquivalentTo("--help            Display this help screen.");
            lines[6].Should().BeEquivalentTo("--version         Display version information.");
            lines[7].Should().BeEquivalentTo("value pos. 0");

            // Teardown
        }

        [Theory]
        [MemberData(nameof(IgnoreUnknownArgumentsData))]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing(
            string[] arguments,
            Simple_Options expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<Simple_Options>(arguments);

            // Verify outcome
            result.Tag.Should().BeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.Should().BeEquivalentTo(expected));

            // Teardown
        }

        [Theory]
        [MemberData(nameof(IgnoreUnknownArgumentsForVerbsData))]
        public void When_IgnoreUnknownArguments_is_set_valid_unknown_arguments_avoid_a_failure_parsing_for_verbs(
            string[] arguments,
            Commit_Verb expected)
        {
            // Fixture setup
            var sut = new Parser(config => config.IgnoreUnknownArguments = true);

            // Exercize system
            var result = sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(arguments);

            // Verify outcome
            result.Tag.Should().BeEquivalentTo(ParserResultType.Parsed);
            result.WithParsed(opts => opts.Should().BeEquivalentTo(expected));

            // Teardown
        }

        [Fact]
        public void Properly_formatted_help_screen_excludes_help_as_unknown_option()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config =>
            {
                config.HelpWriter = help;
                config.MaximumDisplayWidth = 80;
            });

            // Exercize system
            sut.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "clone", "--bad-arg", "--help" });
            var result = help.ToString();

            // Verify outcome
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("Option 'bad-arg' is unknown.");
            lines[4].Should().BeEquivalentTo("--no-hardlinks    Optimize the cloning process from a repository on a local");
            lines[5].Should().BeEquivalentTo("filesystem by copying files.");
            lines[6].Should().BeEquivalentTo("-q, --quiet       Suppress summary message.");
            lines[7].Should().BeEquivalentTo("--help            Display this help screen.");
            lines[8].Should().BeEquivalentTo("--version         Display version information.");
            lines[9].Should().BeEquivalentTo("value pos. 0");

            // Teardown
        }

        [Fact]
        public static void Breaking_mutually_exclusive_set_constraint_with_both_set_name_with_gererates_Error()
        {
            // Fixture setup
            var expectedResult = new[]
            {
                new MutuallyExclusiveSetError(new NameInfo("", "weburl"), "theweb"),
                new MutuallyExclusiveSetError(new NameInfo("", "somethingelse"), "theweb"),

            };
            var sut = new Parser();

            // Exercize system 
            var result = sut.ParseArguments<Options_With_SetName_That_Ends_With_Previous_SetName>(
                new[] { "--weburl", "value", "--somethingelse", "othervalue" });

            // Verify outcome
            ((NotParsed<Options_With_SetName_That_Ends_With_Previous_SetName>)result).Errors.Should().BeEquivalentTo(expectedResult);
           
        }

        [Fact]
        public static void Arguments_with_the_same_values_when_unknown_arguments_are_ignored()
        {
            var sameValues = new[] { "--stringvalue=test", "--shortandlong=test" };
            var sut = new Parser(parserSettings => { parserSettings.IgnoreUnknownArguments = true; });
            var result = sut.ParseArguments<Simple_Options>(sameValues);

            result.MapResult(_ => true, _ => false).Should().BeTrue();
        }

        [Fact]
        public static void Arguments_with_the_different_values_when_unknown_arguments_are_ignored()
        {
            var sameValues = new[] { "--stringvalue=test1", "--shortandlong=test2" };
            var sut = new Parser(parserSettings => { parserSettings.IgnoreUnknownArguments = true; });
            var result = sut.ParseArguments<Simple_Options>(sameValues);

            result.MapResult(_ => true, _ => false).Should().BeTrue();
        }

        public static IEnumerable<object[]> IgnoreUnknownArgumentsData
        {
            get
            {
                yield return new object[] { new[] { "--stringvalue=strdata0", "--unknown=valid" }, new Simple_Options { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "1234", "--unknown", "-i", "1", "2", "3" }, new Simple_Options { StringValue = "strdata0", LongValue = 1234L, IntSequence = new[] { 1, 2, 3 } } };
                yield return new object[] { new[] { "--stringvalue=strdata0", "-u" }, new Simple_Options { StringValue = "strdata0", IntSequence = Enumerable.Empty<int>() } };
            }
        }

        public static IEnumerable<object[]> IgnoreUnknownArgumentsForVerbsData
        {
            get
            {
                yield return new object[] { new[] { "commit", "-up" }, new Commit_Verb { Patch =  true } };
                yield return new object[] { new[] { "commit", "--amend", "--unknown", "valid" }, new Commit_Verb { Amend = true } };
            }
        }

        [Fact]
        public static void Null_default()
        {
            Parser parser = new Parser();
            parser.ParseArguments<NullDefaultCommandLineArguments>("".Split())
                .WithParsed(r =>
                {
                    Assert.Null(r.User);
                });
        }

        public class NullDefaultCommandLineArguments
        {
            [Option('u', "user", Default = null)]
            public string User { get; set; }
        }

        [Fact]
        public void Parse_options_with_same_option_and_value_args()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Options_With_Option_And_Value_Of_String_Type>(
                new[] { "arg", "-o", "arg" })
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed(args =>
                {
                    Assert.Equal("arg", args.OptValue);
                    Assert.Equal("arg", args.PosValue);
                });
        }

        [Fact]
        public void Parse_verb_with_same_option_and_value_args()
        {
            var parser = Parser.Default;
            var result = parser.ParseArguments(
                new[] { "test", "arg", "-o", "arg" }, 
                typeof(Verb_With_Option_And_Value_Of_String_Type));
            result
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed<Verb_With_Option_And_Value_Of_String_Type>(args =>
                {
                    Assert.Equal("arg", args.OptValue);
                    Assert.Equal("arg", args.PosValue);
                });
        }

        [Fact]
        public void Parse_options_with_shuffled_index_values()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Options_With_Shuffled_Index_Values>(
                new[] { "zero", "one", "two" })
                .WithNotParsed(errors => { throw new InvalidOperationException("Must be parsed."); })
                .WithParsed(args =>
                {
                    Assert.Equal("zero", args.Arg0);
                    Assert.Equal("one", args.Arg1);
                    Assert.Equal("two", args.Arg2);
                });

        }


        [Fact]
        public void Blank_lines_are_inserted_between_verbs()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => config.HelpWriter = help);

            // Exercize system
            sut.ParseArguments<Secert_Verb, Add_Verb_With_Usage_Attribute>(new string[] { });
            var result = help.ToString();
            
            // Verify outcome
            var lines = result.ToLines().TrimStringArray();
            lines[6].Should().BeEquivalentTo("add        Add file contents to the index.");
            lines[8].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[10].Should().BeEquivalentTo("version    Display version information.");
            // Teardown
        }


        [Fact]
        public void Parse_default_verb_implicit()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Default_Verb_One>(new[] { "-t" })
                .WithNotParsed(errors => throw new InvalidOperationException("Must be parsed."))
                .WithParsed(args =>
                {
                    Assert.True(args.TestValueOne);
                });
        }

        [Fact]
        public void Parse_default_verb_explicit()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Default_Verb_One>(new[] { "default1", "-t" })
                .WithNotParsed(errors => throw new InvalidOperationException("Must be parsed."))
                .WithParsed(args =>
                {
                    Assert.True(args.TestValueOne);
                });
        }

        [Fact]
        public void Parse_multiple_default_verbs()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Default_Verb_One, Default_Verb_Two>(new string[] { })
                .WithNotParsed(errors => Assert.IsType<MultipleDefaultVerbsError>(errors.First()))
                .WithParsed(args => throw new InvalidOperationException("Should not be parsed."));
        }

        [Fact]
        public void Parse_repeated_options_in_verbs_scenario_with_multi_instance()
        {
            using (var sut = new Parser(settings => settings.AllowMultiInstance = true))
            {
                var longVal1 = 100;
                var longVal2 = 200;
                var longVal3 = 300;
                var stringVal = "shortSeq1";

                var result = sut.ParseArguments(
                    new[] { "sequence", "--long-seq", $"{longVal1}", "-s", stringVal, "--long-seq", $"{longVal2};{longVal3}" },
                    typeof(Add_Verb), typeof(Commit_Verb), typeof(SequenceOptions));

                Assert.IsType<Parsed<object>>(result);
                Assert.IsType<SequenceOptions>(((Parsed<object>)result).Value);
                result.WithParsed<SequenceOptions>(verb =>
                {
                    Assert.Equal(new long[] { longVal1, longVal2, longVal3 }, verb.LongSequence);
                    Assert.Equal(new[] { stringVal }, verb.StringSequence);
                });
            }
        }

        [Fact]
        public void Parse_repeated_options_in_verbs_scenario_without_multi_instance()
        {
            // NOTE: Once GetoptMode becomes the default, it will imply MultiInstance and this test will fail because the parser result will be Parsed.
            using (var sut = new Parser(settings => settings.AllowMultiInstance = false))
            {
                var longVal1 = 100;
                var longVal2 = 200;
                var longVal3 = 300;
                var stringVal = "shortSeq1";

                var result = sut.ParseArguments(
                    new[] { "sequence", "--long-seq", $"{longVal1}", "-s", stringVal, "--long-seq", $"{longVal2};{longVal3}" },
                    typeof(Add_Verb), typeof(Commit_Verb), typeof(SequenceOptions));

                Assert.IsType<NotParsed<object>>(result);
                result.WithNotParsed(errors => Assert.All(errors, e =>
                {
                    if (e is RepeatedOptionError)
                    {
                        // expected
                    }
                    else
                    {
                        throw new Exception($"{nameof(RepeatedOptionError)} expected");
                    }
                }));
            }
        }

        [Fact]
        public void Parse_default_verb_with_empty_name()
        {
            var parser = Parser.Default;
            parser.ParseArguments<Default_Verb_With_Empty_Name>(new[] { "-t" })
                .WithNotParsed(errors => throw new InvalidOperationException("Must be parsed."))
                .WithParsed(args =>
                {
                    Assert.True(args.TestValue);
                });
        }
        //Fix Issue #409 for WPF
        [Fact]
        public void When_HelpWriter_is_null_it_should_not_fire_exception()
        {
            // Arrange
            
            //Act
            var sut = new Parser(config => config.HelpWriter = null);
            sut.ParseArguments<Simple_Options>(new[] {"--dummy"});
            //Assert
            sut.Settings.MaximumDisplayWidth.Should().BeGreaterThan(1);
        }
    }
}
