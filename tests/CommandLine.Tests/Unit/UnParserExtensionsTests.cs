// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
#if !SKIP_FSHARP
using Microsoft.FSharp.Core;
#endif
 
namespace CommandLine.Tests.Unit
{
    public class UnParserExtensionsTests
    {
        [Theory]
        [MemberData(nameof(UnParseData))]
        public static void UnParsing_instance_returns_command_line(Simple_Options options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(result);
        }

        [Theory]
        [MemberData(nameof(UnParseData))]
        public static void UnParsing_instance_with_splitArgs_returns_same_option_class(Simple_Options options, string result)
        {
           new Parser()
            .FormatCommandLineArgs(options)
            .Should().BeEquivalentTo(result.SplitArgs());
             
        }

        [Theory]
        [MemberData(nameof(UnParseFileDirectoryData))]
        public static void UnParsing_instance_returns_command_line_for_file_directory_paths(Options_With_FileDirectoryInfo options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(result);
        }

        [Theory]
        [MemberData(nameof(UnParseFileDirectoryData))]
        public static void UnParsing_instance_by_splitArgs_returns_command_line_for_file_directory_paths(Options_With_FileDirectoryInfo options, string result)
        {
            new Parser()
                .FormatCommandLineArgs(options)
                .Should().BeEquivalentTo(result.SplitArgs());
        }
        [Theory]
        [MemberData(nameof(UnParseDataVerbs))]
        public static void UnParsing_instance_returns_command_line_for_verbs(Add_Verb verb, string result)
        {
            new Parser()
                .FormatCommandLine(verb)
                .Should().BeEquivalentTo(result);
        }

        [Theory]
        [MemberData(nameof(UnParseDataVerbs))]
        public static void UnParsing_instance_to_splitArgs_returns_command_line_for_verbs(Add_Verb verb, string result)
        {
            new Parser()
                .FormatCommandLineArgs(verb)
                .Should().BeEquivalentTo(result.SplitArgs());
        }

        [Theory]
        [MemberData(nameof(UnParseDataImmutable))]
        public static void UnParsing_immutable_instance_returns_command_line(Immutable_Simple_Options options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(result);
        }

        [Theory]
        [MemberData(nameof(UnParseDataHidden))]
        public static void Unparsing_hidden_option_returns_command_line(Hidden_Option options, bool showHidden, string result)
        {
            new Parser()
                .FormatCommandLine(options, config => config.ShowHidden = showHidden)
                .Should().BeEquivalentTo(result);
        }

#if !SKIP_FSHARP
        [Theory]
        [MemberData(nameof(UnParseDataFSharpOption))]
        public static void UnParsing_instance_with_fsharp_option_returns_command_line(Options_With_FSharpOption options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(result);
        }
#endif

        [Fact]
        public static void UnParsing_instance_with_group_switches_returns_command_line_with_switches_grouped()
        {
            var options = new Options_With_Switches { InputFile = "input.bin", HumanReadable = true, IgnoreWarnings = true };
            new Parser()
                .FormatCommandLine(options, config => config.GroupSwitches = true)
                .Should().BeEquivalentTo("-hi --input input.bin");
        }

        [Fact]
        public static void UnParsing_instance_with_equal_token_returns_command_line_with_long_option_using_equal_sign()
        {
            var options = new Simple_Options { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 };
            new Parser()
                .FormatCommandLine(options, config => config.UseEqualToken = true)
                .Should().BeEquivalentTo("-i 1 2 3 --stringvalue=nospaces -x 123456789");
        }

        [Fact]
        public static void UnParsing_instance_with_dash_in_value_and_dashdash_enabled_returns_command_line_with_value_prefixed_with_dash_dash()
        {
            var options = new Simple_Options_With_Values { StringSequence = new List<string> { "-something", "with", "dash" } };
            new Parser((setting) => setting.EnableDashDash = true)
                .FormatCommandLine(options)
                .Should().BeEquivalentTo("-- -something with dash");
        }

        [Fact]
        public static void UnParsing_instance_with_no_values_and_dashdash_enabled_returns_command_line_without_dash_dash()
        {
            var options = new Simple_Options_With_Values();
            new Parser((setting) => setting.EnableDashDash = true)
                .FormatCommandLine(options)
                .Should().BeEquivalentTo("");
        }

