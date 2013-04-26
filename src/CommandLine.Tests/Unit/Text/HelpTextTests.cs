// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;
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
            Assert.Equal(string.Empty, new HelpText().ToString());
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

            Assert.Equal("Unit-tests 2.0", lines[0]);
            Assert.Equal("Copyright (C) 2005 - 2013 Author", lines[1]);
            Assert.Equal("pre-options line 1", lines[2]);
            Assert.Equal("pre-options line 2", lines[3]);
            Assert.Equal("post-options line 1", lines[4]);
            Assert.Equal("post-options line 2", lines[5]);
            // Teardown
        }

        [Fact]
        public void Create_instance_with_options()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText { AddDashesToOption = true }
                .AddPreOptionsLine("pre-options")
                .AddOptions(new FakeOptions())
                .AddPostOptionsLine("post-options");

            // Verify outcome

            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();
            Assert.Equal("pre-options", lines[0]);
            Assert.Equal("--stringvalue    Define a string value here.", lines[1]);
            Assert.Equal("-i               Define a int sequence here.", lines[2]);
            Assert.Equal("-x               Define a boolean or switch value here.", lines[3]);
            Assert.Equal("--help           Display this help screen.", lines[4]);
            Assert.Equal( "post-options", lines[5]);
            // Teardown
        }

        [Fact]
        public void When_defined_MetaValue_should_be_rendered()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText("Meta Value.")
                .AddOptions(new FakeOptionsWithMetaValue());

            // Verify outcome
            var lines = sut.ToString().ToNotEmptyLines().TrimStringArray();

            Assert.Equal("i FILE, input-file=FILE    Required. Specify input FILE to be processed.", lines[2]);
            // Teardown
        }

        [Fact]
        public void When_help_text_is_longer_than_width_it_will_wrap_around_as_if_in_a_column()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 40;
            sut.AddOptions(new FakeOptionsWithLongDescription());

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().Be("  v, verbose    This is the description"); //"The first line should have the arguments and the start of the Help Text.");
            //string formattingMessage = "Beyond the second line should be formatted as though it's in a column.";
            lines[3].Should().Be("                of the verbosity to ");
            lines[4].Should().Be("                test out the wrapping ");
            lines[5].Should().Be("                capabilities of the ");
            lines[6].Should().Be("                Help Text.");
            // Teardown
        }

        [Fact]
        public void Long_help_text_without_spaces()
        {
            // Fixture setup
            // Exercize system 
            var sut = new HelpText(new HeadingInfo("CommandLine.Tests.dll", "1.9.4.131"));
            sut.MaximumDisplayWidth = 40;
            sut.AddOptions(new FakeOptionsWithLongDescriptionAndNoSpaces());

            // Verify outcome
            var lines = sut.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines[2].Should().Be("  v, verbose    Before ");
            lines[3].Should().Be("                012345678901234567890123");
            lines[4].Should().Be("                After");
            lines[5].Should().Be("  input-file    Before ");
            lines[6].Should().Be("                012345678901234567890123");
            lines[7].Should().Be("                456789 After");
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
                .AddOptions(new FakeOptionsForHelp())
                .AddPostOptionsLine("Before 0123456789012345678901234567890123456789 After");

            // Verify outcome
            var lines = sut.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            lines[1].Should().Be("Before ");
            lines[2].Should().Be("0123456789012345678901234567890123456789");
            lines[3].Should().Be("012 After");
            lines[lines.Length - 3].Should().Be("Before ");
            lines[lines.Length - 2].Should().Be("0123456789012345678901234567890123456789");
            lines[lines.Length - 1].Should().Be(" After");

            // Teardown
        }
    
        [Fact]
        public void Invoking_RenderParsingErrorsText_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new ParserResult<NullInstance>(
                ParserResultType.Options,
                new NullInstance(),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken"),
                        new MissingValueOptionError(new NameInfo("x", "switch")),
                        new UnknownOptionError("unknown"),
                        new MissingRequiredOptionError(new NameInfo("", "missing")),
                        new MutuallyExclusiveSetError(new NameInfo("z", "")),
                        new SequenceOutOfRangeError(new NameInfo("s", "sequence")),
                        new NoVerbSelectedError(),
                        new BadVerbSelectedError("badverb"),
                        new HelpRequestedError(), // should be ignored
                        new HelpVerbRequestedError(null, null, false), // should be ignored 
                    },
                Maybe.Nothing<IEnumerable<Type>>());
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
                        case ErrorType.MutuallyExclusiveSetError:
                            return "ERR " + ((MutuallyExclusiveSetError)err).NameInfo.NameText;
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

            // Exercize system
            var errorsText = HelpText.RenderParsingErrorsText(fakeResult, fakeRenderer, 2);

            // Verify outcome
            var lines = errorsText.ToNotEmptyLines();

            Assert.Equal("  ERR badtoken", lines[0]);
            Assert.Equal("  ERR x, switch", lines[1]);
            Assert.Equal("  ERR unknown", lines[2]);
            Assert.Equal("  ERR missing", lines[3]);
            Assert.Equal("  ERR z", lines[4]);
            Assert.Equal("  ERR s, sequence", lines[5]);
            Assert.Equal("  ERR no-verb-selected", lines[6]);
            Assert.Equal("  ERR badverb", lines[7]);
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Options_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new ParserResult<FakeOptions>(
                ParserResultType.Options,
                new FakeOptions(),
                new Error[]
                    {
                        new BadFormatTokenError("badtoken"),
                        new SequenceOutOfRangeError(new NameInfo("i", ""))
                    },
                Maybe.Nothing<IEnumerable<Type>>());

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            Assert.True(lines[0].StartsWith("CommandLine", StringComparison.Ordinal));
            Assert.True(lines[1].StartsWith("Copyright (c)", StringComparison.Ordinal));
            Assert.Equal("ERROR(S):", lines[2]);
            Assert.Equal("Token 'badtoken' is not recognized.", lines[3]);
            Assert.Equal("A sequence option 'i' is defined with few items than required.", lines[4]);
            Assert.Equal("--stringvalue    Define a string value here.", lines[5]);
            Assert.Equal("-i               Define a int sequence here.", lines[6]);
            Assert.Equal("-x               Define a boolean or switch value here.", lines[7]);
            Assert.Equal("--help           Display this help screen.", lines[8]);
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_specific_verb_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var fakeResult = new ParserResult<object>(
                ParserResultType.Verbs,
                new NullInstance(),
                new Error[]
                    {
                        new HelpVerbRequestedError("commit", typeof(CommitOptions), true)
                    },
                Maybe.Nothing<IEnumerable<Type>>());

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            Assert.True(lines[0].StartsWith("CommandLine", StringComparison.Ordinal));
            Assert.True(lines[1].StartsWith("Copyright (c)", StringComparison.Ordinal));
            Assert.Equal("-p, --patch    Use the interactive patch selection interface to chose which", lines[2]);
            Assert.Equal("changes to commit.", lines[3]);
            Assert.Equal("--amend        Used to amend the tip of the current branch.", lines[4]);
            Assert.Equal("--help         Display this help screen.", lines[5]);
            // Teardown
        }

        [Fact]
        public void Invoke_AutoBuild_for_Verbs_with_unknown_verb_returns_appropriate_formatted_text()
        {
            // Fixture setup
            var verbTypes = Enumerable.Empty<Type>().Concat(
                new[] { typeof(AddOptions), typeof(CommitOptions), typeof(CloneOptions) });
            var fakeResult = new ParserResult<object>(
                ParserResultType.Verbs,
                new NullInstance(),
                new Error[]
                    {
                        new HelpVerbRequestedError(null, null, false)
                    },
                Maybe.Just(verbTypes));

            // Exercize system
            var helpText = HelpText.AutoBuild(fakeResult);

            // Verify outcome
            var lines = helpText.ToString().ToNotEmptyLines().TrimStringArray();

            Assert.True(lines[0].StartsWith("CommandLine", StringComparison.Ordinal));
            Assert.True(lines[1].StartsWith("Copyright (c)", StringComparison.Ordinal));
            Assert.Equal("add       Add file contents to the index.", lines[2]);
            Assert.Equal("commit    Record changes to the repository.", lines[3]);
            Assert.Equal("clone     Clone a repository into a new directory.", lines[4]);
            Assert.Equal("help      Display more information on a specific command.", lines[5]);
            // Teardown
        }
    }
}