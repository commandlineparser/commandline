using FluentAssertions;
using System.Linq;
using Xunit;

// Issue #847
// no parsing error if additional positional argument is present

namespace CommandLine.Tests.Unit
{
    public class Issue847Tests
    {
        [Fact]
        public void IgnoreUnknownArguments_should_work_for_values()
        {
            var arguments = new[] { "foo", "bar", "too_much" };
            var result = new Parser(with => { with.IgnoreUnknownArguments = true; })
                .ParseArguments<Options>(arguments);

            Assert.Empty(result.Errors);
            Assert.Equal(ParserResultType.Parsed, result.Tag);

            result.WithParsed(options =>
            {
                options.Foo.Should().Be("foo");
                options.Bar.Should().Be("bar");
            });
        }

        [Fact]
        public void Additional_positional_arguments_should_raise_errors()
        {
            var arguments = new[] { "foo", "bar", "too_much" };
            var result = new Parser(with => { with.IgnoreUnknownArguments = false; })
                .ParseArguments<Options>(arguments);

            Assert.NotEmpty(result.Errors);
            Assert.Equal(ParserResultType.NotParsed, result.Tag);

            result.WithNotParsed(errors =>
            {
                Assert.NotEmpty(errors);
                Assert.IsType<UnknownValueError>(errors.Single());
            });
        }

        private class Options
        {
            [Value(0, Required = true)]
            public string Foo { get; set; }


            [Value(1, Required = false)]
            public string Bar { get; set; }
        }
    }
}
