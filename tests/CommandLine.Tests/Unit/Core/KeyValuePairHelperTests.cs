// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Xunit;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class KeyValuePairHelperTests
    {
        [Fact]
        public void Empty_token_sequence_creates_an_empty_KeyValuePair_sequence()
        {
            var expected = new KeyValuePair<string, IEnumerable<string>>[] { };

            var result = KeyValuePairHelper.ForSequence(new Token[] { });
        
            AssertEqual(expected, result);
        }

        [Fact]
        public void Token_sequence_creates_a_KeyValuePair_sequence()
        {
            var expected = new[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("seq", new[] {"seq0", "seq1", "seq2"})
                };

            var result = KeyValuePairHelper.ForSequence(new []
                {
                    Token.Name("seq"), Token.Value("seq0"), Token.Value("seq1"), Token.Value("seq2") 
                }).ToArray();

            AssertEqual(expected, result);
        }

        [Fact]
        public void Token_sequence_creates_a_KeyValuePair_sequence_for_multiple_sequences()
        {
            var expected = new[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("seq1", new[] {"seq10", "seq11", "seq12"}),
                    new KeyValuePair<string, IEnumerable<string>>("seq2", new[] {"seq20", "seq21"})
                };

            var result = KeyValuePairHelper.ForSequence(new[]
                {
                    Token.Name("seq1"), Token.Value("seq10"), Token.Value("seq11"), Token.Value("seq12"),
                    Token.Name("seq2"), Token.Value("seq20"), Token.Value("seq21")
                });

            AssertEqual(expected, result);
        }

        private static void AssertEqual(IEnumerable<KeyValuePair<string, IEnumerable<string>>> expected, IEnumerable<KeyValuePair<string, IEnumerable<string>>> result)
        {
            Assert.Equal(expected.Count(), result.Count());
            foreach (var value in expected.Zip(result, (e, r) => new { Expected = e, Result = r }))
            {
                Assert.Equal(value.Expected.Key, value.Result.Key);
                Assert.Equal(value.Expected.Value, value.Result.Value);
            }
        }
    }
}
