// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;
using Microsoft.FSharp.Core;

namespace CommandLine.Tests.Unit
{
    public class UnParserExtensionsTests
    {
        [Theory]
        [MemberData("UnParseData")]
        public static void UnParsing_instance_returns_command_line(FakeOptions options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        [Theory]
        [MemberData("UnParseDataVerbs")]
        public static void UnParsing_instance_returns_command_line_for_verbs(AddOptions options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        [Theory]
        [MemberData("UnParseDataImmutable")]
        public static void UnParsing_immutable_instance_returns_command_line(FakeImmutableOptions options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        [Theory]
        [MemberData("UnParseDataFSharpOption")]
        public static void UnParsing_instance_with_fsharp_option_returns_command_line(FakeOptionsWithFSharpOption options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        [Fact]
        public static void UnParsing_instance_with_group_switches_returns_command_line_with_switches_grouped()
        {
            var options = new FakeOptionsWithSwitches { InputFile = "input.bin", HumanReadable = true, IgnoreWarnings = true };
            new Parser()
                .FormatCommandLine(options, config => config.GroupSwitches = true)
                .ShouldBeEquivalentTo("-hi --input input.bin");
        }

        [Fact]
        public static void UnParsing_instance_with_equal_token_returns_command_line_with_long_option_using_equal_sign()
        {
            var options = new FakeOptions { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 };
            new Parser()
                .FormatCommandLine(options, config => config.UseEqualToken = true)
                .ShouldBeEquivalentTo("-i 1 2 3 --stringvalue=nospaces -x 123456789");
        }

        public static IEnumerable<object> UnParseData
        {
            get
            {
                yield return new object[] { new FakeOptions(), "" };
                yield return new object[] { new FakeOptions { BoolValue = true }, "-x" };
                yield return new object[] { new FakeOptions { IntSequence = new[] { 1, 2, 3 } }, "-i 1 2 3" };
                yield return new object[] { new FakeOptions { StringValue = "nospaces" }, "--stringvalue nospaces" };
                yield return new object[] { new FakeOptions { StringValue = " with spaces " }, "--stringvalue \" with spaces \"" };
                yield return new object[] { new FakeOptions { StringValue = "with\"quote" }, "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new FakeOptions { StringValue = "with \"quotes\" spaced" }, "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new FakeOptions { LongValue = 123456789 }, "123456789" };
                yield return new object[] { new FakeOptions { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 }, "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new FakeOptions { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "with \"quotes\" spaced", LongValue = 123456789 }, "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };
            }
        }


        public static IEnumerable<object> UnParseDataVerbs
        {
            get
            {
                yield return new object[] { new AddOptions(), "add" };
                yield return new object[] { new AddOptions { Patch = true, FileName = "mysource.cs" }, "add --patch mysource.cs" };
                yield return new object[] { new AddOptions { Force = true, FileName = "mysource.fs" }, "add --force mysource.fs" };
            }
        }

        public static IEnumerable<object> UnParseDataImmutable
        {
            get
            {
                yield return new object[] { new FakeImmutableOptions("", Enumerable.Empty<int>(), default(bool), default(long)), "" };
                yield return new object[] { new FakeImmutableOptions ("", Enumerable.Empty<int>(), true, default(long) ), "-x" };
                yield return new object[] { new FakeImmutableOptions ("", new[] { 1, 2, 3 }, default(bool), default(long) ), "-i 1 2 3" };
                yield return new object[] { new FakeImmutableOptions ("nospaces", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue nospaces" };
                yield return new object[] { new FakeImmutableOptions (" with spaces ", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \" with spaces \"" };
                yield return new object[] { new FakeImmutableOptions ("with\"quote", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new FakeImmutableOptions ("with \"quotes\" spaced", Enumerable.Empty<int>(), default(bool), default(long)), "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new FakeImmutableOptions ("", Enumerable.Empty<int>(), default(bool), 123456789), "123456789" };
                yield return new object[] { new FakeImmutableOptions ("nospaces", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new FakeImmutableOptions ("with \"quotes\" spaced", new[] { 1, 2, 3 }, true, 123456789), "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };
            }
        }

        public static IEnumerable<object> UnParseDataFSharpOption
        {
            get
            {
                yield return new object[] { new FakeOptionsWithFSharpOption(), "" };
                yield return new object[] { new FakeOptionsWithFSharpOption { FileName = FSharpOption<string>.Some("myfile.bin") }, "--filename myfile.bin" };
                yield return new object[] { new FakeOptionsWithFSharpOption { Offset = FSharpOption<int>.Some(123456789) }, "123456789" };
                yield return new object[] { new FakeOptionsWithFSharpOption { FileName = FSharpOption<string>.Some("myfile.bin"), Offset = FSharpOption<int>.Some(123456789) }, "--filename myfile.bin 123456789" };
            }
        }
    }
}
