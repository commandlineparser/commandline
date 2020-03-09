using System.Linq;
using CommandLine.Tests.Fakes;
using CommandLine.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

//Issue #70
//When the factory overload is used for ParseArguments, there should be no constraint not having an empty constructor.

namespace CommandLine.Tests.Unit
{
    public class Issue70Tests
    {
        [Fact]
        public void Create_instance_with_factory_method_should_not_fail()
        {
            bool actual = false;

            var arguments = new[] { "--amend" };
            var result = Parser.Default.ParseArguments(() => Mutable_Without_Empty_Constructor.Create(), arguments);
            result.WithParsed(options => {
                actual = options.Amend;
            });

            actual.Should().BeTrue();
        }
    }
}
