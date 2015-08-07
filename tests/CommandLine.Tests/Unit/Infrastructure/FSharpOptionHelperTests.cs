// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Tests.Fakes;
using Microsoft.FSharp.Core;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Infrastructure
{
    public class FSharpOptionHelperTests
    {
        [Fact]
        public void Match_type_returns_true_if_FSharpOption()
        {
            ReflectionHelper.IsFSharpOptionType(TestData.PropertyType)
                .Should().BeTrue();
        }

        [Fact]
        public void Get_underlying_type()
        {
            FSharpOptionHelper.GetUnderlyingType(TestData.PropertyType).FullName
                .ShouldBeEquivalentTo("System.String");
        }

        [Fact]
        public void Create_some()
        {
            var expected = FSharpOptionHelper.Some(FSharpOptionHelper.GetUnderlyingType(TestData.PropertyType), "with data");

            expected.Should().BeOfType<FSharpOption<string>>();
            FSharpOption<string>.get_IsSome((FSharpOption<string>)expected).Should().BeTrue();
        }

        [Fact]
        public void Create_none()
        {
            var expected = FSharpOptionHelper.None(FSharpOptionHelper.GetUnderlyingType(TestData.PropertyType));

            FSharpOption<string>.get_IsNone((FSharpOption<string>)expected).Should().BeTrue();
        }

        private PropertyInfo TestData
        {
            get { return typeof(Options_With_FSharpOption).GetProperty("FileName", BindingFlags.Public | BindingFlags.Instance); }
        }
    }
}
