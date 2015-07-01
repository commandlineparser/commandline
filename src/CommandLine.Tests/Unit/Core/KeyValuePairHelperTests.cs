// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using CommandLine.Core;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class KeyValuePairHelperTests
    {
        [Fact]
        public void Empty_token_sequence_creates_an_empty_KeyValuePair_sequence()
        {
            var expected = new KeyValuePair<string, IEnumerable<string>>[] { };

            var result = KeyValuePairHelper.ForSequence(new Token[] { });

            result.SequenceEqual(expected);
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
                });

            result.SequenceEqual(expected);
        }

    }
}
