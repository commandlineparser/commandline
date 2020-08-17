using CommandLine.Core;
using CommandLine.Tests.Fakes;
using CSharpx;
using System.Collections.Generic;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{

    public class SpecificationPropertyRulesTests
    {
        [Fact]
        public void Lookup_allows_multi_instance()
        {
            var tokens = new[]
            {
                Token.Name("name"),
                Token.Value("value"),
                Token.Name("name"),
                Token.Value("value2"),
            };

            var specProps = new[]
            {
                SpecificationProperty.Create(
                    new OptionSpecification(string.Empty, "name", false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', Maybe.Nothing<object>(), string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<string>), TargetType.Sequence, string.Empty),
                    typeof(SequenceOptions).GetProperty(nameof(SequenceOptions.StringSequence)),
                    Maybe.Just(new object())),
            };

            var results = specProps.Validate(SpecificationPropertyRules.Lookup(tokens, true));
            Assert.Empty(results);
        }

        [Fact]
        public void Lookup_fails_with_repeated_options_false_multi_instance()
        {
            var tokens = new[]
            {
                Token.Name("name"),
                Token.Value("value"),
                Token.Name("name"),
                Token.Value("value2"),
            };

            var specProps = new[]
            {
                SpecificationProperty.Create(
                    new OptionSpecification(string.Empty, "name", false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', Maybe.Nothing<object>(), string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<string>), TargetType.Sequence, string.Empty),
                    typeof(SequenceOptions).GetProperty(nameof(SequenceOptions.StringSequence)),
                    Maybe.Just(new object())),
            };

            var results = specProps.Validate(SpecificationPropertyRules.Lookup(tokens, false));
            Assert.Contains(results, r => r.GetType() == typeof(RepeatedOptionError));
        }
    }
}
