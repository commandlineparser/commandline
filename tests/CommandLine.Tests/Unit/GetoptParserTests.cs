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
    public class GetoptParserTests
    {
        public GetoptParserTests()
        {
        }

        public class SimpleArgsData : TheoryData<string[], Simple_Options_WithExtraArgs>
        {
            public SimpleArgsData()
            {
                // Options and values can be mixed by default
                Add(new string [] { "--stringvalue=foo", "-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        IntSequence = Enumerable.Empty<int>(),
                        ShortAndLong = null,
                        StringValue = "foo",
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] { "256", "--stringvalue=foo", "-x" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = Enumerable.Empty<int>(),
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] {"--stringvalue=foo", "256", "-x" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = Enumerable.Empty<int>(),
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });

                // Sequences end at first non-value arg even if they haven't yet consumed their max
                Add(new string [] {"--stringvalue=foo", "-i1", "2", "3", "-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = new[] { 1, 2, 3 },
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                // Sequences also end after consuming their max even if there would be more parameters
                Add(new string [] {"--stringvalue=foo", "-i1", "2", "3", "4", "256", "-x" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = new[] { 1, 2, 3, 4 },
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });

                // The special -- option, if not consumed, turns off further option processing
                Add(new string [] {"--stringvalue", "foo", "256", "-x", "-sbar" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = "bar",
                        BoolValue = true,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] {"--stringvalue", "foo", "--", "256", "-x", "-sbar" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        BoolValue = false,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = new [] { "-x", "-sbar" },
                    });

                // But if -- is specified as a value following an equal sign, it has no special meaning
                Add(new string [] {"--stringvalue=--", "256", "-x", "-sbar" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "--",
                        ShortAndLong = "bar",
                        BoolValue = true,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });

                // Options that take values will take the next arg whatever it looks like
                Add(new string [] {"--stringvalue", "-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "-x",
                        BoolValue = false,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] {"--stringvalue", "-x", "-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "-x",
                        BoolValue = true,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });

                // That applies even if the next arg is -- which would normally stop option processing: if it's after an option that takes a value, it's consumed as the value
                Add(new string [] {"--stringvalue", "--", "256", "-x", "-sbar" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "--",
                        ShortAndLong = "bar",
                        BoolValue = true,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });

                // Options that take values will not swallow the next arg if a value was specified with =
                Add(new string [] {"--stringvalue=-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "-x",
                        BoolValue = false,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] {"--stringvalue=-x", "-x", "256" },
                    new Simple_Options_WithExtraArgs {
                        StringValue = "-x",
                        BoolValue = true,
                        LongValue = 256,
                        IntSequence = Enumerable.Empty<int>(),
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
            }
        }

        [Theory]
        [ClassData(typeof(SimpleArgsData))]
        public void Getopt_parser_without_posixly_correct_allows_mixed_options_and_nonoptions(string[] args, Simple_Options_WithExtraArgs expected)
        {
            // Arrange
            var sut = new Parser(config => {
                config.GetoptMode = true;
                config.PosixlyCorrect = false;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options_WithExtraArgs>(args);

            // Assert
            if (result is Parsed<Simple_Options_WithExtraArgs> parsed) {
                parsed.Value.Should().BeEquivalentTo(expected);
            } else if (result is NotParsed<Simple_Options_WithExtraArgs> notParsed) {
                Console.WriteLine(String.Join(", ", notParsed.Errors.Select(err => err.Tag.ToString())));
            }
            result.Should().BeOfType<Parsed<Simple_Options_WithExtraArgs>>();
            result.As<Parsed<Simple_Options_WithExtraArgs>>().Value.Should().BeEquivalentTo(expected);
        }

        public class SimpleArgsDataWithPosixlyCorrect : TheoryData<string[], Simple_Options_WithExtraArgs>
        {
            public SimpleArgsDataWithPosixlyCorrect()
            {
                Add(new string [] { "--stringvalue=foo", "-x", "256" },
                    // Parses all options
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = Enumerable.Empty<int>(),
                        BoolValue = true,
                        LongValue = 256,
                        ExtraArgs = Enumerable.Empty<string>(),
                    });
                Add(new string [] { "256", "--stringvalue=foo", "-x" },
                    // Stops parsing after "256", so StringValue and BoolValue not set
                    new Simple_Options_WithExtraArgs {
                        StringValue = null,
                        ShortAndLong = null,
                        IntSequence = Enumerable.Empty<int>(),
                        BoolValue = false,
                        LongValue = 256,
                        ExtraArgs = new string[] { "--stringvalue=foo", "-x" },
                    });
                Add(new string [] {"--stringvalue=foo", "256", "-x" },
                    // Stops parsing after "256", so StringValue is set but BoolValue is not
                    new Simple_Options_WithExtraArgs {
                        StringValue = "foo",
                        ShortAndLong = null,
                        IntSequence = Enumerable.Empty<int>(),
                        BoolValue = false,
                        LongValue = 256,
                        ExtraArgs = new string[] { "-x" },
                    });
            }
        }

        [Theory]
        [ClassData(typeof(SimpleArgsDataWithPosixlyCorrect))]
        public void Getopt_parser_with_posixly_correct_stops_parsing_at_first_nonoption(string[] args, Simple_Options_WithExtraArgs expected)
        {
            // Arrange
            var sut = new Parser(config => {
                config.GetoptMode = true;
                config.PosixlyCorrect = true;
                config.EnableDashDash = true;
            });

            // Act
            var result = sut.ParseArguments<Simple_Options_WithExtraArgs>(args);

            // Assert
            result.Should().BeOfType<Parsed<Simple_Options_WithExtraArgs>>();
            result.As<Parsed<Simple_Options_WithExtraArgs>>().Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Getopt_mode_defaults_to_EnableDashDash_being_true()
        {
            // Arrange
            var sut = new Parser(config => {
                config.GetoptMode = true;
                config.PosixlyCorrect = false;
            });
            var args = new string [] {"--stringvalue", "foo", "256", "--", "-x", "-sbar" };
            var expected = new Simple_Options_WithExtraArgs {
                    StringValue = "foo",
                    ShortAndLong = null,
                    BoolValue = false,
                    LongValue = 256,
                    IntSequence = Enumerable.Empty<int>(),
                    ExtraArgs = new [] { "-x", "-sbar" },
                };

            // Act
            var result = sut.ParseArguments<Simple_Options_WithExtraArgs>(args);

            // Assert
            result.Should().BeOfType<Parsed<Simple_Options_WithExtraArgs>>();
            result.As<Parsed<Simple_Options_WithExtraArgs>>().Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Getopt_mode_can_have_EnableDashDash_expicitly_disabled()
        {
            // Arrange
            var sut = new Parser(config => {
                config.GetoptMode = true;
                config.PosixlyCorrect = false;
                config.EnableDashDash = false;
            });
            var args = new string [] {"--stringvalue", "foo", "256", "--", "-x", "-sbar" };
            var expected = new Simple_Options_WithExtraArgs {
                    StringValue = "foo",
                    ShortAndLong = "bar",
                    BoolValue = true,
                    LongValue = 256,
                    IntSequence = Enumerable.Empty<int>(),
                    ExtraArgs = new [] { "--" },
                };

            // Act
            var result = sut.ParseArguments<Simple_Options_WithExtraArgs>(args);

            // Assert
            result.Should().BeOfType<Parsed<Simple_Options_WithExtraArgs>>();
            result.As<Parsed<Simple_Options_WithExtraArgs>>().Value.Should().BeEquivalentTo(expected);
        }
    }
}
