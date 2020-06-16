// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Linq;
using Xunit;
using FluentAssertions;
using CommandLine.Tests.Fakes;
using System.Threading.Tasks;

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

            "value".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_parsed_lambda_when_parsedAsync()
        {
            var expected = string.Empty;
            await Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .WithParsedAsync(opts => Task.Run(() => expected = opts.StringValue));

            "value".Should().BeEquivalentTo(expected);
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

            "https://value.org/user/file.git".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_parsed_lambda_when_parsed_for_verbsAsync()
        {
            var expected = string.Empty;
            var parsedArguments = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                new[] { "clone", "https://value.org/user/file.git" });

            await parsedArguments.WithParsedAsync<Add_Verb>(opts => Task.Run(() => expected = "wrong1"));
            await parsedArguments.WithParsedAsync<Commit_Verb>(opts => Task.Run(() => expected = "wrong2"));
            await parsedArguments.WithParsedAsync<Clone_Verb>(opts => Task.Run(() => expected = opts.Urls.First()));

            "https://value.org/user/file.git".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_not_parsed_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .WithNotParsed(_ => expected = "changed");

            "changed".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_not_parsed_lambda_when_not_parsedAsync()
        {
            var expected = "a default";
            await Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .WithNotParsedAsync(_ => Task.Run(() => expected = "changed"));

            "changed".Should().BeEquivalentTo(expected);
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

            "changed".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_not_parsed_lambda_when_parsed_for_verbsAsync()
        {
            var expected = "a default";
            var parsedArguments = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(new[] { "undefined", "-xyz" });

            await parsedArguments.WithParsedAsync<Add_Verb>(opts => Task.Run(() => expected = "wrong1"));
            await parsedArguments.WithParsedAsync<Commit_Verb>(opts => Task.Run(() => expected = "wrong2"));
            await parsedArguments.WithParsedAsync<Clone_Verb>(opts => Task.Run(() => expected = "wrong3"));
            await parsedArguments.WithNotParsedAsync(_ => Task.Run(() => expected = "changed"));

            "changed".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_parsed()
        {
            var expected = string.Empty;
            Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "value".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_proper_lambda_when_parsedAsync()
        {
            var expected = string.Empty;
            var parsedArguments = Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" });

            await parsedArguments.WithParsedAsync(opts => Task.Run(() => expected = opts.StringValue));
            await parsedArguments.WithNotParsedAsync(_ => Task.Run(() => expected = "changed"));

            "value".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void Invoke_proper_lambda_when_not_parsed()
        {
            var expected = "a default";
            Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .WithParsed(opts => expected = opts.StringValue)
                .WithNotParsed(_ => expected = "changed");

            "changed".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_proper_lambda_when_not_parsedAsync()
        {
            var expected = "a default";
            var parsedArguments = Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" });

            await parsedArguments.WithParsedAsync(opts => Task.Run(() => expected = opts.StringValue));
            await parsedArguments.WithNotParsedAsync(_ => Task.Run(() => expected = "changed"));

            "changed".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<Simple_Options>(new[] { "--stringvalue", "value" })
                .MapResult(_ => 0, _ => -1);

            0.Should().Be(expected);
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

            2.Should().Be(expected);
        }

        [Fact]
        public static void Turn_failed_parsing_into_exit_code()
        {
            var expected = Parser.Default.ParseArguments<Simple_Options>(new[] { "-i", "aaa" })
                .MapResult(_ => 0, _ => -1);

            (-1).Should().Be(expected);
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

            3.Should().Be(expected);
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

            "dummy.bin".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static async Task Invoke_parsed_lambda_when_parsed_for_base_verbsAsync()
        {
            var expected = string.Empty;
            var parsedArguments = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                new[] { "derivedadd", "dummy.bin" });

            await parsedArguments.WithParsedAsync<Add_Verb>(opts => Task.Run(() => expected = "wrong1"));
            await parsedArguments.WithParsedAsync<Commit_Verb>(opts => Task.Run(() => expected = "wrong2"));
            await parsedArguments.WithParsedAsync<Clone_Verb>(opts => Task.Run(() => expected = "wrong3"));
            await parsedArguments.WithParsedAsync<Base_Class_For_Verb>(opts => Task.Run(() => expected = opts.FileName));

            "dummy.bin".Should().BeEquivalentTo(expected);
        }

        [Fact]
        public static void Turn_sucessful_parsing_into_exit_code_for_single_base_verbs()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                new[] { "derivedadd", "dummy.bin" })
                .MapResult(
                    (Base_Class_For_Verb opts) => 1,
                    errs => 2);

            1.Should().Be(expected);
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

            4.Should().Be(expected);
        }

        [Fact]
        public static void Turn_successful_parsing_into_exit_code_for_verbs_using_generalized_map_result()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb>(
                    new[] {"undefined", "-xyz"})
                .MapResult(errs => 3,
                    (Func<Add_Verb, int>) (opts => 0),
                    (Func<Commit_Verb, int>) (opts => 1),
                    (Func<Clone_Verb, int>) (opts => 2)
                );

            3.Should().Be(expected);
        }

        [Fact]
        public static void Turn_successful_parsing_into_exit_code_for_multiple_base_verbs_using_generalized_map_result()
        {
            var expected = Parser.Default.ParseArguments<Add_Verb, Commit_Verb, Clone_Verb, Derived_Verb>(
                    new[] {"derivedadd", "dummy.bin"})
                .MapResult(errs => 5,
                    (Func<Add_Verb, int>) (opts => 0),
                    (Func<Commit_Verb, int>) (opts => 1),
                    (Func<Clone_Verb, int>) (opts => 2),
                    (Func<Derived_Verb, int>) (opts => 3));

            3.Should().Be(expected);
        }

        [Fact]
        public static void Invoke_generalized_map_result_with_more_than_16_verbs()
        {
            var expected = Parser.Default.ParseArguments(new[] {"Verb17", "test.bin"},
                typeof(Option1), typeof(Option2), typeof(Option3), typeof(Option4), typeof(Option5), typeof(Option6),
                typeof(Option7), typeof(Option8), typeof(Option9), typeof(Option10), typeof(Option11), typeof(Option12),
                typeof(Option13), typeof(Option14), typeof(Option15), typeof(Option16), typeof(Option17)).MapResult(
                errs => 100,
                (Func<Option1, int>) (opts => 1),
                (Func<Option2, int>) (opts => 2),
                (Func<Option3, int>) (opts => 3),
                (Func<Option4, int>) (opts => 4),
                (Func<Option5, int>) (opts => 5),
                (Func<Option6, int>) (opts => 6),
                (Func<Option7, int>) (opts => 7),
                (Func<Option8, int>) (opts => 8),
                (Func<Option9, int>) (opts => 9),
                (Func<Option10, int>) (opts => 10),
                (Func<Option11, int>) (opts => 11),
                (Func<Option12, int>) (opts => 12),
                (Func<Option13, int>) (opts => 13),
                (Func<Option14, int>) (opts => 14),
                (Func<Option15, int>) (opts => 15),
                (Func<Option16, int>) (opts => 16),
                (Func<Option17, int>) (opts => 17));
            17.Should().Be(expected);
        }
    }
}
