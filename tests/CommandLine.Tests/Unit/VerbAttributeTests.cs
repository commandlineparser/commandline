using System;
using Xunit;

namespace CommandLine.Tests
{
	//Test localization of VerbAttribute
    public  class VerbAttributeTests
    {
        [Theory]
        [InlineData("", null, "")]
        [InlineData("", typeof(Fakes.StaticResource), "")]
        [InlineData("Help text", null, "Help text")]
        [InlineData("HelpText", typeof(Fakes.StaticResource), "Localized HelpText")]
        [InlineData("HelpText", typeof(Fakes.NonStaticResource), "Localized HelpText")]
        public static void VerbHelpText(string helpText, Type resourceType, string expected)
        {
            TestVerbAttribute verbAttribute = new TestVerbAttribute
            {
                HelpText = helpText, 
                ResourceType = resourceType
            };

            Assert.Equal(expected, verbAttribute.HelpText);
        }

        [Theory]
        [InlineData("HelpText", typeof(Fakes.NonStaticResource_WithNonStaticProperty))]
        [InlineData("WriteOnlyText", typeof(Fakes.NonStaticResource))]
        [InlineData("PrivateOnlyText", typeof(Fakes.NonStaticResource))]
        [InlineData("HelpText", typeof(Fakes.InternalResource))]
        public void ThrowsHelpText(string helpText, Type resourceType)
        {
            TestVerbAttribute verbAttribute = new TestVerbAttribute
            {
                HelpText = helpText, 
                ResourceType = resourceType
            };

            // Verify exception
            Assert.Throws<ArgumentException>(() => verbAttribute.HelpText);
        }

        private class TestVerbAttribute : VerbAttribute
        {
            public TestVerbAttribute() : base("verb")
            {
                // Do nothing
            }
        }
    }
}
