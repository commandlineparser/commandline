using CommandLine.Tests.Fakes;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Attributes
{
    public class ValueOptionAttributeFixture : BaseFixture
    {
        [Fact]
        public void Index_Implicit_By_Declaration_Order()
        {
            var options = new OptionsWithValueOptionImplicitIndex();
            string[] args = "foo bar".Split();
            CommandLine.Parser.Default.ParseArguments(args, options);
            options.A.ShouldBeEquivalentTo("foo");
            options.B.ShouldBeEquivalentTo("bar");
            options.C.Should().BeNull();
        }

        [Fact]
        public void Index_Explicitly_Set_On_Value_Option()
        {
            var options = new OptionsWithValueOptionExplicitIndex();
            string[] args = "foo bar".Split();
            CommandLine.Parser.Default.ParseArguments(args, options);
            options.A.Should().BeNull();
            options.B.ShouldBeEquivalentTo("bar");
            options.C.ShouldBeEquivalentTo("foo");
        }
    }
}
