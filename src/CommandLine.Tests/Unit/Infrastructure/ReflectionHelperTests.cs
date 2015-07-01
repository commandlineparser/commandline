// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using CommandLine.Infrastructure;
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
            ReflectionHelper.IsTypeMutable(typeof(FakeOptions)).Should().BeTrue();
        }

        [Fact]
        public static void Class_without_public_set_properties_or_fields_is_ranked_immutable()
        {
            ReflectionHelper.IsTypeMutable(typeof(FakeImmutableOptions)).Should().BeFalse();
        }
    }
}
