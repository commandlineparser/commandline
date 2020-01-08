using System;
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
        [InlineData("HelpText", typeof(Fakes.StaticResource), "Localized HelpText")]
        [InlineData("HelpText", typeof(Fakes.NonStaticResource), "Localized HelpText")]
        public static void HelpText(string helpText, Type resourceType, string expected)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute();
            baseAttribute.HelpText = helpText;
            baseAttribute.ResourceType = resourceType;
            
            Assert.Equal(expected, baseAttribute.HelpText);
        }

        [Theory]
        [InlineData("HelpText", typeof(Fakes.NonStaticResource_WithNonStaticProperty))]
        [InlineData("WriteOnlyText", typeof(Fakes.NonStaticResource))]
        [InlineData("PrivateOnlyText", typeof(Fakes.NonStaticResource))]
        [InlineData("HelpText", typeof(Fakes.InternalResource))]
        public void ThrowsHelpText(string helpText, Type resourceType)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute();
            baseAttribute.HelpText = helpText;
            baseAttribute.ResourceType = resourceType;

            // Verify exception
            Assert.Throws<ArgumentException>(() => baseAttribute.HelpText.ToString());
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
