// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Linq;
using Xunit;
using FluentAssertions;
using CSharpx;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class ScalarTests
    {
        [Fact]
        public void Partition_scalar_values_from_empty_token_sequence()
        {
            var expected = new Token[] { };

            var result = Scalar.Partition(
                new Token[] { },
                name =>
                    new[] { "str", "int" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Scalar, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().BeEquivalentTo(result);
        }

        [Fact]
        public void Partition_scalar_values()
        {
            var expected = new [] { Token.Name("str"), Token.Value("strvalue") };

            var result = Scalar.Partition(
                new []
                    {
                        Token.Name("str"), Token.Value("strvalue"), Token.Value("freevalue"),
                        Token.Name("x"), Token.Value("freevalue2")
                    },
                name =>
                    new[] { "str", "int" }.Contains(name)
                        ? Maybe.Just(TypeDescriptor.Create(TargetType.Scalar, Maybe.Nothing<int>()))
                        : Maybe.Nothing<TypeDescriptor>());

            expected.Should().BeEquivalentTo(result);
        }
    }
}