        [Fact]
        public static void UnParsing_instance_with_dash_in_value_and_dashdash_disabled_returns_command_line_with_value()
        {
            var options = new Simple_Options_With_Values { StringSequence = new List<string> { "-something", "with", "dash" } };
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo("-something with dash");
        }

        #region Issue 579
        [Fact]
        public static void UnParsing_instance_with_TimeSpan_returns_the_value_unquoted_in_command_line()
        {
            var options = new Options_With_TimeSpan { Duration = TimeSpan.FromMinutes(1) };
            new Parser()
                .FormatCommandLine(options)
                .Should().Be("--duration 00:01:00");
        }
        #endregion

        #region PR 550

        [Fact]
        public static void UnParsing_instance_with_default_values_when_skip_default_is_false()
        {
            var options = new Options_With_Defaults { P2 = "xyz", P1 = 99, P3 = 88, P4 = Shapes.Square };
            new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo("--p1 99 --p2 xyz --p3 88 --p4 Square");
        }

        [Theory]
        [InlineData(true, "--p2 xyz")]
        [InlineData(false, "--p1 99 --p2 xyz --p3 88 --p4 Square")]
        public static void UnParsing_instance_with_default_values_when_skip_default_is_true(bool skipDefault, string expected)
        {
            var options = new Options_With_Defaults { P2 = "xyz", P1 = 99, P3 = 88, P4 = Shapes.Square };
            new Parser()
                .FormatCommandLine(options, x => x.SkipDefault = skipDefault)
                .Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(true, "--p2 xyz")]
        [InlineData(false, "--p1 99 --p2 xyz --p3 88 --p4 Square")]
        public static void UnParsing_instance_with_nullable_default_values_when_skip_default_is_true(bool skipDefault, string expected)
        {
            var options = new Nuulable_Options_With_Defaults { P2 = "xyz", P1 = 99, P3 = 88, P4 = Shapes.Square };
            new Parser()
                .FormatCommandLine(options, x => x.SkipDefault = skipDefault)
                .Should().BeEquivalentTo(expected);
        }
        [Fact]
        public static void UnParsing_instance_with_datetime()
        {
            var date = new DateTime(2019, 5, 1);
            var options = new Options_Date { Start = date };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().MatchRegex("--start\\s\".+\"");
        }

        [Fact]
        public static void UnParsing_instance_with_datetime_nullable()
        {
            var date = new DateTime(2019, 5, 1);
            var options = new Options_Date_Nullable { Start = date };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().MatchRegex("--start\\s\".+\"");
        }

        [Fact]
        public static void UnParsing_instance_with_datetime_offset()
        {
            DateTimeOffset date = new DateTime(2019, 5, 1);
            var options = new Options_DateTimeOffset { Start = date };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().MatchRegex("--start\\s\".+\"");
        }

        [Fact]
        public static void UnParsing_instance_with_timespan()
        {
            var ts = new TimeSpan(1,2,3);
            var options = new Options_TimeSpan { Start = ts };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo("--start 01:02:03"); //changed for issue 579
        }

        [Theory]
        [InlineData(false, 0, "")] //default behaviour based on type
        [InlineData(false, 1, "-v 1")]  //default skip=false
        [InlineData(false, 2, "-v 2")]
        [InlineData(true, 1, "")]  //default skip=true
        public static void UnParsing_instance_with_int(bool skipDefault, int value, string expected)
        {
            var options = new Option_Int { VerboseLevel = value };
            var result = new Parser()
                .FormatCommandLine(options, x => x.SkipDefault = skipDefault)
                .Should().BeEquivalentTo(expected);

        }

        [Theory]
        [InlineData(false, 0, "-v 0")]
        [InlineData(false, 1, "-v 1")]  //default
        [InlineData(false, 2, "-v 2")]
        [InlineData(false, null, "")]
        [InlineData(true, 1, "")]  //default
        public static void UnParsing_instance_with_int_nullable(bool skipDefault, int? value, string expected)
        {
            var options = new Option_Int_Nullable { VerboseLevel = value };
            var result = new Parser()
                .FormatCommandLine(options, x => x.SkipDefault = skipDefault)
                .Should().BeEquivalentTo(expected);

        }

