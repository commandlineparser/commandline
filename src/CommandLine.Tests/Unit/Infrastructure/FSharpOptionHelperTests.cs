// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Reflection;

using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;

using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class FSharpOptionHelperTests
    {
        [Fact]
        public void Match_type_returns_true_if_FSharpOption()
        {
            var prop = typeof(FakeOptionsWithFSharpOption).GetProperty("FileName", BindingFlags.Public | BindingFlags.Instance);

            ReflectionHelper.IsFSharpOptionType(prop.PropertyType).Should().BeTrue();
        }
    }
}
