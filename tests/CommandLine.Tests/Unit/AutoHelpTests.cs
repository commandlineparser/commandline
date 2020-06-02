using System;
using Xunit;
using FluentAssertions;
using CommandLine.Core;
using CommandLine.Tests.Fakes;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CommandLine.Tests.Unit
{
    public class AutoHelpTests
    {
        public AutoHelpTests()
        {
        }

        public static object[][] ValidArgsData = new []
        {
            new [] { "--help" },
            new [] { "--help", "256" },
            new [] { "--help", "--stringvalue", "foo" },
            new [] { "--help", "--stringvalue=foo" },
            new [] { "--stringvalue", "foo", "--help" },
            new [] { "--stringvalue=foo", "--help" },
            new [] { "--help", "--stringvalue", "foo", "-x" },
            new [] { "--help", "--stringvalue=foo", "-x" },
            new [] { "--stringvalue", "foo", "--help", "-x" },
            new [] { "--stringvalue=foo", "--help", "-x" },
            new [] { "--stringvalue", "foo", "-x", "256", "--help" },
            new [] { "--stringvalue=foo", "-x", "256", "--help" },
            new [] { "--stringvalue", "foo", "--help", "-x", "256" },
            new [] { "--stringvalue=foo", "--help", "-x", "256" },
            new [] { "--help", "--stringvalue", "foo", "-x", "256" },
            new [] { "--help", "--stringvalue=foo", "-x", "256" },
        };

        public static object[][] InvalidArgsData = new []
        {
            new [] { "--help", "foo" },
            new [] { "--help", "-s" },
            new [] { "--help", "-i", "foo" },
            new [] {"--help", "--invalid-switch", "foo" },
            new [] {"--invalid-switch", "foo", "--help" },
            new [] {"--invalid-switch", "--help", "foo" },
        };

        public static object[][] ConsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--help" },
            new [] { "--stringvalue=--help" },
            new [] { "--stringvalue", "--help", "-s", "--help" },
            new [] { "--stringvalue=--help", "-s", "--help" },
            new [] { "--stringvalue", "--help", "-s--help" },
            new [] { "--stringvalue=--help", "-s--help" },
        };

        public static object[][] MixOfConsumedAndUnconsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--help", "--help" },
            new [] { "--help", "--stringvalue", "--help" },
            new [] { "--stringvalue=--help", "--help" },
            new [] { "--help", "--stringvalue=--help" },
            new [] { "--stringvalue", "--help", "-s", "--help", "--help" },
            new [] { "--stringvalue", "--help", "--help", "-s", "--help" },
            new [] { "--help", "--stringvalue", "--help", "-s", "--help" },
            new [] { "--stringvalue=--help", "-s", "--help", "--help" },
            new [] { "--stringvalue=--help", "--help", "-s", "--help" },
            new [] { "--help", "--stringvalue=--help", "-s", "--help" },
            new [] { "--stringvalue", "--help", "-s--help", "--help" },
            new [] { "--stringvalue", "--help", "--help", "-s--help", "--help" },
            new [] { "--help", "--stringvalue", "--help", "-s--help" },
            new [] { "--stringvalue=--help", "-s--help", "--help" },
            new [] { "--stringvalue=--help", "--help", "-s--help" },
            new [] { "--help", "--stringvalue=--help", "-s--help" },
        };

        public static object[][] ConsumedDashDashHelpInvalidArgsData = new []
        {
            new [] { "--stringvalue", "--help", "foo" },
            new [] { "-s", "--help", "--stringvalue" },
            new [] { "-s", "--help", "-i", "foo" },
            new [] { "--stringvalue", "--help", "--invalid-switch", "256" },
            new [] { "--stringvalue=--help", "--invalid-switch", "256" },
            new [] { "--invalid-switch", "-s", "--help" },
        };

        public static IEnumerable<object[]> ConvertDataToShortOption(object[][] data, string search, string replacement)
        {
            foreach (object[] row in data)
            {
                var strings = row as string[];
                if (strings != null)
                {
                    yield return strings.Select(item => item.Replace(search, replacement)).ToArray();
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidArgsData))]
        public void Explicit_help_command_with_valid_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> ValidArgsDataWithShortOption =
            ConvertDataToShortOption(ValidArgsData, "--help", "-h");

        [Theory]
        [MemberData(nameof(ValidArgsDataWithShortOption))]
        public void Explicit_help_command_with_valid_args_and_short_option_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgsData))]
        public void Explicit_help_command_with_invalid_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> InvalidArgsDataWithShortOption =
            ConvertDataToShortOption(InvalidArgsData, "--help", "-h");

        [Theory]
        [MemberData(nameof(InvalidArgsDataWithShortOption))]
        public void Explicit_help_command_with_invalid_args_and_short_option_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsData))]
        public void Dash_dash_help_in_a_string_value_does_not_produce_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("--help");
            shortAndLong.Should().BeOneOf("--help", null, "");
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpValidArgsData, "--help", "-h");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsDataWithShortOption))]
        public void Dash_dash_help_in_a_string_value_with_short_option_does_not_produce_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("-h");
            shortAndLong.Should().BeOneOf("-h", null, "");
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpValidArgsDataWithShortOptionInData =
            ConvertDataToShortOption(ConsumedDashDashHelpValidArgsData, "--help", "h");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsDataWithShortOptionInData))]
        public void Dash_dash_help_in_a_string_value_with_short_option_in_data_does_not_produce_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("h");
            shortAndLong.Should().BeOneOf("h", null, "");
        }

        [Theory]
        [MemberData(nameof(MixOfConsumedAndUnconsumedDashDashHelpValidArgsData))]
        public void Explicit_help_command_mixed_with_some_consumed_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> MixOfConsumedAndUnconsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(MixOfConsumedAndUnconsumedDashDashHelpValidArgsData, "--help", "-h");

        [Theory]
        [MemberData(nameof(MixOfConsumedAndUnconsumedDashDashHelpValidArgsDataWithShortOption))]
        public void Explicit_short_help_command_mixed_with_some_consumed_args_produces_just_one_HelpRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<HelpRequestedError>();
        }

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpInvalidArgsData))]
        public void Dash_dash_help_consumed_by_valid_args_with_invalid_args_produces_no_HelpRequestedErrors(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCountGreaterOrEqualTo(1);
            result.As<NotParsed<Simple_Options>>().Errors.Should().NotBeOfType<HelpRequestedError>();
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpInvalidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpInvalidArgsData, "--help", "-h");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpInvalidArgsDataWithShortOption))]
        public void Dash_h_consumed_by_valid_args_with_invalid_args_produces_no_HelpRequestedErrors(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoHelp = true;
                config.AutoHelpShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCountGreaterOrEqualTo(1);
            result.As<NotParsed<Simple_Options>>().Errors.Should().NotBeOfType<HelpRequestedError>();
        }

        [Fact]
        public void Explicit_help_request_generates_help_requested_error()
        {
            // Fixture setup
            var expectedError = new HelpRequestedError();
            var sut = new Parser();

            // Exercize system
            var result = sut.ParseArguments<Simple_Options>(new[] { "--help" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<Simple_Options>)result).Errors.Should().ContainSingle(e => e.Equals(expectedError));
            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_AutoHelp_off_generates_unknown_option_error()
        {
            // Fixture setup
            var expectedError = new UnknownOptionError("help");
            var sut = new Parser(config => { config.AutoHelp = false; });

            // Exercise system
            var result = sut.ParseArguments<Simple_Options>(new[] { "--help" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<Simple_Options>)result).Errors.Single().Tag.Should().Be(expectedError.Tag);
            ((NotParsed<Simple_Options>)result).Errors.First().As<UnknownOptionError>().Token.Should().BeEquivalentTo(expectedError.Token);

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_AutoHelp_off_displays_unknown_option_error()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => { config.AutoHelp = false; config.HelpWriter = help; });

            // Exercise system
            sut.ParseArguments<Simple_Options>(new[] { "--help" });
            var result = help.ToString();

            // Verify outcome

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(CommandLine.Text.HeadingInfo.Default.ToString());
            lines[1].Should().Be(CommandLine.Text.CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("Option 'help' is unknown.");

            // Teardown
        }

        [Fact]
        public void Explicit_help_request_with_AutoHelp_off_and_IgnoreUnknownArguments_on_does_not_generate_help_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => { config.HelpWriter = help; config.AutoHelp = false; config.IgnoreUnknownArguments = true; });

            // Exercize system
            sut.ParseArguments<Simple_Options>(new[] { "--help" });
            var result = help.ToString();

            // Verify outcome
            result.Should().BeEquivalentTo("");
            // Teardown
        }
    }
}
