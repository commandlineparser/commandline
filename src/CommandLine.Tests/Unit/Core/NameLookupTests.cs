// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using CommandLine.Core;
using FluentAssertions;
using Xunit;
using CSharpx;

namespace CommandLine.Tests.Unit.Core
{
    public class NameLookupTests
    {
        [Fact]
        public void Lookup_name_of_sequence_option_with_separator()
        {
            // Fixture setup
            var expected = Maybe.Just(".");
            var specs = new[] { new OptionSpecification(string.Empty, "string-seq",
                false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '.', null, string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<string>), TargetType.Sequence)};

            // Exercize system
            var result = NameLookup.HavingSeparator("string-seq", specs, StringComparer.InvariantCulture);

            // Verify outcome
            expected.ShouldBeEquivalentTo(result);

            // Teardown
        }
    }
}
