using System;
using System.Linq;
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

        [Theory]
        [InlineData("", null, "")]
        [InlineData("", typeof(Fakes.StaticResource), "")]
        [InlineData("Help text", null, "Help text")]
        [InlineData(nameof(Fakes.StaticResource.HelpText), typeof(Fakes.StaticResource), "Localized HelpText")]
        [InlineData(nameof(Fakes.NonStaticResource.HelpText), typeof(Fakes.NonStaticResource), "Localized HelpText")]
        public static void HelpText(string helpText, Type resourceType, string expected)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute
            {
                HelpText = helpText,
                ResourceType = resourceType
            };

            Assert.Equal(expected, baseAttribute.HelpText);
        }

        [Theory]
        [InlineData("HelpText", typeof(Fakes.NonStaticResource_WithNonStaticProperty), "propertyName")]
        [InlineData("WriteOnlyText", typeof(Fakes.NonStaticResource), "propertyName")]
        [InlineData("PrivateOnlyText", typeof(Fakes.NonStaticResource), "propertyName")]
        [InlineData("HelpText", typeof(Fakes.InternalResource), nameof(BaseAttribute.ResourceType))]
        public void ThrowsHelpText(string helpText, Type resourceType, string expectedParamName)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute
            {
                HelpText = helpText,
                ResourceType = resourceType
            };

            // Verify exception
            var e = Assert.Throws<ArgumentException>(() => baseAttribute.HelpText);

            Assert.Equal(expectedParamName, e.ParamName);
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
