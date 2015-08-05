// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

using CommandLine.Core;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Text
{
    public class HelpTextTests
    {
        [Fact]
        public void Create_empty_instance()
        {
            string.Empty.ShouldBeEquivalentTo(new HelpText().ToString());
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
            var lines = sut.ToString().ToNotEmptyLines();

            lines[0].ShouldBeEquivalentTo("Unit-tests 2.0");
            lines[1].ShouldBeEquivalentTo("Copyright (C) 2005 - 2013 Author");
            lines[2].ShouldBeEquivalentTo("pre-options line 1");
            lines[3].ShouldBeEquivalentTo("pre-options line 2");
            lines[4].ShouldBeEquivalentTo("post-options line 1");
            lines[5].ShouldBeEquivalentTo("post-options line 2");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_options()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<FakeOptions>(TypeInfo.Create(typeof(FakeOptions)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("pre-options");
            lines[1].ShouldBeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].ShouldBeEquivalentTo("-i               Define a int sequence here.");
            lines[3].ShouldBeEquivalentTo("-x               Define a boolean or switch value here.");
            lines[4].ShouldBeEquivalentTo("--help           Display this help screen.");
            lines[5].ShouldBeEquivalentTo("--version        Display version information.");
            lines[6].ShouldBeEquivalentTo("value pos. 0     Define a long value here.");
            lines[7].ShouldBeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_enum_options_enabled()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true, AddEnumValuesToHelpText = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<FakeOptionsWithHelpTextEnum>(TypeInfo.Create(typeof(FakeOptionsWithHelpTextEnum)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("pre-options");
            lines[1].ShouldBeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].ShouldBeEquivalentTo("--shape          Define a enum value here. Valid values: Circle, Square,");
            lines[3].ShouldBeEquivalentTo("Triangle");
            lines[4].ShouldBeEquivalentTo("--help           Display this help screen.");
            lines[5].ShouldBeEquivalentTo("--version        Display version information.");
            lines[6].ShouldBeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_enum_options_disabled()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<FakeOptionsWithHelpTextEnum>(TypeInfo.Create(typeof(FakeOptionsWithHelpTextEnum)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("pre-options");
            lines[1].ShouldBeEquivalentTo("--stringvalue    Define a string value here.");
            lines[2].ShouldBeEquivalentTo("--shape          Define a enum value here.");
            lines[3].ShouldBeEquivalentTo("--help           Display this help screen.");
            lines[4].ShouldBeEquivalentTo("--version        Display version information.");
            lines[5].ShouldBeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void When_defined_MetaValue_should_be_rendered()
        {
            // Fixture setup
            // Exercize system 
            var sut =
                new HelpText("Meta Value.").AddOptions(
                    new NotParsed<FakeOptionsWithMetaValue>(TypeInfo.Create(typeof(FakeOptionsWithMetaValue)), Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();

            lines[2].ShouldBeEquivalentTo("i FILE, input-file=FILE    Required. Specify input FILE to be processed.");
            // Teardown
        }

        [Fact]
        public void When_help_text_is_longer_than_width_it_will_wrap_around_as_if_in_a_column()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 40;
            sut.AddOptions(
                new NotParsed<FakeOptionsWithLongDescription>(
                    TypeInfo.Create(typeof(FakeOptionsWithLongDescription)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].ShouldBeEquivalentTo("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].ShouldBeEquivalentTo("                of the verbosity to ");
            lines[4].ShouldBeEquivalentTo("                test out the wrapping ");
            lines[5].ShouldBeEquivalentTo("                capabilities of the ");
            lines[6].ShouldBeEquivalentTo("                Help Text.");
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
                new NotParsed<FakeOptionsWithLongDescriptionAndNoSpaces>(
                    TypeInfo.Create(typeof(FakeOptionsWithLongDescriptionAndNoSpaces)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines();
            lines[1].ShouldBeEquivalentTo("  v, verbose    Before ");
            lines[2].ShouldBeEquivalentTo("                012345678901234567890123");
            lines[3].ShouldBeEquivalentTo("                After");
            lines[4].ShouldBeEquivalentTo("  input-file    Before ");
            lines[5].ShouldBeEquivalentTo("                012345678901234567890123");
            lines[6].ShouldBeEquivalentTo("                456789 After");
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
                .AddOptions(new NotParsed<FakeOptionsForHelp>(TypeInfo.Create(typeof(FakeOptionsForHelp)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("Before 0123456789012345678901234567890123456789 After");

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines();
            lines[1].ShouldBeEquivalentTo("Before ");
            lines[2].ShouldBeEquivalentTo("0123456789012345678901234567890123456789");
            lines[3].ShouldBeEquivalentTo("012 After");
            lines[lines.Length - 3].ShouldBeEquivalentTo("Before ");
            lines[lines.Length - 2].ShouldBeEquivalentTo("0123456789012345678901234567890123456789");
            lines[lines.Length - 1].ShouldBeEquivalentTo(" After");

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

            lines[0].ShouldBeEquivalentTo("  ERR badtoken");
            lines[1].ShouldBeEquivalentTo("  ERR x, switch");
            lines[2].ShouldBeEquivalentTo("  ERR unknown");
            lines[3].ShouldBeEquivalentTo("  ERR missing");
            lines[4].ShouldBeEquivalentTo("  ERR s, sequence");
            lines[5].ShouldBeEquivalentTo("  ERR no-verb-selected");
            lines[6].ShouldBeEquivalentTo("  ERR badverb");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Options_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<FakeOptions>(
                TypeInfo.Create(typeof(FakeOptions)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken"),
                        new SequenceOutOfRangeError(new NameInfo("i", ""))
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[4].ShouldBeEquivalentTo("A sequence option 'i' is defined with fewer or more items than required.");
            lines[5].ShouldBeEquivalentTo("--stringvalue    Define a string value here.");
            lines[6].ShouldBeEquivalentTo("-i               Define a int sequence here.");
            lines[7].ShouldBeEquivalentTo("-x               Define a boolean or switch value here.");
            lines[8].ShouldBeEquivalentTo("--help           Display this help screen.");
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
                        new HelpVerbRequestedError("commit", typeof(CommitOptions), true)
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
            lines[2].ShouldBeEquivalentTo("-p, --patch      Use the interactive patch selection interface to chose which");
            lines[3].ShouldBeEquivalentTo("changes to commit.");
            lines[4].ShouldBeEquivalentTo("--amend          Used to amend the tip of the current branch.");
            lines[5].ShouldBeEquivalentTo("-m, --message    Use the given message as the commit message.");
            lines[6].ShouldBeEquivalentTo("--help           Display this help screen.");
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_unknown_verb_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var verbTypes = Enumerable.Empty<Type>().Concat(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) });
            var fakeResult = new NotParsed<object>(
                TypeInfo.Create(typeof(NullInstance),
                    verbTypes),
                new Error[] { new HelpVerbRequestedError(null, null, false) });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
            lines[2].ShouldBeEquivalentTo("add        Add file contents to the index.");
            lines[3].ShouldBeEquivalentTo("commit     Record changes to the repository.");
            lines[4].ShouldBeEquivalentTo("clone      Clone a repository into a new directory.");
            lines[5].ShouldBeEquivalentTo("help       Display more information on a specific command.");
            lines[6].ShouldBeEquivalentTo("version    Display version information.");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_options_and_values()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<FakeOptionsWithHelpTextValue>(TypeInfo.Create(typeof(FakeOptionsWithHelpTextValue)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("pre-options");
            lines[1].ShouldBeEquivalentTo("--stringvalue=STR            Define a string value here.");
            lines[2].ShouldBeEquivalentTo("-i INTSEQ                    Define a int sequence here.");
            lines[3].ShouldBeEquivalentTo("-x                           Define a boolean or switch value here.");
            lines[4].ShouldBeEquivalentTo("--help                       Display this help screen.");
            lines[5].ShouldBeEquivalentTo("--version                    Display version information.");
            lines[6].ShouldBeEquivalentTo("number (pos. 0) NUM          Define a long value here.");
            lines[7].ShouldBeEquivalentTo("paintcolor (pos. 1) COLOR    Define a color value here.");
            lines[8].ShouldBeEquivalentTo("post-options", lines[8]);
            // Teardown
        }

        [Fact]
        public static void RenderUsageText_returns_properly_formatted_text()
        {
            // Fixture setup
            ParserResult<FakeOptionsWithUsageText> result =
                new NotParsed<FakeOptionsWithUsageText>(
                    TypeInfo.Create(typeof(FakeOptionsWithUsageText)), Enumerable.Empty<Error>());
            
            // Exercize system
            var text = HelpText.RenderUsageText(result);

            // Verify outcome
            var lines = text.ToNotEmptyLines();

            // Teardown
            lines[0].ShouldBeEquivalentTo("Normal scenario:");
            lines[1].ShouldBeEquivalentTo("  mono testapp.exe --input file.bin --output out.bin");
            lines[2].ShouldBeEquivalentTo("Logging warnings:");
            lines[3].ShouldBeEquivalentTo("  mono testapp.exe -w --input file.bin");
            lines[4].ShouldBeEquivalentTo("Logging errors:");
            lines[5].ShouldBeEquivalentTo("  mono testapp.exe -e --input file.bin");
            lines[6].ShouldBeEquivalentTo("  mono testapp.exe --errs --input=file.bin");
        }

        [Fact]
        public void Invoke_AutoBuild_for_Options_with_Usage_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new NotParsed<FakeOptionsWithUsageText>(
                TypeInfo.Create(typeof(FakeOptionsWithUsageText)),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken")
                    });

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var text = helpText.ToString();
            var lines = text.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[4].ShouldBeEquivalentTo("USAGE:");
            lines[5].ShouldBeEquivalentTo("Normal scenario:");
            lines[6].ShouldBeEquivalentTo("mono testapp.exe --input file.bin --output out.bin");
            lines[7].ShouldBeEquivalentTo("Logging warnings:");
            lines[8].ShouldBeEquivalentTo("mono testapp.exe -w --input file.bin");
            lines[9].ShouldBeEquivalentTo("Logging errors:");
            lines[10].ShouldBeEquivalentTo("mono testapp.exe -e --input file.bin");
            lines[11].ShouldBeEquivalentTo("mono testapp.exe --errs --input=file.bin");
            lines[12].ShouldBeEquivalentTo("-i, --input     Set input file.");
            lines[13].ShouldBeEquivalentTo("-i, --output    Set output file.");
            lines[14].ShouldBeEquivalentTo("--verbose       Set verbosity level.");
            lines[15].ShouldBeEquivalentTo("-w, --warns     Log warnings.");
            lines[16].ShouldBeEquivalentTo("-e, --errs      Log errors.");
            lines[17].ShouldBeEquivalentTo("--help          Display this help screen.");
            lines[18].ShouldBeEquivalentTo("--version       Display version information.");

            // Teardown
        }

        [Fact]
        public void Default_set_to_sequence_should_be_properly_printed()
        {
            // Fixture setup
            var handlers = new CultureInfo("en-US").MakeCultureHandlers();
            var fakeResult =
                new NotParsed<FakeOptionsWithDefaultSetToSequence>(
                    typeof(FakeOptionsWithDefaultSetToSequence).ToTypeInfo(),
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
    }
}