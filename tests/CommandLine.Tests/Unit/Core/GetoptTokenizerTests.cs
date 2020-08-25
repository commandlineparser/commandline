// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using RailwaySharp.ErrorHandling;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class GetoptTokenizerTests
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
                GetoptTokenizer.ExplodeOptionList(
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
                GetoptTokenizer.ExplodeOptionList(
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
        public void Should_properly_parse_option_with_equals_in_value()
        {
            /**
             * This is how the arg. would look in `static void Main(string[] args)`
             * if passed from the command-line and the option-value wrapped in quotes.
             * Ex.) ./app --connectionString="Server=localhost;Data Source..."
             */
            var args = new[] { "--connectionString=Server=localhost;Data Source=(LocalDB)\v12.0;Initial Catalog=temp;" };

            var result = GetoptTokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound);

            var tokens = result.SucceededWith();

            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Count());
            Assert.Equal("connectionString", tokens.First().Text);
            Assert.Equal("Server=localhost;Data Source=(LocalDB)\v12.0;Initial Catalog=temp;", tokens.Last().Text);
        }

        [Fact]
        public void Should_return_error_if_option_format_with_equals_is_not_correct()
        {
            var args = new[] { "--option1 = fail", "--option2= succeed" };

            var result = GetoptTokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound);

            var errors = result.SuccessMessages();

            Assert.NotNull(errors);
            Assert.Equal(1, errors.Count());
            Assert.Equal(ErrorType.BadFormatTokenError, errors.First().Tag);

            var tokens = result.SucceededWith();
            Assert.NotNull(tokens);
            Assert.Equal(2, tokens.Count());
            Assert.Equal(TokenType.Name, tokens.First().Tag);
            Assert.Equal(TokenType.Value, tokens.Last().Tag);
            Assert.Equal("option2", tokens.First().Text);
            Assert.Equal(" succeed", tokens.Last().Text);
        }


        [Theory]
        [InlineData(new[] { "-a", "-" }, 2,"a","-")]
        [InlineData(new[] { "--file", "-" }, 2,"file","-")]
        [InlineData(new[] { "-f-" }, 2,"f", "-")]
        [InlineData(new[] { "--file=-" }, 2, "file", "-")]
        [InlineData(new[] { "-a", "--" }, 2, "a", "--")]
        public void Single_dash_as_a_value(string[] args, int countExcepted,string first,string last)
        {
            //Arrange
            //Act
            var result = GetoptTokenizer.Tokenize(args, name => NameLookupResult.OtherOptionFound);
            var tokens = result.SucceededWith().ToList();
            //Assert
            tokens.Should().NotBeNull();
            tokens.Count.Should().Be(countExcepted);
            tokens.First().Text.Should().Be(first);
            tokens.Last().Text.Should().Be(last);
        }
    }

}
