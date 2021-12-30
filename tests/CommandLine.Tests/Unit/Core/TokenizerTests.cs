// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using RailwaySharp.ErrorHandling;
using CommandLine.Core;
using CommandLine.Infrastructure;

namespace CommandLine.Tests.Unit.Core
{
    public class TokenizerTests
    {
        [Fact]
        public void Explode_scalar_with_separator_in_odd_args_input_returns_sequence()
        {
            // Fixture setup
            var expectedTokens = new[] { Token.Name("i"), Token.Value("10"), Token.Name("string-seq"),
                Token.Value("aaa"), Token.Value("bb"),  Token.Value("cccc"), Token.Name("switch") };
            var specs = new[] { new OptionSpecification(string.Empty, "string-seq",
                false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), ',', null, string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<string>), TargetType.Sequence, string.Empty)};

            // Exercize system
            var result =
                Tokenizer.ExplodeOptionList(
                    Result.Succeed(
                        Enumerable.Empty<Token>().Concat(new[] { Token.Name("i"), Token.Value("10"),
                            Token.Name("string-seq"), Token.Value("aaa,bb,cccc"), Token.Name("switch") }),
                        Enumerable.Empty<Error>()),
                        optionName => NameLookup.HavingSeparator(optionName, specs, StringComparer.Ordinal));
            // Verify outcome
            ((Ok<IEnumerable<Token>, Error>)result).Success.Should().BeEquivalentTo(expectedTokens);

            // Teardown
        }

        [Fact]
        public void Explode_scalar_with_separator_in_even_args_input_returns_sequence()
        {
            // Fixture setup
            var expectedTokens = new[] { Token.Name("x"), Token.Name("string-seq"),
                Token.Value("aaa"), Token.Value("bb"),  Token.Value("cccc"), Token.Name("switch") };
            var specs = new[] { new OptionSpecification(string.Empty, "string-seq",
                false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), ',', null, string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<string>), TargetType.Sequence, string.Empty)};

            // Exercize system
            var result =
                Tokenizer.ExplodeOptionList(
                    Result.Succeed(
                        Enumerable.Empty<Token>().Concat(new[] { Token.Name("x"),
                            Token.Name("string-seq"), Token.Value("aaa,bb,cccc"), Token.Name("switch") }),
                        Enumerable.Empty<Error>()),
                        optionName => NameLookup.HavingSeparator(optionName, specs, StringComparer.Ordinal));

            // Verify outcome
            ((Ok<IEnumerable<Token>, Error>)result).Success.Should().BeEquivalentTo(expectedTokens);

            // Teardown
        }

        [Fact]
        public void Normalize_should_remove_all_names_and_values_with_explicit_assignment_of_non_existing_names()
        {
            // Fixture setup
            var expectedTokens = new[] {
                Token.Name("x"), Token.Name("string-seq"), Token.Value("value0", true), Token.Value("bb"),
                Token.Name("switch") };
            Func<string, bool> nameLookup =
                name => name.Equals("x") || name.Equals("string-seq") || name.Equals("switch");

            // Exercize system
            var result =
                Tokenizer.Normalize(
                        //Result.Succeed(
                        Enumerable.Empty<Token>()
                            .Concat(
                                new[] {
                                    Token.Name("x"), Token.Name("string-seq"), Token.Value("value0", true), Token.Value("bb"),
                                    Token.Name("unknown"), Token.Value("value0", true), Token.Name("switch") })
                    //,Enumerable.Empty<Error>()),
                    , nameLookup);

            // Verify outcome
            result.Should().BeEquivalentTo(expectedTokens);

            // Teardown
        }

        [Fact]
        public void Normalize_should_remove_all_names_of_non_existing_names()
        {
            // Fixture setup
            var expectedTokens = new[] {
                Token.Name("x"), Token.Name("string-seq"), Token.Value("value0", true), Token.Value("bb"),
                Token.Name("switch") };
            Func<string, bool> nameLookup =
                name => name.Equals("x") || name.Equals("string-seq") || name.Equals("switch");

            // Exercize system
            var result =
                Tokenizer.Normalize(
                    //Result.Succeed(
                    Enumerable.Empty<Token>()
                        .Concat(
                            new[] {
                                Token.Name("x"), Token.Name("string-seq"), Token.Value("value0", true), Token.Value("bb"),
                                Token.Name("unknown"), Token.Name("switch") })
                    //,Enumerable.Empty<Error>()),
                    , nameLookup);

            // Verify outcome
            result.Should().BeEquivalentTo(expectedTokens);

            // Teardown
        }

        [Fact]
        public void Should_properly_parse_option_with_equals_in_value()
        {
            /**
             * This is how the arg. would look in `static void Main(string[] args)`
             * if passed from the command-line and the option-value wrapped in quotes.
             * Ex.) ./app --connectionString="Server=localhost;Data Source..."
             */
            var args = new[] { "--connectionString=Server=localhost;Data Source=(LocalDB)\v12.0;Initial Catalog=temp;" };

            var result = Tokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound, token => token);

            var tokens = result.SucceededWith();

            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Count());
            Assert.Equal("connectionString", tokens.First().Text);
            Assert.Equal("Server=localhost;Data Source=(LocalDB)\v12.0;Initial Catalog=temp;", tokens.Last().Text);
        }

        [Fact]
        public void Should_return_error_if_option_format_with_equals_is_not_correct()
        {
            var args = new[] { "--option1 = fail", "--option2= fail" };

            var result = Tokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound, token => token);

            var tokens = result.SuccessMessages();

            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Count());
            Assert.Equal(ErrorType.BadFormatTokenError, tokens.First().Tag);
            Assert.Equal(ErrorType.BadFormatTokenError, tokens.Last().Tag);
        }

        [Theory]
        [InlineData(new[] { "-a", "-" }, 2,"a","-")]
        [InlineData(new[] { "--file", "-" }, 2,"file","-")]
        [InlineData(new[] { "-f-" }, 2,"f", "-")]
        [InlineData(new[] { "--file=-" }, 2, "file", "-")]
        [InlineData(new[] { "-a", "--" }, 1, "a", "a")]
        public void Single_dash_as_a_value(string[] args, int countExcepted,string first,string last)
        {
            //Arrange
            //Act
            var result = Tokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound, token => token);
            var tokens = result.SucceededWith().ToList();
            //Assert
            tokens.Should().NotBeNull();
            tokens.Count.Should().Be(countExcepted);
            tokens.First().Text.Should().Be(first);
            tokens.Last().Text.Should().Be(last);
        }
    }

}
