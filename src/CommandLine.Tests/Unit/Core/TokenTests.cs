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
    public class TokenTests
    {
        [Fact]
        public void Equality()
        {
            Assert.True(Token.Name("nametok").Equals(Token.Name("nametok")));
        }
    }
}
