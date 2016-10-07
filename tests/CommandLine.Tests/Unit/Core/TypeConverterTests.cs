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
        public void ChangeType_scalars(string testValue, Type destinationType, bool expectFail, object expectedResult)
        {
            Maybe<object> result = TypeConverter.ChangeType(new[] {testValue}, destinationType, true, CultureInfo.InvariantCulture, true);

            if (expectFail)
            {
                result.MatchNothing().Should().BeTrue("should fail parsing");
            }
            else
            {
                object matchedValue;

                result.MatchJust(out matchedValue).Should().BeTrue("should parse successfully");
                Assert.Equal(matchedValue, expectedResult);
            }
        }

        public static IEnumerable<object[]> ChangeType_scalars_source
        {
            get
            {
                yield return new object[] { "1", typeof (int), false, 1};
                yield return new object[] { "0", typeof (int), false, 0 };
                yield return new object[] { "-1", typeof (int), false, -1 };
                yield return new object[] { "abcd", typeof(int), true, null };
                yield return new object[] { "1.0", typeof(int), true, null };
                yield return new object[] { int.MaxValue.ToString(), typeof(int), false, int.MaxValue };
                yield return new object[] { int.MinValue.ToString(), typeof(int), false, int.MinValue };
                yield return new object[] { ((long)int.MaxValue + 1).ToString(), typeof(int), true, null };
                yield return new object[] { ((long)int.MinValue - 1).ToString(), typeof(int), true, null };

                yield return new object[] { "1", typeof(uint), false, (uint) 1 };
                yield return new object[] { "0", typeof(uint), false, (uint) 0 };
                yield return new object[] { "-1", typeof(uint), true, null };
                yield return new object[] { uint.MaxValue.ToString(), typeof(uint), false, uint.MaxValue };
                yield return new object[] { uint.MinValue.ToString(), typeof(uint), false, uint.MinValue };
                yield return new object[] { ((long)uint.MaxValue + 1).ToString(), typeof(uint), true, null };
                yield return new object[] { ((long)uint.MinValue - 1).ToString(), typeof(uint), true, null };

                yield return new object[] { "true", typeof(bool), false, true };
                yield return new object[] { "True", typeof(bool), false, true };
                yield return new object[] { "TRUE", typeof(bool), false, true };
                yield return new object[] { "false", typeof(bool), false, false };
                yield return new object[] { "False", typeof(bool), false, false };
                yield return new object[] { "FALSE", typeof(bool), false, false };
                yield return new object[] { "abcd", typeof(bool), true, null };
                yield return new object[] { "0", typeof(bool), true, null };
                yield return new object[] { "1", typeof(bool), true, null };

                yield return new object[] { "1.0", typeof(float), false, 1.0f };
                yield return new object[] { "0.0", typeof (float), false, 0.0f};
                yield return new object[] { "-1.0", typeof (float), false, -1.0f};
                yield return new object[] { "abcd", typeof(float), true, null };

                yield return new object[] { "1.0", typeof(double), false, 1.0 };
                yield return new object[] { "0.0", typeof (double), false, 0.0};
                yield return new object[] { "-1.0", typeof(double), false, -1.0 };
                yield return new object[] { "abcd", typeof(double), true, null };

                yield return new object[] { "1.0", typeof(decimal), false, 1.0m };
                yield return new object[] { "0.0", typeof(decimal), false, 0.0m };
                yield return new object[] { "-1.0", typeof(decimal), false, -1.0m };
                yield return new object[] { "-1.123456", typeof(decimal), false, -1.123456m };
                yield return new object[] { "abcd", typeof(decimal), true, null };

                yield return new object[] { "", typeof(string), false, "" };
                yield return new object[] { "abcd", typeof(string), false, "abcd" };

                // Failed before change
                yield return new object[] { "false", typeof(int), true, 0 };
                yield return new object[] { "true", typeof(int), true, 0 };
            }
        }
    }
}
