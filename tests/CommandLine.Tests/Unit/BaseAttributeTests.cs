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
            TestBaseAttribute baseAttribute = new TestBaseAttribute
            {
                Default = defaultValue
            };
            Assert.Equal(defaultValue, baseAttribute.Default);
        }

        [Theory]
        [InlineData("", null, "")]
        [InlineData("", typeof(Fakes.StaticResource), "")]
        [InlineData("Help text", null, "Help text")]
        [InlineData(nameof(Fakes.StaticResource.HelpText), typeof(Fakes.StaticResource), "Localized HelpText")]
        [InlineData(nameof(Fakes.StaticResource.InternalText), typeof(Fakes.StaticResource), "Internal Text")]
        [InlineData("PrivateText", typeof(Fakes.StaticResource), "Private Text")]
        [InlineData(nameof(Fakes.NonStaticResource.HelpText), typeof(Fakes.NonStaticResource), "Localized HelpText")]
        [InlineData(nameof(Fakes.InternalResource.Text), typeof(Fakes.InternalResource), "Localized Text")]
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
        [InlineData("NonExistantProperty", typeof(Fakes.StaticResource))]
        [InlineData(nameof(Fakes.NonStaticResource.InstanceText), typeof(Fakes.NonStaticResource))]
        [InlineData(nameof(Fakes.NonStaticResource.WriteOnlyText), typeof(Fakes.NonStaticResource))]
        public void ThrowsHelpText(string helpText, Type resourceType)
        {
            TestBaseAttribute baseAttribute = new TestBaseAttribute
            {
                HelpText = helpText,
                ResourceType = resourceType
            };

            // Verify exception
            Assert.Throws<ArgumentException>(() => baseAttribute.HelpText);
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
