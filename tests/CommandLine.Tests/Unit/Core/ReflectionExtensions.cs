// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using CommandLine.Core;
using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class ReflectionHelperTests
    {
        [Fact]
        public static void Class_with_public_set_properties_or_fields_is_ranked_mutable()
        {
            typeof(Simple_Options).IsMutable().Should().BeTrue();
        }

        [Fact]
        public static void Class_without_public_set_properties_or_fields_is_ranked_immutable()
        {
            typeof(Immutable_Simple_Options).IsMutable().Should().BeFalse();
        }
    }
}
