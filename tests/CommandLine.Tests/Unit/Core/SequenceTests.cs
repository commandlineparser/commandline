// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class SequenceTests
    {
        [Fact]
        public void Partition_sequence_values_from_empty_token_sequence()
        {
            var expected = new Token[] { };

            var result = Sequence.Partition(
                new Token[] { },
                name =>
                    new[] { "seq" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().AllBeEquivalentTo(result);
        }

        [Fact]
        public void Partition_sequence_values()
        {
            var expected = new[]
                {
                    Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1")
                };

            var result = Sequence.Partition(
                new[]
                    {
                        Token.Name("str"), Token.Value("strvalue"), Token.Value("freevalue"),
                        Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1"),
                        Token.Name("x"), Token.Value("freevalue2")
                    },
                name =>
                    new[] { "seq" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void Partition_sequence_values_from_two_sequences()
        {
            var expected = new[]
                {
                    Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1"),
                    Token.Name("seqb"), Token.Value("seqbval0")
                };

            var result = Sequence.Partition(
                new[]
                    {
                        Token.Name("str"), Token.Value("strvalue"), Token.Value("freevalue"),
                        Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1"),
                        Token.Name("x"), Token.Value("freevalue2"),
                        Token.Name("seqb"), Token.Value("seqbval0")
                    },
                name =>
                    new[] { "seq", "seqb" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void Partition_sequence_values_only()
        {
            var expected = new[]
                {
                    Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1")
                };

            var result = Sequence.Partition(
                new[]
                    {
                        Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1")
                    },
                name =>
                    new[] { "seq" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void Partition_sequence_multi_instance()
        {
            var expected = new[]
            {
                Token.Name("seq"),
                Token.Value("seqval0"),
                Token.Value("seqval1"),
                Token.Value("seqval2"),
                Token.Value("seqval3"),
                Token.Value("seqval4"),
            };

            var result = Sequence.Partition(
                new[]
                {
                    Token.Name("str"), Token.Value("strvalue"), Token.Value("freevalue"),
                    Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1"),
                    Token.Name("x"), Token.Value("freevalue2"),
                    Token.Name("seq"), Token.Value("seqval2"), Token.Value("seqval3"),
                    Token.Name("seq"), Token.Value("seqval4")
                },
                name =>
                    new[] { "seq" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            var actual = result.ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Partition_sequence_multi_instance_with_max()
        {
            var expected = new[]
            {
                Token.Name("seq"),
                Token.Value("seqval0"),
                Token.Value("seqval1"),
                Token.Value("seqval2"),
                Token.Value("seqval3"),
                Token.Value("seqval4"),
                Token.Value("seqval5"),
            };

            var result = Sequence.Partition(
                new[]
                {
                    Token.Name("str"), Token.Value("strvalue"), Token.Value("freevalue"),
                    Token.Name("seq"), Token.Value("seqval0"), Token.Value("seqval1"),
                    Token.Name("x"), Token.Value("freevalue2"),
                    Token.Name("seq"), Token.Value("seqval2"), Token.Value("seqval3"),
                    Token.Name("seq"), Token.Value("seqval4"), Token.Value("seqval5"),
                },
                name =>
                    new[] { "seq" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Sequence, Maybe.Just<int>(3)))
                        : Maybe.Nothing<TypeDescriptor>());

            Assert.Equal(expected, result);
        }
    }
}
