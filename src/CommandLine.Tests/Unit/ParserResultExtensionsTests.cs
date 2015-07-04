// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommandLine.Tests.Fakes;

using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
    public class ParserResultExtensionsTests
    {
        [Fact]
        public static void Invoker_parsed_lambda_when_parsed()
        {
            string expected = string.Empty;
            var result = Parser.Default.ParseArguments<FakeOptions>(new[] { "--stringvalue", "value" });
            result.WithParsed(opts => expected = opts.StringValue);

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoker_not_parsed_lambda_when_not_parsed()
        {
            string expected = "a default";
            var result = Parser.Default.ParseArguments<FakeOptions>(new[] { "-i", "aaa" });
            result.WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoker_proper_lambda_when_parsed()
        {
            string expected = string.Empty;
            var result = Parser.Default.ParseArguments<FakeOptions>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoker_proper_lambda_when_not_parsed()
        {
            string expected = "a default";
            var result = Parser.Default.ParseArguments<FakeOptions>(new[] { "-i", "aaa" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }
    }
}
