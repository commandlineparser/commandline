// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Tests.Fakes;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit
{
    public class ParserResultExtensionsTests
    {
        [Fact]
        public static void Invoke_parsed_lambda_when_parsed()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue);

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_parsed_lambda_when_parsed_for_verbs()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "clone", "https://value.org/user/file.git" })
                .WithParsed<Add_Verb>(opts => expected = "wrong1")
                .WithParsed<Commit_Verb>(opts => expected = "wrong2")
                .WithParsed<Clone_Verb>(opts => expected = opts.Urls.First());

            "https://value.org/user/file.git".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_not_parsed_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_not_parsed_lambda_when_parsed_for_verbs()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new[] { "undefined", "-xyz" })
                .WithParsed<Add_Verb>(opts => expected = "wrong1")
                .WithParsed<Commit_Verb>(opts => expected = "wrong2")
                .WithParsed<Clone_Verb>(opts => expected = "wrong3")
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_parsed()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .MapResult(_ => 0, _ => -1);

            0.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_verbs()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "clone", "https://value.org/user/file.git" })
                .MapResult(
                    (Add_Verb opts) => 0,
                    (Commit_Verb opts) => 1,
                    (Clone_Verb opts) => 2,
                    errs => 3);

            2.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_failed_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .MapResult(_ => 0, _ => -1);

            (-1).ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_failed_parsing_into_exit_code_for_verbs()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "undefined", "-xyz" })
                .MapResult(
                    (Add_Verb opts) => 0,
                    (Commit_Verb opts) => 1,
                    (Clone_Verb opts) => 2,
                    errs => 3);

            3.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_parsed_lambda_when_parsed_for_base_verbs()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                new[] { "derivedadd", "dummy.bin" })
                .WithParsed<Add_Verb>(opts => expected = "wrong1")
                .WithParsed<Commit_Verb>(opts => expected = "wrong2")
                .WithParsed<Clone_Verb>(opts => expected = "wrong3")
                .WithParsed<Base_Class_For_Verb>(opts => expected = opts.FileName);

            "dummy.bin".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_single_base_verbs()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                new[] { "derivedadd", "dummy.bin" })
                .MapResult(
                    (Base_Class_For_Verb opts) => 1,
                    errs => 2);

            1.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_multiple_base_verbs()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                new[] { "derivedadd", "dummy.bin" })
                .MapResult(
                    (Add_Verb opts) => 0,
                    (Commit_Verb opts) => 1,
                    (Clone_Verb opts) => 2,
                    (Base_Class_For_Verb opts) => 4,
                    (Derived_Verb opts) => 3,
                    errs => 5);

            4.ShouldBeEquivalentTo(expected);
        }
    }
}
