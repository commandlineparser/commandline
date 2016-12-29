// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommandLine.Core;
using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;
using CommandLine.Tests.Unit.Infrastructure;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using System.Text;

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
                .AddOptions(new NotParsed<Simple_Options>(TypeInfo.Create(typeof(Simple_Options)), Enumerable.Empty<Error>()))
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            lines[0].ShouldBeEquivalentTo("pre-options");
            lines[1].ShouldBeEquivalentTo("--stringvalue         Define a string value here.");
            lines[2].ShouldBeEquivalentTo("-s, --shortandlong    Example with both short and long name.");
            lines[3].ShouldBeEquivalentTo("-i                    Define a int sequence here.");
            lines[4].ShouldBeEquivalentTo("-x                    Define a boolean or switch value here.");
            lines[5].ShouldBeEquivalentTo("--help                Display this help screen.");
            lines[6].ShouldBeEquivalentTo("--version             Display version information.");
            lines[7].ShouldBeEquivalentTo("value pos. 0          Define a long value here.");
            lines[8].ShouldBeEquivalentTo("post-options");
            // Teardown
        }

        [Fact]
        public void Create_instance_with_enum_options_enabled()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true, AddEnumValuesToHelpText = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new NotParsed<Options_With_Enum_Having_HelpText>(TypeInfo.Create(typeof(Options_With_Enum_Having_HelpText)), Enumerable.Empty<Error>()))
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
                .AddOptions(new NotParsed<Options_With_Enum_Having_HelpText>(TypeInfo.Create(typeof(Options_With_Enum_Having_HelpText)), Enumerable.Empty<Error>()))
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
                    new NotParsed<Options_With_MetaValue>(TypeInfo.Create(typeof(Options_With_MetaValue)), Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();

            lines[2].ShouldBeEquivalentTo("i FILE, input-file=FILE    Required. Specify input FILE to be processed.");
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
            lines[2].ShouldBeEquivalentTo("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].ShouldBeEquivalentTo("                of the verbosity to ");
            lines[4].ShouldBeEquivalentTo("                test out the wrapping ");
            lines[5].ShouldBeEquivalentTo("                capabilities of the ");
            lines[6].ShouldBeEquivalentTo("                Help Text.");
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
            lines[2].ShouldBeEquivalentTo("  v, verbose    This is the description of the verbosity to test out the wrapping capabilities of "); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].ShouldBeEquivalentTo("                the Help Text.");
            // Teardown
        }

        [Fact]
        public void When_help_text_has_hidden_option_it_should_not_be_added_to_help_text_output()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.AddOptions(
                new NotParsed<Simple_Options_With_HelpText_Set_To_Long_Description>(
                    TypeInfo.Create(typeof(Simple_Options_With_HelpText_Set_To_Long_Description)),
                    Enumerable.Empty<Error>()));

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].ShouldBeEquivalentTo("  v, verbose    This is the description of the verbosity to test out the "); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].ShouldBeEquivalentTo("                wrapping capabilities of the Help Text.");
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
                .AddOptions(new NotParsed<Simple_Options_Without_HelpText>(TypeInfo.Create(typeof(Simple_Options_Without_HelpText)), Enumerable.Empty<Error>()))
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
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();
#if !PLATFORM_DOTNET
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
#else
            // Takes the name of the xUnit test program
            lines[0].Should().StartWithEquivalent("xUnit");
            lines[1].Should().StartWithEquivalent("Copyright (C) Outercurve Foundation");
#endif
            lines[2].ShouldBeEquivalentTo("ERROR(S):");
            lines[3].ShouldBeEquivalentTo("Token 'badtoken' is not recognized.");
            lines[4].ShouldBeEquivalentTo("A sequence option 'i' is defined with fewer or more items than required.");
            lines[5].ShouldBeEquivalentTo("--stringvalue         Define a string value here.");
            lines[6].ShouldBeEquivalentTo("-s, --shortandlong    Example with both short and long name.");
            lines[7].ShouldBeEquivalentTo("-i                    Define a int sequence here.");
            lines[8].ShouldBeEquivalentTo("-x                    Define a boolean or switch value here.");
            lines[9].ShouldBeEquivalentTo("--help                Display this help screen.");
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
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

#if !PLATFORM_DOTNET
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
#else
            // Takes the name of the xUnit test program
            lines[0].Should().StartWithEquivalent("xUnit");
            lines[1].Should().StartWithEquivalent("Copyright (C) Outercurve Foundation");
