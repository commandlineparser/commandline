// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;using CommandLine.Tests.Fakes;using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
    public class UnParserExtensionsTests
    {
        [Theory]        [MemberData("UnParseData")]        public static void UnParsing_instance_returns_command_line(FakeOptions options, string result)        {            new Parser()                .FormatCommandLine(options)                .ShouldBeEquivalentTo(result);        }

        [Theory]
        [MemberData("UnParseDataVerbs")]
        public static void UnParsing_instance_returns_command_line_for_verbs(AddOptions options, string result)
        {
            new Parser()
                .FormatCommandLine(options)
                .ShouldBeEquivalentTo(result);
        }

        public static IEnumerable<object> UnParseData
        {            get            {
                yield return new object[] { new FakeOptions(), "" };
                yield return new object[] { new FakeOptions { BoolValue = true }, "-x" };
                yield return new object[] { new FakeOptions { IntSequence = new[] { 1, 2, 3 } }, "-i 1 2 3" };
                yield return new object[] { new FakeOptions { StringValue = "nospaces" }, "--stringvalue nospaces" };
                yield return new object[] { new FakeOptions { StringValue = " with spaces " }, "--stringvalue \" with spaces \"" };
                yield return new object[] { new FakeOptions { StringValue = "with\"quote" }, "--stringvalue \"with\\\"quote\"" };
                yield return new object[] { new FakeOptions { StringValue = "with \"quotes\" spaced" }, "--stringvalue \"with \\\"quotes\\\" spaced\"" };
                yield return new object[] { new FakeOptions { LongValue = 123456789 }, "123456789" };
                yield return new object[] { new FakeOptions { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "nospaces", LongValue = 123456789 }, "-i 1 2 3 --stringvalue nospaces -x 123456789" };
                yield return new object[] { new FakeOptions { BoolValue = true, IntSequence = new[] { 1, 2, 3 }, StringValue = "with \"quotes\" spaced", LongValue = 123456789 }, "-i 1 2 3 --stringvalue \"with \\\"quotes\\\" spaced\" -x 123456789" };            }        }

        public static IEnumerable<object> UnParseDataVerbs        {            get            {
                yield return new object[] { new AddOptions(), "add" };
                yield return new object[] { new AddOptions { Patch = true, FileName = "mysource.cs" }, "add --patch mysource.cs" };
                yield return new object[] { new AddOptions { Force = true, FileName = "mysource.fs" }, "add --force mysource.fs" };            }        }

    }
}
