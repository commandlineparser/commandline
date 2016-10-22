using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CommandLine.Tests.Unit
{
    public class BaseAttributeTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public static void Default(object defaultValue)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute();
            baseAttribute.Default = defaultValue;
            Assert.Equal(defaultValue, baseAttribute.Default);
        }

        private class TestBaseAttribute : BaseAttribute
        {
            public TestBaseAttribute()
            {
                // Do nothing
            }
        }
    }
}
