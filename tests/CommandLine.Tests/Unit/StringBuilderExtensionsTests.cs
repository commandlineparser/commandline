using System.Collections.Generic;
using System.Text;
using Xunit;
using FluentAssertions;
using CommandLine.Infrastructure;

namespace CommandLine.Tests.Unit
{
    public class StringBuilderExtensionsTests
    {
        private static StringBuilder _sb = new StringBuilder("test string");
        private static StringBuilder _emptySb = new StringBuilder();
        private static StringBuilder _nullSb = null;

        public static IEnumerable<object[]> GoodStartsWithData => new []
        {
            new object[] { "t" },
            new object[] { "te" },
            new object[] { "test " },
            new object[] { "test string" }
        };

        public static IEnumerable<object[]> BadTestData => new []
        {
            new object[] { null },
            new object[] { "" },
            new object[] { "xyz" },
            new object[] { "some long test string" }
        };

        public static IEnumerable<object[]> GoodEndsWithData => new[]
        {
            new object[] { "g" },
            new object[] { "ng" },
            new object[] { " string" },
            new object[] { "test string" }
        };

        

        [Theory]
        [MemberData(nameof(GoodStartsWithData))]
        [MemberData(nameof(BadTestData))]
        public void StartsWith_null_builder_returns_false(string input)
        {
            _nullSb.SafeStartsWith(input).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GoodStartsWithData))]
        [MemberData(nameof(BadTestData))]
        public void StartsWith_empty_builder_returns_false(string input)
        {
            _emptySb.SafeStartsWith(input).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GoodStartsWithData))]
        public void StartsWith_good_data_returns_true(string input)
        {
            _sb.SafeStartsWith(input).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(BadTestData))]
        public void StartsWith_bad_data_returns_false(string input)
        {
            _sb.SafeStartsWith(input).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GoodEndsWithData))]
        [MemberData(nameof(BadTestData))]
        public void EndsWith_null_builder_returns_false(string input)
        {
            _nullSb.SafeEndsWith(input).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GoodEndsWithData))]
        [MemberData(nameof(BadTestData))]
        public void EndsWith_empty_builder_returns_false(string input)
        {
            _emptySb.SafeEndsWith(input).Should().BeFalse();
        }

        [Theory]
        [MemberData(nameof(GoodEndsWithData))]
        public void EndsWith_good_data_returns_true(string input)
        {
            _sb.SafeEndsWith(input).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(BadTestData))]
        public void EndsWith_bad_data_returns_false(string input)
        {
            _sb.SafeEndsWith(input).Should().BeFalse();
        }
    }
}