        [Theory]
        [InlineData(false, false, 0, "")]
        [InlineData(false, false, 1, "-v")]  // default but not skipped
        [InlineData(false, false, 2, "-v -v")]
        [InlineData(false, true, 2, "-vv")]
        [InlineData(false, false, 3, "-v -v -v")]
        [InlineData(false, true, 3, "-vvv")]
        [InlineData(true, false, 1, "")]  // default, skipped
        public static void UnParsing_instance_with_flag_counter(bool skipDefault, bool groupSwitches, int value, string expected)
        {
            var options = new Option_FlagCounter { VerboseLevel = value };
            var result = new Parser()
                .FormatCommandLine(options, x => { x.SkipDefault = skipDefault; x.GroupSwitches = groupSwitches; })
                .Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(Shapes.Circle, "--shape Circle")]
        [InlineData(Shapes.Square, "--shape Square")]
        [InlineData(null, "")]
        public static void UnParsing_instance_with_nullable_enum(Shapes? shape, string expected)
        {
            var options = new Option_Nullable_Enum { Shape = shape };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(true, "-v True")]
        [InlineData(false, "-v False")]
        [InlineData(null, "")]
        public static void UnParsing_instance_with_nullable_bool(bool? flag, string expected)
        {
            var options = new Option_Nullable_Bool { Verbose = flag };
            var result = new Parser()
                .FormatCommandLine(options)
                .Should().BeEquivalentTo(expected);
        }
        #region SplitArgs
        [Theory]
        [InlineData("--shape Circle", new[] { "--shape","Circle" })]
        [InlineData("  --shape     Circle  ", new[] { "--shape", "Circle" })]
        [InlineData("-a --shape Circle", new[] {"-a", "--shape", "Circle" })]
        [InlineData("-a --shape Circle -- -x1 -x2", new[] { "-a", "--shape", "Circle","--","-x1","-x2" })]
        [InlineData("--name \"name with space and quote\" -x1", new[] { "--name", "name with space and quote","-x1" })]       
        public static void Split_arguments(string command, string[] expectedArgs)
        {
            var args = command.SplitArgs();
            args.Should().BeEquivalentTo(expectedArgs);
        }
        [Theory]
        [InlineData("--shape Circle", new[] { "--shape", "Circle" })]
        [InlineData("  --shape     Circle  ", new[] { "--shape", "Circle" })]
        [InlineData("-a --shape Circle", new[] { "-a", "--shape", "Circle" })]
        [InlineData("-a --shape Circle -- -x1 -x2", new[] { "-a", "--shape", "Circle", "--", "-x1", "-x2" })]
        [InlineData("--name \"name with space and quote\" -x1", new[] { "--name", "\"name with space and quote\"", "-x1" })]
        public static void Split_arguments_with_keep_quote(string command, string[] expectedArgs)
        {
            var args = command.SplitArgs(true);
            args.Should().BeEquivalentTo(expectedArgs);
        }
        #endregion
        class Option_Int_Nullable
        {
            [Option('v', Default = 1)]
            public int? VerboseLevel { get; set; }
        }
        class Option_Int
        {
            [Option('v', Default = 1)]
            public int VerboseLevel { get; set; }
        }
        class Option_FlagCounter
        {
            [Option('v', Default = 1, FlagCounter=true)]
            public int VerboseLevel { get; set; }
        }
        class Option_Nullable_Bool
        {
            [Option('v')]
            public bool? Verbose { get; set; }
        }
        class Option_Nullable_Enum
        {
            [Option]
            public Shapes? Shape { get; set; }
        }
        class Options_Date
        {
            [Option]
            public DateTime Start { get; set; }
        }
        class Options_Date_Nullable
        {
            [Option]
            public DateTime? Start { get; set; }
        }
        class Options_TimeSpan
        {
            [Option]
            public TimeSpan Start { get; set; }
        }
        class Options_DateTimeOffset
        {
            [Option]
            public DateTimeOffset Start { get; set; }
        }
        #endregion
        public static IEnumerable<object[]> UnParseData
        {
            get
            {
                yield return new object[] { new Simple_Options(), "" };
                yield return new object[] { new Simple_Options { BoolValue = true }, "-x" };
                yield return new object[] { new Simple_Options { IntSequence = new[] { 1, 2, 3 } }, "-i 1 2 3" };
                yield return new object[] { new Simple_Options { StringValue = "nospaces" }, "--stringvalue nospaces" };
                yield return new object[] { new Simple_Options { StringValue = " with spaces " }, "--stringvalue \" with spaces \"" };
                yield return new object[] { new Simple_Options { StringValue = "with\"quote" }, "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new Simple_Options { StringValue = "with \"quotes\" spaced" }, "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new Simple_Options { LongValue = 123456789 }, "123456789" };
                yield return new object[] { new Simple_Options { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 }, "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new Simple_Options { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "with \"quotes\" spaced", LongValue = 123456789 }, "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };
            }
        }

