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
        public void Partition_sequence_values_from_two_sequneces()
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
    }
}
