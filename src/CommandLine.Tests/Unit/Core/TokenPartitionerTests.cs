// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
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
                    new OptionSpecification(string.Empty, "stringvalue", false, string.Empty, -1, -1, null, typeof(string), string.Empty, string.Empty),
                    new OptionSpecification("i", string.Empty, false, string.Empty, 3, 4, null, typeof(IEnumerable<int>), string.Empty, string.Empty)
                };

            // Exercize system 
            var result = TokenPartitioner.Partition(
                new[] { Token.Name("i"), Token.Value("10"), Token.Value("20"), Token.Value("30"), Token.Value("40") },
                name => TypeLookup.GetDescriptorInfo(name, specs, StringComparer.InvariantCulture)
                );

            // Verify outcome
            Assert.True(expectedSequence.All(a => result.Item1.Any(r => a.Key.Equals(r.Key) && a.Value.SequenceEqual(r.Value))));

            // Teardown
        }
    }
}