        public static IEnumerable<object[]> UnParseFileDirectoryData
        {
            get
            {
                yield return new object[] { new Options_With_FileDirectoryInfo(), "" };
                yield return new object[] { new Options_With_FileDirectoryInfo { FilePath = new FileInfo(@"C:\my path\with spaces\file with spaces.txt"), DirectoryPath = new DirectoryInfo(@"C:\my path\with spaces\"), StringPath = @"C:\my path\with spaces\file with spaces.txt" }, @"--directoryPath ""C:\my path\with spaces\"" --filePath ""C:\my path\with spaces\file with spaces.txt"" --stringPath ""C:\my path\with spaces\file with spaces.txt""" };
            }
        }


        public static IEnumerable<object[]> UnParseDataVerbs
        {
            get
            {
                yield return new object[] { new Add_Verb(), "add" };
                yield return new object[] { new Add_Verb { Patch = true, FileName = "mysource.cs" }, "add --patch mysource.cs" };
                yield return new object[] { new Add_Verb { Force = true, FileName = "mysource.fs" }, "add --force mysource.fs" };
            }
        }

        public static IEnumerable<object[]> UnParseDataImmutable
        {
            get
            {
                yield return new object[] { new Immutable_Simple_Options("", Enumerable.Empty<int>(), default(bool), default(long)), "" };
                yield return new object[] { new Immutable_Simple_Options("", Enumerable.Empty<int>(), true, default(long)), "-x" };
                yield return new object[] { new Immutable_Simple_Options("", new[] { 1, 2, 3 }, default(bool), default(long)), "-i 1 2 3" };
                yield return new object[] { new Immutable_Simple_Options("nospaces", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue nospaces" };
                yield return new object[] { new Immutable_Simple_Options(" with spaces ", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \" with spaces \"" };
                yield return new object[] { new Immutable_Simple_Options("with\"quote", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new Immutable_Simple_Options("with \"quotes\" spaced", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new Immutable_Simple_Options("", Enumerable.Empty<int>(), default(bool), 123456789), "123456789" };
                yield return new object[] { new Immutable_Simple_Options("nospaces", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new Immutable_Simple_Options("with \"quotes\" spaced", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };
            }
        }

        public static IEnumerable<object[]> UnParseDataHidden
        {
            get
            {
                yield return new object[] { new Hidden_Option { HiddenOption = "hidden" }, true, "--hiddenOption hidden" };
                yield return new object[] { new Hidden_Option { HiddenOption = "hidden" }, false, "" };
            }
        }
#if !SKIP_FSHARP
        public static IEnumerable<object[]> UnParseDataFSharpOption
        {
            get
            {
                yield return new object[] { new Options_With_FSharpOption(), "" };
                yield return new object[] { new Options_With_FSharpOption { FileName = FSharpOption<string>.Some("myfile.bin") }, "--filename myfile.bin" };
                yield return new object[] { new Options_With_FSharpOption { Offset = FSharpOption<int>.Some(123456789) }, "123456789" };
                yield return new object[] { new Options_With_FSharpOption { FileName = FSharpOption<string>.Some("myfile.bin"), Offset = FSharpOption<int>.Some(123456789) }, "--filename myfile.bin 123456789" };
            }
        }
#endif
    }
}
