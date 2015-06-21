// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Core;
using Xunit;
using FluentAssertions;

namespace CommandLine.Tests.Unit.Core
{
    public class KeyValuePairHelperTests
    {
        [Fact]
        public void Empty_token_sequence_creates_an_empty_KeyValuePair_sequence()
        {
            var expected = new KeyValuePair<string, IEnumerable<string>>[] { };

            var result = KeyValuePairHelper.CreateSequence(new Token[] { });

            result.SequenceEqual(expected);
        }
    }
}
