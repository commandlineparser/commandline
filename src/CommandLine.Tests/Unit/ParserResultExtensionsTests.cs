// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

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
        public static void Invoke_parsed_lambda_when_parsed()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<FakeOptions>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue);

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_parsed_lambda_when_parsed_for_verbs()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(
                new[] { "clone", "https://value.org/user/file.git" })
                .WithParsed<AddOptions>(opts => expected = "wrong1")
                .WithParsed<CommitOptions>(opts => expected = "wrong2")
                .WithParsed<CloneOptions>(opts => expected = opts.Urls.First());

            "https://value.org/user/file.git".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_not_parsed_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<FakeOptions>(new[] { "-i", "aaa" })
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_not_parsed_lambda_when_parsed_for_verbs()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(new[] { "undefined", "-xyz" })
                .WithParsed<AddOptions>(opts => expected = "wrong1")
                .WithParsed<CommitOptions>(opts => expected = "wrong2")
                .WithParsed<CloneOptions>(opts => expected = "wrong3")
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_parsed()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<FakeOptions>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "value".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<FakeOptions>(new[] { "-i", "aaa" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "changed".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<FakeOptions>(new[] { "--stringvalue", "value" })
                .Return(_ => 0, _ => -1);

            0.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_verbs()
        {
            var expected = Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(
                new[] { "clone", "https://value.org/user/file.git" })
                .Return(
                    (AddOptions opts) => 0,
                    (CommitOptions opts) => 1,
                    (CloneOptions opts) => 2,
                    errs => 3);

            2.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_failed_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<FakeOptions>(new[] { "-i", "aaa" })
                .Return(_ => 0, _ => -1);

            (-1).ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_failed_parsing_into_exit_code_for_verbs()
        {
            var expected = Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions>(
                new[] { "undefined", "-xyz" })
                .Return(
                    (AddOptions opts) => 0,
                    (CommitOptions opts) => 1,
                    (CloneOptions opts) => 2,
                    errs => 3);

            3.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_parsed_lambda_when_parsed_for_base_verbs()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions, DerivedAddOptions>(
                new[] { "derivedadd", "dummy.bin" })
                .WithParsed<AddOptions>(opts => expected = "wrong1")
                .WithParsed<CommitOptions>(opts => expected = "wrong2")
                .WithParsed<CloneOptions>(opts => expected = "wrong3")
                .WithParsed<BaseFileOptions>(opts => expected = opts.FileName);

            "dummy.bin".ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_single_base_verbs()
        {
            var expected = Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions, DerivedAddOptions>(
                new[] { "derivedadd", "dummy.bin" })
                .Return(
                    (BaseFileOptions opts) => 1,
                    errs => 2);

            1.ShouldBeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_multiple_base_verbs()
        {
            var expected = Parser.Default.ParseArguments<AddOptions, CommitOptions, CloneOptions, DerivedAddOptions>(
                new[] { "derivedadd", "dummy.bin" })
                .Return(
                    (AddOptions opts) => 0,
                    (CommitOptions opts) => 1,
                    (CloneOptions opts) => 2,
                    (BaseFileOptions opts) => 4,
                    (DerivedAddOptions opts) => 3,
                    errs => 5);

            4.ShouldBeEquivalentTo(expected);
        }
    }
}
