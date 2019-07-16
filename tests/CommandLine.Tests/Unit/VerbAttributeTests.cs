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
        [InlineData(nameof(Fakes.StaticResource.HelpText), typeof(Fakes.StaticResource), "Localized HelpText")]
        [InlineData(nameof(Fakes.StaticResource.InternalText), typeof(Fakes.StaticResource), "Internal Text")]
        [InlineData("PrivateText", typeof(Fakes.StaticResource), "Private Text")]
        [InlineData(nameof(Fakes.NonStaticResource.HelpText), typeof(Fakes.NonStaticResource), "Localized HelpText")]
        [InlineData(nameof(Fakes.InternalResource.Text), typeof(Fakes.InternalResource), "Localized Text")]
        public static void VerbHelpText(string helpText, Type resourceType, string expected)
        {
            VerbAttribute verbAttribute = new VerbAttribute("verb")
            {
                HelpText = helpText,
                ResourceType = resourceType
            };

            Assert.Equal(expected, verbAttribute.HelpText);
        }

        [Theory]
        [InlineData("NonExistantProperty", typeof(Fakes.StaticResource))]
        [InlineData(nameof(Fakes.NonStaticResource.InstanceText), typeof(Fakes.NonStaticResource))]
        [InlineData(nameof(Fakes.NonStaticResource.WriteOnlyText), typeof(Fakes.NonStaticResource))]
        public void ThrowsHelpText(string helpText, Type resourceType)
        {
            VerbAttribute verbAttribute = new VerbAttribute("verb")
            {
                HelpText = helpText,
                ResourceType = resourceType
            };

            // Verify exception
            Assert.Throws<ArgumentException>(() => verbAttribute.HelpText);
        }
    }
}
