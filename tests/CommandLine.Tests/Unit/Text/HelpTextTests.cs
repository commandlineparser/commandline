// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using CommandLine.Core;
using System.Linq;
using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using System.Text;

namespace CommandLine.Tests.Unit.Text
{
    public class HelpTextTests : IDisposable
    {
        public void Dispose()
        {
            ReflectionHelper.SetAttributeOverride(null);
        }

        [Fact]
        public void Create_empty_instance()
        {
            string.Empty.Should().BeEquivalentTo(new HelpText().ToString());
        }

        [Fact]
        public void Create_instance_without_options()
        {
            // Fixture setup
            // Exercize system 
            var sut =
                new HelpText(new HeadingInfo("Unit-tests", "2.0"), new CopyrightInfo(true, "Author", 2005, 2013))
                    .AddPreOptionsLine("pre-options line 1")
                    .AddPreOptionsLine("pre-options line 2")
                    .AddPostOptionsLine("post-options line 1")
                    .AddPostOptionsLine("post-options line 2");

            // Verify outcome
            var lines = sut.ToString().ToLines();

            lines[0].Should().BeEquivalentTo("Unit-tests 2.0");
            lines[1].Should().BeEquivalentTo("Copyright (C) 2005 - 2013 Author");
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("pre-options line 1");
            lines[4].Should().BeEquivalentTo("pre-options line 2");
            lines[5].Should().BeEquivalentTo("post-options line 1");
            lines[6].Should().BeEquivalentTo("post-options line 2");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_options()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().BeEquivalentTo("pre-options");
            lines[1].Should().BeEquivalentTo("--stringvalue         Define a string value here.");
            lines[2].Should().BeEquivalentTo("-s, --shortandlong    Example with both short and long name.");
            lines[3].Should().BeEquivalentTo("-i                    Define a int sequence here.");
            lines[4].Should().BeEquivalentTo("-x                    Define a boolean or switch value here.");
            lines[5].Should().BeEquivalentTo("--help                Display this help screen.");
            lines[6].Should().BeEquivalentTo("--version             Display version information.");
            lines[7].Should().BeEquivalentTo("value pos. 0          Define a long value here.");
            lines[8].Should().BeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_enum_options_enabled()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true, AddEnumValuesToHelpText = true, MaximumDisplayWidth = 80 }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Options_With_Enum_Having_HelpText>(TypeInfo.Create(typeof(Options_With_Enum_Having_HelpText)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().BeEquivalentTo("pre-options");
            lines[1].Should().BeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].Should().BeEquivalentTo("--shape          Define a enum value here. Valid values: Circle, Square,");
            lines[3].Should().BeEquivalentTo("Triangle");
            lines[4].Should().BeEquivalentTo("--help           Display this help screen.");
            lines[5].Should().BeEquivalentTo("--version        Display version information.");
            lines[6].Should().BeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_enum_options_disabled()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Options_With_Enum_Having_HelpText>(TypeInfo.Create(typeof(Options_With_Enum_Having_HelpText)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().BeEquivalentTo("pre-options");
            lines[1].Should().BeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].Should().BeEquivalentTo("--shape          Define a enum value here.");
            lines[3].Should().BeEquivalentTo("--help           Display this help screen.");
            lines[4].Should().BeEquivalentTo("--version        Display version information.");
            lines[5].Should().BeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void When_defined_MetaValue_should_be_rendered()
        {
            // Fixture setup
            // Exercize system 
            var sut =
                new HelpText("Meta Value.").AddOptions(
                    new NotParsed<Options_With_MetaValue>(TypeInfo.Create(typeof(Options_With_MetaValue)), Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();

            lines[2].Should().BeEquivalentTo("i FILE, input-file=FILE    Required. Specify input FILE to be processed.");
            // Teardown
        }

        [Fact]
        public void When_help_text_is_longer_than_width_it_will_wrap_around_as_if_in_a_column_given_width_of_40()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 40;
            sut.AddOptions(
                new NotParsed<Simple_Options_With_HelpText_Set_To_Long_Description>(
                    TypeInfo.Create(typeof(Simple_Options_With_HelpText_Set_To_Long_Description)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().BeEquivalentTo("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().BeEquivalentTo("                of the verbosity to test");
            lines[4].Should().BeEquivalentTo("                out the wrapping");
            lines[5].Should().BeEquivalentTo("                capabilities of the Help");
            lines[6].Should().BeEquivalentTo("                Text.");
            // Teardown
        }

        [Fact]
        public void When_help_text_is_longer_than_width_it_will_wrap_around_as_if_in_a_column_given_width_of_100()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131")) { MaximumDisplayWidth = 100} ;
            sut.AddOptions(
                new NotParsed<Simple_Options_With_HelpText_Set_To_Long_Description>(
                    TypeInfo.Create(typeof(Simple_Options_With_HelpText_Set_To_Long_Description)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().BeEquivalentTo("  v, verbose    This is the description of the verbosity to test out the wrapping capabilities of"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().BeEquivalentTo("                the Help Text.");
            // Teardown
        }

        [Fact]
        public void When_help_text_has_hidden_option_it_should_not_be_added_to_help_text_output()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 80;
            sut.AddOptions(
                new NotParsed<Simple_Options_With_HelpText_Set_To_Long_Description>(
                    TypeInfo.Create(typeof(Simple_Options_With_HelpText_Set_To_Long_Description)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().BeEquivalentTo("  v, verbose    This is the description of the verbosity to test out the"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().BeEquivalentTo("                wrapping capabilities of the Help Text.");
            // Teardown
        }

        [Fact]
        public void Long_help_text_without_spaces()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 40;
            sut.AddOptions(
                new NotParsed<Simple_Options_With_HelpText_Set_To_Long_Description_Without_Spaces>(
                    TypeInfo.Create(typeof(Simple_Options_With_HelpText_Set_To_Long_Description_Without_Spaces)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines();
            lines[1].Should().BeEquivalentTo("  v, verbose    Before");
            lines[2].Should().BeEquivalentTo("                012345678901234567890123");
            lines[3].Should().BeEquivalentTo("                After");
            lines[4].Should().BeEquivalentTo("  input-file    Before");
            lines[5].Should().BeEquivalentTo("                012345678901234567890123");
            lines[6].Should().BeEquivalentTo("                456789 After");
            // Teardown
        }

        [Fact]
        public void Long_pre_and_post_lines_without_spaces()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText("Heading Info.");
            sut.MaximumDisplayWidth = 40;
            sut.AddPreOptionsLine("Before 0123456789012345678901234567890123456789012 After")
                .AddOptions(new NotParsed<Simple_Options_Without_HelpText>(TypeInfo.Create(typeof(Simple_Options_Without_HelpText)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("Before 0123456789012345678901234567890123456789 After");

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines();
            lines[1].Should().BeEquivalentTo("Before");
            lines[2].Should().BeEquivalentTo("0123456789012345678901234567890123456789");
            lines[3].Should().BeEquivalentTo("012 After");
            lines[lines.Length - 3].Should().BeEquivalentTo("Before");
            lines[lines.Length - 2].Should().BeEquivalentTo("0123456789012345678901234567890123456789");
            lines[lines.Length - 1].Should().BeEquivalentTo("After");

            // Teardown
        }
    
        [Fact]
        public void Invoking_RenderParsingErrorsText_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<object>(
                TypeInfo.Create(typeof(NullInstance)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken"),
                        new MissingValueOptionError(new NameInfo("x", "switch")),
                        new UnknownOptionError("unknown"),
                        new MissingRequiredOptionError(new NameInfo("", "missing")),
                        new SequenceOutOfRangeError(new NameInfo("s", "sequence")),
                        new NoVerbSelectedError(),
                        new BadVerbSelectedError("badverb"),
                        new HelpRequestedError(), // should be ignored
                        new HelpVerbRequestedError(null, null, false) // should be ignored 
                    });
            Func<Error, string> fakeRenderer = err =>
                {
                    switch (err.Tag)
                    {
                        case ErrorType.BadFormatTokenError:
                            return "ERR " + ((BadFormatTokenError)err).Token;
                        case ErrorType.MissingValueOptionError:
                            return "ERR " + ((MissingValueOptionError)err).NameInfo.NameText;
                        case ErrorType.UnknownOptionError:
                            return "ERR " + ((UnknownOptionError)err).Token;
                        case ErrorType.MissingRequiredOptionError:
                            return "ERR " + ((MissingRequiredOptionError)err).NameInfo.NameText;
                        case ErrorType.SequenceOutOfRangeError:
                            return "ERR " + ((SequenceOutOfRangeError)err).NameInfo.NameText;
                        case ErrorType.NoVerbSelectedError:
                            return "ERR no-verb-selected";
                        case ErrorType.BadVerbSelectedError:
                            return "ERR " + ((BadVerbSelectedError)err).Token;
                        default:
                            throw new InvalidOperationException();
                    }
                };
            Func<IEnumerable<MutuallyExclusiveSetError>, string> fakeMutExclRenderer =
                _ => string.Empty;

            // Exercize system
            var errorsText = HelpText.RenderParsingErrorsText(fakeResult, fakeRenderer, fakeMutExclRenderer, 2);

            // Verify outcome
            var lines = errorsText.ToNotEmptyLines();
            lines[0].Should().BeEquivalentTo("  ERR badtoken");
            lines[1].Should().BeEquivalentTo("  ERR x, switch");
            lines[2].Should().BeEquivalentTo("  ERR unknown");
            lines[3].Should().BeEquivalentTo("  ERR missing");
            lines[4].Should().BeEquivalentTo("  ERR s, sequence");
            lines[5].Should().BeEquivalentTo("  ERR no-verb-selected");
            lines[6].Should().BeEquivalentTo("  ERR badverb");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Options_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<Simple_Options>(
                TypeInfo.Create(typeof(Simple_Options)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken"),
                        new SequenceOutOfRangeError(new NameInfo("i", ""))
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("ERROR(S):");
            lines[4].Should().BeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[5].Should().BeEquivalentTo("A sequence option 'i' is defined with fewer or more items than required.");
            lines[6].Should().BeEmpty();
            lines[7].Should().BeEquivalentTo("--stringvalue         Define a string value here.");
            lines[8].Should().BeEmpty();
            lines[9].Should().BeEquivalentTo("-s, --shortandlong    Example with both short and long name.");
            lines[10].Should().BeEmpty();
            lines[11].Should().BeEquivalentTo("-i                    Define a int sequence here.");
            lines[12].Should().BeEmpty();
            lines[13].Should().BeEquivalentTo("-x                    Define a boolean or switch value here.");
            lines[14].Should().BeEmpty();
            lines[15].Should().BeEquivalentTo("--help                Display this help screen.");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_specific_verb_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<object>(
                TypeInfo.Create(typeof(NullInstance)),
                new Error[]
                    {
                        new HelpVerbRequestedError("commit", typeof(Commit_Verb), true)
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToLines().TrimStringArray();

            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("-p, --patch      Use the interactive patch selection interface to chose which");
            lines[4].Should().BeEquivalentTo("changes to commit.");
            lines[5].Should().BeEmpty();
            lines[6].Should().BeEquivalentTo("--amend          Used to amend the tip of the current branch.");
            lines[7].Should().BeEmpty();
            lines[8].Should().BeEquivalentTo("-m, --message    Use the given message as the commit message.");
            lines[9].Should().BeEmpty();
            lines[10].Should().BeEquivalentTo("--help           Display this help screen.");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_specific_verb_returns_appropriate_formatted_text_given_display_width_100()
        {
            // Fixture setup
            var fakeResult = new NotParsed<object>(
                TypeInfo.Create(typeof(NullInstance)),
                new Error[]
                    {
                        new HelpVerbRequestedError("commit", typeof(Commit_Verb), true)
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult, maxDisplayWidth: 100);            

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());	
            lines[2].Should().BeEquivalentTo("-p, --patch      Use the interactive patch selection interface to chose which changes to commit.");
            lines[3].Should().BeEquivalentTo("--amend          Used to amend the tip of the current branch.");
            lines[4].Should().BeEquivalentTo("-m, --message    Use the given message as the commit message.");
            lines[5].Should().BeEquivalentTo("--help           Display this help screen.");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_unknown_verb_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var verbTypes = Enumerable.Empty<Type>().Concat(
                new[] { typeof(Add_Verb), typeof(Commit_Verb), typeof(Clone_Verb) });
            var fakeResult = new NotParsed<object>(
                TypeInfo.Create(typeof(NullInstance),
                    verbTypes),
                new Error[] { new HelpVerbRequestedError(null, null, false) });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());	
            lines[2].Should().BeEquivalentTo("add        Add file contents to the index.");
            lines[3].Should().BeEquivalentTo("commit     Record changes to the repository.");
            lines[4].Should().BeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[5].Should().BeEquivalentTo("help       Display more information on a specific command.");
            lines[6].Should().BeEquivalentTo("version    Display version information.");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_options_and_values()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true, AdditionalNewLineAfterOption = false }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Options_With_HelpText_And_MetaValue>(TypeInfo.Create(typeof(Options_With_HelpText_And_MetaValue)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToLines().TrimStringArray();
            lines[0].Should().BeEmpty();
            lines[1].Should().BeEquivalentTo("pre-options");
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("--stringvalue=STR            Define a string value here.");
            lines[4].Should().BeEquivalentTo("-i INTSEQ                    Define a int sequence here.");
            lines[5].Should().BeEquivalentTo("-x                           Define a boolean or switch value here.");
            lines[6].Should().BeEquivalentTo("--help                       Display this help screen.");
            lines[7].Should().BeEquivalentTo("--version                    Display version information.");
            lines[8].Should().BeEquivalentTo("number (pos. 0) NUM          Define a long value here.");
            lines[9].Should().BeEquivalentTo("paintcolor (pos. 1) COLOR    Define a color value here.");
            lines[10].Should().BeEmpty();
            lines[11].Should().BeEquivalentTo("post-options", lines[11]);
            // Teardown
        }

        [Fact]
        public static void RenderUsageText_returns_properly_formatted_text()
        {
            // Fixture setup
            ParserResult<Options_With_Usage_Attribute> result =
                new NotParsed<Options_With_Usage_Attribute>(
                    TypeInfo.Create(typeof(Options_With_Usage_Attribute)), Enumerable.Empty<Error>());
            
            // Exercize system
            var text = HelpText.RenderUsageText(result);

            // Verify outcome
            var lines = text.ToNotEmptyLines();

            // Teardown
            lines[0].Should().BeEquivalentTo("Normal scenario:");
            lines[1].Should().BeEquivalentTo("  mono testapp.exe --input file.bin --output out.bin");
            lines[2].Should().BeEquivalentTo("Logging warnings:");
            lines[3].Should().BeEquivalentTo("  mono testapp.exe -w --input file.bin");
            lines[4].Should().BeEquivalentTo("Logging errors:");
            lines[5].Should().BeEquivalentTo("  mono testapp.exe -e --input file.bin");
            lines[6].Should().BeEquivalentTo("  mono testapp.exe --errs --input=file.bin");
            lines[7].Should().BeEquivalentTo("List:");
            lines[8].Should().BeEquivalentTo("  mono testapp.exe -l 1,2");
            lines[9].Should().BeEquivalentTo("Value:");
            lines[10].Should().BeEquivalentTo("  mono testapp.exe value");
        }

        [Fact]
        public void Invoke_AutoBuild_for_Options_with_Usage_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<Options_With_Usage_Attribute>(
                TypeInfo.Create(typeof(Options_With_Usage_Attribute)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken")
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var text = helpText.ToString();
            var lines = text.ToLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("ERROR(S):");
            lines[4].Should().BeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[5].Should().BeEmpty();
            lines[6].Should().BeEquivalentTo("USAGE:");
            lines[7].Should().BeEquivalentTo("Normal scenario:");
            lines[8].Should().BeEquivalentTo("mono testapp.exe --input file.bin --output out.bin");
            lines[9].Should().BeEquivalentTo("Logging warnings:");
            lines[10].Should().BeEquivalentTo("mono testapp.exe -w --input file.bin");
            lines[11].Should().BeEquivalentTo("Logging errors:");
            lines[12].Should().BeEquivalentTo("mono testapp.exe -e --input file.bin");
            lines[13].Should().BeEquivalentTo("mono testapp.exe --errs --input=file.bin");
            lines[14].Should().BeEquivalentTo("List:");
            lines[15].Should().BeEquivalentTo("mono testapp.exe -l 1,2");
            lines[16].Should().BeEquivalentTo("Value:");
            lines[17].Should().BeEquivalentTo("mono testapp.exe value");
            lines[18].Should().BeEmpty();
            lines[19].Should().BeEquivalentTo("-i, --input     Set input file.");
            lines[20].Should().BeEmpty();
            lines[21].Should().BeEquivalentTo("-i, --output    Set output file.");
            lines[22].Should().BeEmpty();
            lines[23].Should().BeEquivalentTo("--verbose       Set verbosity level.");
            lines[24].Should().BeEmpty();
            lines[25].Should().BeEquivalentTo("-w, --warns     Log warnings.");
            lines[26].Should().BeEmpty();
            lines[27].Should().BeEquivalentTo("-e, --errs      Log errors.");
            lines[28].Should().BeEmpty();
            lines[29].Should().BeEquivalentTo("-l              List.");
            lines[30].Should().BeEmpty();
            lines[31].Should().BeEquivalentTo("--help          Display this help screen.");
            lines[32].Should().BeEmpty();
            lines[33].Should().BeEquivalentTo("--version       Display version information.");
            lines[34].Should().BeEmpty();
            lines[35].Should().BeEquivalentTo("value pos. 0    Value.");

            // Teardown
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AutoBuild_with_errors_and_preoptions_renders_correctly(bool startWithNewline)
        {
            // Fixture setup
            var fakeResult = new NotParsed<Simple_Options_Without_HelpText>(
                TypeInfo.Create(typeof(Simple_Options_Without_HelpText)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken")
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult,
                h =>
                {
                    h.AddPreOptionsLine((startWithNewline ? Environment.NewLine : null) + "pre-options");
                    return HelpText.DefaultParsingErrorsHandler(fakeResult, h);
                },
                e => e
            );

            // Verify outcome
            var text = helpText.ToString();
            var lines = text.ToLines().TrimStringArray();
            lines[0].Should().Be(HeadingInfo.Default.ToString());
            lines[1].Should().Be(CopyrightInfo.Default.ToString());
            lines[2].Should().BeEmpty();
            lines[3].Should().BeEquivalentTo("pre-options");
            lines[4].Should().BeEmpty();
            lines[5].Should().BeEquivalentTo("ERROR(S):");
            lines[6].Should().BeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[7].Should().BeEmpty();
            lines[8].Should().BeEquivalentTo("-v, --verbose");
            lines[9].Should().BeEmpty();
            lines[10].Should().BeEquivalentTo("--input-file");
        }

        [Fact]
        public void Default_set_to_sequence_should_be_properly_printed()
        {
            // Fixture setup
            var handlers = new CultureInfo("en-US").MakeCultureHandlers();
            var fakeResult =
                new NotParsed<Options_With_Default_Set_To_Sequence>(
                    typeof(Options_With_Default_Set_To_Sequence).ToTypeInfo(),
                    new Error[] { new BadFormatTokenError("badtoken") });

            // Exercize system
            handlers.ChangeCulture();
            var helpText = HelpText.AutoBuild(fakeResult);
            handlers.ResetCulture();

            // Verify outcome
            var text = helpText.ToString();
            var lines = text.ToNotEmptyLines().TrimStringArray();

            lines[4].Should().Be("-z, --strseq    (Default: a b c)");
            lines[5].Should().Be("-y, --intseq    (Default: 1 2 3)");
            lines[6].Should().Be("-q, --dblseq    (Default: 1.1 2.2 3.3)");

            // Teardown
        }

        [Fact]
        public void AutoBuild_when_no_assembly_attributes()
        {
            string expectedCopyright = "Copyright (C) 1 author";

            ReflectionHelper.SetAttributeOverride(new Attribute[0]);

            ParserResult<Simple_Options> fakeResult = new NotParsed<Simple_Options>(
                TypeInfo.Create(typeof (Simple_Options)), new Error[0]);
            bool onErrorCalled = false;
            HelpText actualResult = HelpText.AutoBuild(fakeResult, ht => 
            {
                onErrorCalled = true;
                return ht;
            }, ex => ex);
                
            onErrorCalled.Should().BeTrue();
            actualResult.Copyright.Should().Be(expectedCopyright);
        }

        [Fact]
        public void AutoBuild_with_assembly_title_and_version_attributes_only()
        {
            string expectedTitle = "Title";
            string expectedVersion = "1.2.3.4";

            ReflectionHelper.SetAttributeOverride(new Attribute[]
            {
                new AssemblyTitleAttribute(expectedTitle),
                new AssemblyInformationalVersionAttribute(expectedVersion)
            });

            ParserResult<Simple_Options> fakeResult = new NotParsed<Simple_Options>(
                TypeInfo.Create(typeof (Simple_Options)), new Error[0]);
            bool onErrorCalled = false;
            HelpText actualResult = HelpText.AutoBuild(fakeResult, ht =>
            {
                onErrorCalled = true;
                return ht;
            }, ex => ex);

            onErrorCalled.Should().BeTrue();
            actualResult.Heading.Should().Be(string.Format("{0} {1}", expectedTitle, expectedVersion));
        }

        [Fact]
        public void AutoBuild_with_assembly_company_attribute_only()
        {
            string expectedCompany = "Company";

            ReflectionHelper.SetAttributeOverride(new Attribute[]
            {
                new AssemblyCompanyAttribute(expectedCompany)
            });

            ParserResult<Simple_Options> fakeResult = new NotParsed<Simple_Options>(
                TypeInfo.Create(typeof (Simple_Options)), new Error[0]);
            bool onErrorCalled = false;
            HelpText actualResult = HelpText.AutoBuild(fakeResult, ht =>
            {
                onErrorCalled = true;
                return ht;
            }, ex => ex);

            onErrorCalled.Should().BeFalse(); // Other attributes have fallback logic
            actualResult.Copyright.Should().Be(string.Format("Copyright (C) {0} {1}", DateTime.Now.Year, expectedCompany));
        }

        [Fact]
        public void Add_line_with_two_empty_spaces_at_the_end()
        {
            StringBuilder b = new StringBuilder();
            HelpText.AddLine(b,
                "Test  ",
                1);

            Assert.Equal("T" + Environment.NewLine + "e" + Environment.NewLine + "s" + Environment.NewLine + "t", b.ToString());
        }

        [Fact]
        public void HelpTextHonoursLineBreaks()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText {AddDashesToOption = true}
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(HelpTextWithLineBreaks_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines[0].Should().BeEquivalentTo("  --stringvalue    This is a help text description.");
            lines[1].Should().BeEquivalentTo("                   It has multiple lines.");
            lines[2].Should().BeEquivalentTo("                   We also want to ensure that indentation is correct.");
         
            // Teardown
        }

        [Fact]
        public void HelpTextHonoursIndentationAfterLineBreaks()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText {AddDashesToOption = true}
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(HelpTextWithLineBreaks_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines[3].Should().BeEquivalentTo("  --stringvalu2    This is a help text description where we want");
            lines[4].Should().BeEquivalentTo("                      the left pad after a linebreak to be honoured so that");
            lines[5].Should().BeEquivalentTo("                      we can sub-indent within a description.");
         
            // Teardown
        }

        [Fact]
        public void HelpTextPreservesIndentationAcrossWordWrap()
        {
            // Fixture setup
            // Exercise system 
            var sut = new HelpText {AddDashesToOption = true,MaximumDisplayWidth = 60}
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(HelpTextWithLineBreaksAndSubIndentation_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines[0].Should().BeEquivalentTo("  --stringvalue    This is a help text description where we");
            lines[1].Should().BeEquivalentTo("                   want:");
            lines[2].Should().BeEquivalentTo("                       * The left pad after a linebreak to");
            lines[3].Should().BeEquivalentTo("                       be honoured and the indentation to be");
            lines[4].Should().BeEquivalentTo("                       preserved across to the next line");
            lines[5].Should().BeEquivalentTo("                       * The ability to return to no indent.");
            lines[6].Should().BeEquivalentTo("                   Like this.");

            // Teardown
        }

        [Fact]
        public void HelpTextIsConsitentRegardlessOfCompileTimeLineStyle()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText {AddDashesToOption = true}
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(HelpTextWithMixedLineBreaks_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines();
            lines[0].Should().BeEquivalentTo("  --stringvalue    This is a help text description");
            lines[1].Should().BeEquivalentTo("                     It has multiple lines.");
            lines[2].Should().BeEquivalentTo("                     Third line");
         
            // Teardown
        }

        [Fact]
        public void HelpTextPreservesIndentationAcrossWordWrapWithSmallMaximumDisplayWidth()
        {
            // Fixture setup
            // Exercise system 
            var sut = new HelpText {AddDashesToOption = true,MaximumDisplayWidth = 10} 
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(HelpTextWithLineBreaksAndSubIndentation_Options)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
          
            Assert.True(sut.ToString().Length>0);
			
            // Teardown
        }

        [Fact]
        public void Options_should_be_separated_by_spaces()
        {
            // Fixture setup
            var handlers = new CultureInfo("en-US").MakeCultureHandlers();
            var fakeResult =
                new NotParsed<Options_With_Default_Set_To_Sequence>(
                    typeof(Options_With_Default_Set_To_Sequence).ToTypeInfo(),
                    Enumerable.Empty<Error>()
                    );

            // Exercize system
            handlers.ChangeCulture();
            var helpText = HelpText.AutoBuild(fakeResult);
            handlers.ResetCulture();

            // Verify outcome
            var text = helpText.ToString();
            var lines = text.ToLines().TrimStringArray();
            Console.WriteLine(text);
            lines[3].Should().Be("-z, --strseq    (Default: a b c)");
            lines[5].Should().Be("-y, --intseq    (Default: 1 2 3)");
            lines[7].Should().Be("-q, --dblseq    (Default: 1.1 2.2 3.3)");

            // Teardown
        }
    }
}
