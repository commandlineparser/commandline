// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;
#if !SKIP_FSHARP
using Microsoft.FSharp.Core;
#endif

namespace CommandLine.Tests.Unit
{
    public class UnParserExtensionsTests
    {
        [Theory]
        [MemberData("UnParseData")]
        public static void UnParsing_instance_returns_command_line(Simple_Options options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        [Theory]
        [MemberData("UnParseDataVerbs")]
        public static void UnParsing_instance_returns_command_line_for_verbs(Add_Verb verb, string result)
        {
            new Parser()
                .FormatCommandLine(verb)
                .ShouldBeEquivalentTo(result);
        }

        [Theory]
        [MemberData("UnParseDataImmutable")]
        public static void UnParsing_immutable_instance_returns_command_line(Immutable_Simple_Options options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

#if !SKIP_FSHARP
        [Theory]
        [MemberData("UnParseDataFSharpOption")]
        public static void UnParsing_instance_with_fsharp_option_returns_command_line(Options_With_FSharpOption options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }
#endif

        [Fact]
        public static void UnParsing_instance_with_group_switches_returns_command_line_with_switches_grouped()
        {
            var options = new Options_With_Switches { InputFile = "input.bin", HumanReadable = true, IgnoreWarnings = true };
            new Parser()
                .FormatCommandLine(options, config => config.GroupSwitches = true)
                .ShouldBeEquivalentTo("-hi --input input.bin");
        }

        [Fact]
        public static void UnParsing_instance_with_equal_token_returns_command_line_with_long_option_using_equal_sign()
        {
            var options = new Simple_Options { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 };
            new Parser()
                .FormatCommandLine(options, config => config.UseEqualToken = true)
                .ShouldBeEquivalentTo("-i 1 2 3 --stringvalue=nospaces -x 123456789");
        }

        [Fact]
        public static void UnParsing_instance_with_dash_in_value_and_dashdash_enabled_returns_command_line_with_value_prefixed_with_dash_dash()
        {
            var options = new Simple_Options_With_Values { StringSequence = new List<string> { "-something", "with", "dash" } };
            new Parser((setting) => setting.EnableDashDash = true)
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo("-- -something with dash");
        }

        [Fact]
        public static void UnParsing_instance_with_no_values_and_dashdash_enabled_returns_command_line_without_dash_dash()
        {
            var options = new Simple_Options_With_Values();
            new Parser((setting) => setting.EnableDashDash = true)
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo("");
        }

        [Fact]
        public static void UnParsing_instance_with_dash_in_value_and_dashdash_disabled_returns_command_line_with_value()
        {
            var options = new Simple_Options_With_Values { StringSequence = new List<string> { "-something", "with", "dash" } };
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo("-something with dash");
        }

        public static IEnumerable<object> UnParseData
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


        public static IEnumerable<object> UnParseDataVerbs
        {
            get
            {
                yield return new object[] { new Add_Verb(), "add" };
                yield return new object[] { new Add_Verb { Patch = true, FileName = "mysource.cs" }, "add --patch mysource.cs" };
                yield return new object[] { new Add_Verb { Force = true, FileName = "mysource.fs" }, "add --force mysource.fs" };
            }
        }

        public static IEnumerable<object> UnParseDataImmutable
        {
            get
            {
                yield return new object[] { new Immutable_Simple_Options("", Enumerable.Empty<int>(), default(bool), default(long)), "" };
                yield return new object[] { new Immutable_Simple_Options ("", Enumerable.Empty<int>(), true, default(long) ), "-x" };
                yield return new object[] { new Immutable_Simple_Options ("", new[] { 1, 2, 3 }, default(bool), default(long) ), "-i 1 2 3" };
                yield return new object[] { new Immutable_Simple_Options ("nospaces", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue nospaces" };
                yield return new object[] { new Immutable_Simple_Options (" with spaces ", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \" with spaces \"" };
                yield return new object[] { new Immutable_Simple_Options ("with\"quote", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new Immutable_Simple_Options ("with \"quotes\" spaced", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new Immutable_Simple_Options ("", Enumerable.Empty<int>(), default(bool), 123456789), "123456789" };
                yield return new object[] { new Immutable_Simple_Options ("nospaces", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new Immutable_Simple_Options ("with \"quotes\" spaced", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };
            }
        }

#if !SKIP_FSHARP
        public static IEnumerable<object> UnParseDataFSharpOption
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
