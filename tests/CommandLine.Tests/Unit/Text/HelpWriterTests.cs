using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Text
{
    public class HelpWriterTests
    {
        private static readonly HelpText TestHelpText = new HelpText("Test Heading", "Test Copyright");

        private static readonly IEnumerable<Error> UserRequestedErrors = new Error[]
        {
            new HelpRequestedError(),
            new HelpVerbRequestedError("add", typeof(Add_Verb), false),
            new VersionRequestedError()
        };

        private static readonly IEnumerable<Error> ErrorTriggeredErrors = new Error[]
        {
            new BadFormatTokenError("someToken"),
            new MissingValueOptionError(new NameInfo("t", "test")),
            new UnknownOptionError("someToken"),
            new MissingRequiredOptionError(new NameInfo("t", "test")),
            new MutuallyExclusiveSetError(new NameInfo("t", "test"), "someSetName"),
            new BadFormatConversionError(new NameInfo("t", "test")),
            new SequenceOutOfRangeError(new NameInfo("t", "test")),
            new RepeatedOptionError(new NameInfo("t", "test")),
            new BadVerbSelectedError("someToken"),
            new NoVerbSelectedError(),
            new SetValueExceptionError(new NameInfo("t", "test"), new Exception(), 0)
        };

        [Fact]
        public void Initializing_With_One_TextWriter_Writes_All_Help_To_That_Writer()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HelpWriter helpWriter = new HelpWriter(writer);

            AssertHelpWritten(helpWriter, UserRequestedErrors.Concat(ErrorTriggeredErrors), builder);
        }

        [Fact]
        public void Initializing_With_Only_Writer_Writes_User_Requested_Help_To_That_Writer()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HelpWriter helpWriter = new HelpWriter(writer, null);

            AssertHelpWritten(helpWriter, UserRequestedErrors, builder);
        }

        [Fact]
        public void Initializing_With_Only_Writer_Skips_Writing_Error_Triggered_Help_To_That_Writer()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HelpWriter helpWriter = new HelpWriter(writer, null);

            AssertNoHelpWritten(helpWriter, ErrorTriggeredErrors, builder);
        }

        [Fact]
        public void Initializing_With_Only_ErrorWriter_Writes_Error_Triggered_Help_To_That_Writer()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HelpWriter helpWriter = new HelpWriter(null, writer);

            AssertHelpWritten(helpWriter, ErrorTriggeredErrors, builder);
        }

        [Fact]
        public void Initializing_With_Only_ErrorWriter_Skips_Writing_User_Requested_Help_To_That_Writer()
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            HelpWriter helpWriter = new HelpWriter(null, writer);

            AssertNoHelpWritten(helpWriter, UserRequestedErrors, builder);
        }

        [Fact]
        public void Initializing_With_Both_Writers_Redirects_Help_Based_On_Errors()
        {
            StringBuilder userRequestedBuilder = new StringBuilder();
            StringBuilder errorTriggeredBuilder = new StringBuilder();

            StringWriter userRequestedWriter = new StringWriter(userRequestedBuilder);
            StringWriter errorTriggeredWriter = new StringWriter(errorTriggeredBuilder);

            HelpWriter helpWriter = new HelpWriter(userRequestedWriter, errorTriggeredWriter);

            foreach (Error error in UserRequestedErrors)
            {
                userRequestedBuilder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");
                errorTriggeredBuilder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");

                Error[] currentError = new Error[] { error };

                helpWriter.WriteHelpText(currentError, TestHelpText);

                userRequestedBuilder.Length.Should().BeGreaterThan(0, "user requested help text should have been written to the writer.");
                errorTriggeredBuilder.Length.Should().Be(0, "no user requested help text should have been written to the writer.");

                userRequestedBuilder.Clear();
            }

            foreach (Error error in ErrorTriggeredErrors)
            {
                userRequestedBuilder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");
                errorTriggeredBuilder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");

                Error[] currentError = new Error[] { error };

                helpWriter.WriteHelpText(currentError, TestHelpText);

                userRequestedBuilder.Length.Should().Be(0, "no error triggered help text should have been written to the writer.");
                errorTriggeredBuilder.Length.Should().BeGreaterThan(0, "error triggered help text should have been written to the writer.");

                errorTriggeredBuilder.Clear();
            }
        }

        private void AssertHelpWritten(HelpWriter helpWriter, IEnumerable<Error> errors, StringBuilder builder)
        {
            foreach (Error error in errors)
            {
                builder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");

                Error[] currentError = new Error[] { error };

                helpWriter.WriteHelpText(currentError, TestHelpText);

                builder.Length.Should().BeGreaterThan(0, "help text should have been written to the writer.");

                builder.Clear();
            }
        }

        private void AssertNoHelpWritten(HelpWriter helpWriter, IEnumerable<Error> errors, StringBuilder builder)
        {
            foreach (Error error in errors)
            {
                builder.Length.Should().Be(0, "no help text should have been written to the writer before WriteHelpText is called.");

                Error[] currentError = new Error[] { error };

                helpWriter.WriteHelpText(currentError, TestHelpText);

                builder.Length.Should().Be(0, "no help text should have been written to the writer.");
            }
        }
    }
}
