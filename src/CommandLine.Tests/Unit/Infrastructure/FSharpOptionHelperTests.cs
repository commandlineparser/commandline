// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using CommandLine.Infrastructure;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Infrastructure
{
    class FSharpOptionHelperTests
    {
        [Fact]
        public void FSharpCore_loaded_when_present()
        {
            new FSharpOptionHelper().Available.Should().BeTrue();
        }
    }
}
