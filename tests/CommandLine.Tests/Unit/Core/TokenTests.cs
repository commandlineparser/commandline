// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using Xunit;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class TokenTests
    {
        [Fact]
        public void Equality()
        {
            Assert.True(Token.Name("nametok").Equals(Token.Name("nametok")));
        }
    }
}