#endif
            lines[2].ShouldBeEquivalentTo("-p, --patch      Use the interactive patch selection interface to chose which");
            lines[3].ShouldBeEquivalentTo("changes to commit.");
            lines[4].ShouldBeEquivalentTo("--amend          Used to amend the tip of the current branch.");
            lines[5].ShouldBeEquivalentTo("-m, --message    Use the given message as the commit message.");
            lines[6].ShouldBeEquivalentTo("--help           Display this help screen.");
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

#if !PLATFORM_DOTNET
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].ShouldBeEquivalentTo("Copyright (c) 2005 - 2015 Giacomo Stelluti Scala");
#else
            // Takes the name of the xUnit test program
            lines[0].Should().StartWithEquivalent("xUnit");
            lines[1].Should().StartWithEquivalent("Copyright (C) Outercurve Foundation");
#endif
            lines[2].ShouldBeEquivalentTo("-p, --patch      Use the interactive patch selection interface to chose which changes to commit.");
            lines[3].ShouldBeEquivalentTo("--amend          Used to amend the tip of the current branch.");
            lines[4].ShouldBeEquivalentTo("-m, --message    Use the given message as the commit message.");
            lines[5].ShouldBeEquivalentTo("--help           Display this help screen.");
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

#if !PLATFORM_DOTNET
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
#else
            // Takes the name of the xUnit test program
            lines[0].Should().StartWithEquivalent("xUnit");
            lines[1].Should().StartWithEquivalent("Copyright (C) Outercurve Foundation");
#endif
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
                .AddOptions(new NotParsed<Options_With_HelpText_And_MetaValue>(TypeInfo.Create(typeof(Options_With_HelpText_And_MetaValue)), Enumerable.Empty<Error>()))
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
            ParserResult<Options_With_Usage_Attribute> result =
                new NotParsed<Options_With_Usage_Attribute>(
                    TypeInfo.Create(typeof(Options_With_Usage_Attribute)), Enumerable.Empty<Error>());
            
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
            lines[7].ShouldBeEquivalentTo("List:");
            lines[8].ShouldBeEquivalentTo("  mono testapp.exe -l 1,2");
            lines[9].ShouldBeEquivalentTo("Value:");
            lines[10].ShouldBeEquivalentTo("  mono testapp.exe value");
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
            var lines = text.ToNotEmptyLines().TrimStringArray();
#if !PLATFORM_DOTNET
            lines[0].Should().StartWithEquivalent("CommandLine");
            lines[1].Should().StartWithEquivalent("Copyright (c)");
#else
            // Takes the name of the xUnit test program
            lines[0].Should().StartWithEquivalent("xUnit");
            lines[1].Should().StartWithEquivalent("Copyright (C) Outercurve Foundation");
#endif
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
            lines[12].ShouldBeEquivalentTo("List:");
            lines[13].ShouldBeEquivalentTo("mono testapp.exe -l 1,2");
            lines[14].ShouldBeEquivalentTo("Value:");
            lines[15].ShouldBeEquivalentTo("mono testapp.exe value");
            lines[16].ShouldBeEquivalentTo("-i, --input     Set input file.");
            lines[17].ShouldBeEquivalentTo("-i, --output    Set output file.");
            lines[18].ShouldBeEquivalentTo("--verbose       Set verbosity level.");
            lines[19].ShouldBeEquivalentTo("-w, --warns     Log warnings.");
            lines[20].ShouldBeEquivalentTo("-e, --errs      Log errors.");
            lines[21].ShouldBeEquivalentTo("-l              List.");
            lines[22].ShouldBeEquivalentTo("--help          Display this help screen.");
            lines[23].ShouldBeEquivalentTo("--version       Display version information.");
            lines[24].ShouldBeEquivalentTo("value pos. 0    Value.");

            // Teardown
        }

#if !PLATFORM_DOTNET
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
#endif

        [Fact]
        public void AutoBuild_when_no_assembly_attributes()
        {
            try
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
            finally
            {
                ReflectionHelper.SetAttributeOverride(null);
            }
        }

        [Fact]
        public void AutoBuild_with_assembly_title_and_version_attributes_only()
        {
            try
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
            finally
            {
                ReflectionHelper.SetAttributeOverride(null);
            }
        }


        [Fact]
        public void AutoBuild_with_assembly_company_attribute_only()
        {
            try
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
            finally
            {
                ReflectionHelper.SetAttributeOverride(null);
            }
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
    }
}
