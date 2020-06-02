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
    public class AutoVersionTests
    {
        public AutoVersionTests()
        {
        }

        public static object[][] ValidArgsData = new []
        {
            new [] { "--version" },
            new [] { "--version", "256" },
            new [] { "--version", "--stringvalue", "foo" },
            new [] { "--version", "--stringvalue=foo" },
            new [] { "--stringvalue", "foo", "--version" },
            new [] { "--stringvalue=foo", "--version" },
            new [] { "--version", "--stringvalue", "foo", "-x" },
            new [] { "--version", "--stringvalue=foo", "-x" },
            new [] { "--stringvalue", "foo", "--version", "-x" },
            new [] { "--stringvalue=foo", "--version", "-x" },
            new [] { "--stringvalue", "foo", "-x", "256", "--version" },
            new [] { "--stringvalue=foo", "-x", "256", "--version" },
            new [] { "--stringvalue", "foo", "--version", "-x", "256" },
            new [] { "--stringvalue=foo", "--version", "-x", "256" },
            new [] { "--version", "--stringvalue", "foo", "-x", "256" },
            new [] { "--version", "--stringvalue=foo", "-x", "256" },
        };

        public static object[][] InvalidArgsData = new []
        {
            new [] { "--version", "foo" },
            new [] { "--version", "-s" },
            new [] { "--version", "-i", "foo" },
            new [] {"--version", "--invalid-switch", "foo" },
            new [] {"--invalid-switch", "foo", "--version" },
            new [] {"--invalid-switch", "--version", "foo" },
        };

        public static object[][] ConsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--version" },
            new [] { "--stringvalue=--version" },
            new [] { "--stringvalue", "--version", "-s", "--version" },
            new [] { "--stringvalue=--version", "-s", "--version" },
            new [] { "--stringvalue", "--version", "-s--version" },
            new [] { "--stringvalue=--version", "-s--version" },
        };

        public static object[][] MixOfConsumedAndUnconsumedDashDashHelpValidArgsData = new []
        {
            new [] { "--stringvalue", "--version", "--version" },
            new [] { "--version", "--stringvalue", "--version" },
            new [] { "--stringvalue=--version", "--version" },
            new [] { "--version", "--stringvalue=--version" },
            new [] { "--stringvalue", "--version", "-s", "--version", "--version" },
            new [] { "--stringvalue", "--version", "--version", "-s", "--version" },
            new [] { "--version", "--stringvalue", "--version", "-s", "--version" },
            new [] { "--stringvalue=--version", "-s", "--version", "--version" },
            new [] { "--stringvalue=--version", "--version", "-s", "--version" },
            new [] { "--version", "--stringvalue=--version", "-s", "--version" },
            new [] { "--stringvalue", "--version", "-s--version", "--version" },
            new [] { "--stringvalue", "--version", "--version", "-s--version", "--version" },
            new [] { "--version", "--stringvalue", "--version", "-s--version" },
            new [] { "--stringvalue=--version", "-s--version", "--version" },
            new [] { "--stringvalue=--version", "--version", "-s--version" },
            new [] { "--version", "--stringvalue=--version", "-s--version" },
        };

        public static object[][] ConsumedDashDashHelpInvalidArgsData = new []
        {
            new [] { "--stringvalue", "--version", "foo" },
            new [] { "-s", "--version", "--stringvalue" },
            new [] { "-s", "--version", "-i", "foo" },
            new [] { "--stringvalue", "--version", "--invalid-switch", "256" },
            new [] { "--stringvalue=--version", "--invalid-switch", "256" },
            new [] { "--invalid-switch", "-s", "--version" },
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
        public void Explicit_version_command_with_valid_args_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        public static IEnumerable<object[]> ValidArgsDataWithShortOption =
            ConvertDataToShortOption(ValidArgsData, "--version", "-V");

        [Theory]
        [MemberData(nameof(ValidArgsDataWithShortOption))]
        public void Explicit_version_command_with_valid_args_and_short_option_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgsData))]
        public void Explicit_version_command_with_invalid_args_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        public static IEnumerable<object[]> InvalidArgsDataWithShortOption =
            ConvertDataToShortOption(InvalidArgsData, "--version", "-V");

        [Theory]
        [MemberData(nameof(InvalidArgsDataWithShortOption))]
        public void Explicit_version_command_with_invalid_args_and_short_option_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsData))]
        public void Dash_dash_help_in_a_string_value_does_not_produce_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("--version");
            shortAndLong.Should().BeOneOf("--version", null, "");
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpValidArgsData, "--version", "-V");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsDataWithShortOption))]
        public void Dash_dash_help_in_a_string_value_with_short_option_does_not_produce_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("-V");
            shortAndLong.Should().BeOneOf("-V", null, "");
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpValidArgsDataWithShortOptionInData =
            ConvertDataToShortOption(ConsumedDashDashHelpValidArgsData, "--version", "h");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpValidArgsDataWithShortOptionInData))]
        public void Dash_dash_help_in_a_string_value_with_short_option_in_data_does_not_produce_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            // result.Should().BeOfType<NotParsed<Simple_Options>>();
            // result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            // result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
            result.Should().BeOfType<Parsed<Simple_Options>>();
            string stringValue = result.As<Parsed<Simple_Options>>().Value.StringValue;
            string shortAndLong = result.As<Parsed<Simple_Options>>().Value.ShortAndLong;
            stringValue.Should().Be("h");
            shortAndLong.Should().BeOneOf("h", null, "");
        }

        [Theory]
        [MemberData(nameof(MixOfConsumedAndUnconsumedDashDashHelpValidArgsData))]
        public void Explicit_version_command_mixed_with_some_consumed_args_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        public static IEnumerable<object[]> MixOfConsumedAndUnconsumedDashDashHelpValidArgsDataWithShortOption =
            ConvertDataToShortOption(MixOfConsumedAndUnconsumedDashDashHelpValidArgsData, "--version", "-V");

        [Theory]
        [MemberData(nameof(MixOfConsumedAndUnconsumedDashDashHelpValidArgsDataWithShortOption))]
        public void Explicit_short_version_command_mixed_with_some_consumed_args_produces_just_one_VersionRequestedError(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCount(x => x == 1);
            result.As<NotParsed<Simple_Options>>().Errors.First().Should().BeOfType<VersionRequestedError>();
        }

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpInvalidArgsData))]
        public void Dash_dash_help_consumed_by_valid_args_with_invalid_args_produces_no_VersionRequestedErrors(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCountGreaterOrEqualTo(1);
            result.As<NotParsed<Simple_Options>>().Errors.Should().NotBeOfType<VersionRequestedError>();
        }

        public static IEnumerable<object[]> ConsumedDashDashHelpInvalidArgsDataWithShortOption =
            ConvertDataToShortOption(ConsumedDashDashHelpInvalidArgsData, "--version", "-V");

        [Theory]
        [MemberData(nameof(ConsumedDashDashHelpInvalidArgsDataWithShortOption))]
        public void Dash_h_consumed_by_valid_args_with_invalid_args_produces_no_VersionRequestedErrors(params string[] args)
        {
            // Arrange
            var help = new StringWriter();
            var sut = new Parser(config => {
                config.AutoVersion = true;
                config.AutoVersionShortName = true;
                config.HelpWriter = help;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options>(args);

            // Assert
            result.Should().BeOfType<NotParsed<Simple_Options>>();
            result.As<NotParsed<Simple_Options>>().Errors.Should().HaveCountGreaterOrEqualTo(1);
            result.As<NotParsed<Simple_Options>>().Errors.Should().NotBeOfType<VersionRequestedError>();
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
        public void Explicit_version_request_with_AutoVersion_off_generates_unknown_option_error()
        {
            // Fixture setup
            var expectedError = new UnknownOptionError("version");
            var sut = new Parser(config => { config.AutoVersion = false; });

            // Exercise system
            var result = sut.ParseArguments<Simple_Options>(new[] { "--version" });

            // Verify outcome
            ((NotParsed<Simple_Options>)result).Errors.Should().HaveCount(x => x == 1);
            ((NotParsed<Simple_Options>)result).Errors.Single().Tag.Should().Be(expectedError.Tag);
            ((NotParsed<Simple_Options>)result).Errors.First().As<UnknownOptionError>().Token.Should().BeEquivalentTo(expectedError.Token);

            // Teardown
        }

        [Fact]
        public void Explicit_version_request_with_AutoVersion_off_displays_unknown_option_error()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => { config.AutoVersion = false; config.HelpWriter = help; });

            // Exercise system
            sut.ParseArguments<Simple_Options>(new[] { "--version" });
            var result = help.ToString();

            // Verify outcome

            // Verify outcome
            result.Length.Should().BeGreaterThan(0);
            var lines = result.ToNotEmptyLines().TrimStringArray();
            lines[0].Should().Be(CommandLine.Text.HeadingInfo.Default.ToString());
            lines[1].Should().Be(CommandLine.Text.CopyrightInfo.Default.ToString());
            lines[2].Should().BeEquivalentTo("ERROR(S):");
            lines[3].Should().BeEquivalentTo("Option 'version' is unknown.");

            // Teardown
        }

        [Fact]
        public void Explicit_version_request_with_AutoVersion_off_and_IgnoreUnknownArguments_on_does_not_generate_version_screen()
        {
            // Fixture setup
            var help = new StringWriter();
            var sut = new Parser(config => { config.HelpWriter = help; config.AutoVersion = false; config.IgnoreUnknownArguments = true; });

            // Exercize system
            sut.ParseArguments<Simple_Options>(new[] { "--version" });
            var result = help.ToString();

            // Verify outcome
            result.Should().BeEquivalentTo("");
            // Teardown
        }
    }
}
