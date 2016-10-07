using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Core;
using CSharpx;
using FluentAssertions;
using Xunit;

namespace CommandLine.Tests.Unit.Core
{
    public class TypeConverterTests
    {
        [Theory]
        [MemberData("ChangeType_scalars_source")]
        public void ChangeType_scalars(string testValue, Type destinationType, CultureInfo culture, bool expectFail, object expectedResult)
        {
            Maybe<object> result = TypeConverter.ChangeType(new[] {testValue}, destinationType, true, culture, true);

            if (expectFail)
            {
                result.MatchNothing().Should().BeTrue();
            }
            else
            {
                object matchedValue;

                result.MatchJust(out matchedValue).Should().BeTrue();
                Assert.Equal(matchedValue, expectedResult);
            }
        }

        public static IEnumerable<object[]> ChangeType_scalars_source
        {
            get
            {
                yield return new object[] { "1", typeof (int), CultureInfo.InvariantCulture, false, 1};
                yield return new object[] { "0", typeof (int), CultureInfo.InvariantCulture, false, 0 };
                yield return new object[] { "-1", typeof (int), CultureInfo.InvariantCulture, false, -1 };
                yield return new object[] { "1.0", typeof (int), CultureInfo.InvariantCulture, true, null };
                yield return new object[] { "1.0", typeof(float), CultureInfo.InvariantCulture, false, 1.0f };
                yield return new object[] { "0.0", typeof (float), CultureInfo.InvariantCulture, false, 0.0f};
                yield return new object[] { "-1.0", typeof (float), CultureInfo.InvariantCulture, false, -1.0f};
                yield return new object[] { "1.0", typeof(double), CultureInfo.InvariantCulture, false, 1.0 };
                yield return new object[] { "0.0", typeof (double), CultureInfo.InvariantCulture, false, 0.0};
                yield return new object[] { "-1.0", typeof(double), CultureInfo.InvariantCulture, false, -1.0 };
                yield return new object[] { "1.0", typeof(decimal), CultureInfo.InvariantCulture, false, 1.0m };
                yield return new object[] { "0.0", typeof(decimal), CultureInfo.InvariantCulture, false, 0.0m };
                yield return new object[] { "-1.0", typeof(decimal), CultureInfo.InvariantCulture, false, -1.0m };
                yield return new object[] { "-1.123456", typeof(decimal), CultureInfo.InvariantCulture, false, -1.123456m };
                yield return new object[] { "true", typeof(bool), CultureInfo.InvariantCulture, false, true };
                yield return new object[] { "false", typeof (bool), CultureInfo.InvariantCulture, false, false };
                yield return new object[] { "", typeof(string), CultureInfo.InvariantCulture, false, "" };
                yield return new object[] { "abcd", typeof(int), CultureInfo.InvariantCulture, true, "abcd" };
                yield return new object[] { "abcd", typeof(string), CultureInfo.InvariantCulture, false, "abcd" };

                // Failed before change
                yield return new object[] { "false", typeof(int), CultureInfo.InvariantCulture, true, 0 };
                yield return new object[] { "true", typeof(int), CultureInfo.InvariantCulture, true, 0 };
            }
        }
    }
}
