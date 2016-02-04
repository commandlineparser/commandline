// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
using CSharpx;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class TokenPartitionerTests
    {
        [Fact]
        public void Partition_sequence_returns_sequence()
        {
            // Fixture setup
            var expectedSequence = new[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("i", new[] {"10", "20", "30", "40"}) 
                };
            var specs =new[]
                {
                    new OptionSpecification(string.Empty, "stringvalue", false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', null, string.Empty, string.Empty, new List<string>(), typeof(string), TargetType.Scalar),
                    new OptionSpecification("i", string.Empty, false, string.Empty, Maybe.Just(3), Maybe.Just(4), '\0', null, string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<int>), TargetType.Sequence)
                };

            // Exercize system 
            var result = TokenPartitioner.Partition(
                new[] { Token.Name("i"), Token.Value("10"), Token.Value("20"), Token.Value("30"), Token.Value("40") },
                name => TypeLookup.FindTypeDescriptorAndSibling(name, specs, StringComparer.Ordinal)
                );

            // Verify outcome
            var options = result.Item1;
            Assert.True(expectedSequence.All(a => options.Any(r => a.Key.Equals(r.Key) && a.Value.SequenceEqual(r.Value))));

            // Teardown
        }

        [Fact]
        public void Partition_sequence_returns_sequence_with_duplicates()
        {
            // Fixture setup
            var expectedSequence = new[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("i", new[] {"10", "10", "30", "40"}) 
                };
            var specs =new[]
                {
                    new OptionSpecification(string.Empty, "stringvalue", false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', null, string.Empty, string.Empty, new List<string>(), typeof(string), TargetType.Scalar),
                    new OptionSpecification("i", string.Empty, false, string.Empty, Maybe.Just(3), Maybe.Just(4), '\0', null, string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<int>), TargetType.Sequence)
                };

            // Exercize system 
            var result = TokenPartitioner.Partition(
                new[] { Token.Name("i"), Token.Value("10"), Token.Value("10"), Token.Value("30"), Token.Value("40") },
                name => TypeLookup.FindTypeDescriptorAndSibling(name, specs, StringComparer.Ordinal)
                );

            // Verify outcome
            var options = result.Item1;
            Assert.True(expectedSequence.All(a => options.Any(r => a.Key.Equals(r.Key) && a.Value.SequenceEqual(r.Value))));

            // Teardown
        }
    }
}
