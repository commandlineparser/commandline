using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using FluentAssertions;
using CSharpx;
using CommandLine.Core;

namespace CommandLine.Tests.Unit.Core
{
    public class TypeConverterTests
    {
        enum TestEnum
        {
            ValueA = 1,
            ValueB = 2
        }

        [Flags]
        enum TestFlagEnum
        {
            ValueA = 0x1,
            ValueB = 0x2
        }

        [Theory]
        [MemberData(nameof(ChangeType_scalars_source))]
        public void ChangeType_scalars(string testValue, Type destinationType, bool expectFail, object expectedResult)
        {
            Maybe<object> result = TypeConverter.ChangeType(new[] {testValue}, destinationType, true, false, CultureInfo.InvariantCulture, true);

            if (expectFail)
            {
                result.MatchNothing().Should().BeTrue("should fail parsing");
            }
            else
            {
                result.MatchJust(out object matchedValue).Should().BeTrue("should parse successfully");
                Assert.Equal(matchedValue, expectedResult);
            }
        }

        public static IEnumerable<object[]> ChangeType_scalars_source
        {
            get
            {
                return new[]
                {
                    new object[] {"1", typeof (int), false, 1},
                    new object[] {"0", typeof (int), false, 0},
                    new object[] {"-1", typeof (int), false, -1},
                    new object[] {"abcd", typeof (int), true, null},
                    new object[] {"1.0", typeof (int), true, null},
                    new object[] {int.MaxValue.ToString(), typeof (int), false, int.MaxValue},
                    new object[] {int.MinValue.ToString(), typeof (int), false, int.MinValue},
                    new object[] {((long) int.MaxValue + 1).ToString(), typeof (int), true, null},
                    new object[] {((long) int.MinValue - 1).ToString(), typeof (int), true, null},

                    new object[] {"1", typeof (uint), false, (uint) 1},
                   // new object[] {"0", typeof (uint), false, (uint) 0},  //cause warning: Skipping test case with duplicate ID
                   // new object[] {"-1", typeof (uint), true, null},  //cause warning: Skipping test case with duplicate ID
                    new object[] {uint.MaxValue.ToString(), typeof (uint), false, uint.MaxValue},
                    new object[] {uint.MinValue.ToString(), typeof (uint), false, uint.MinValue},
                    new object[] {((long) uint.MaxValue + 1).ToString(), typeof (uint), true, null},
                    new object[] {((long) uint.MinValue - 1).ToString(), typeof (uint), true, null},

                    new object[] {"true", typeof (bool), false, true},
                    new object[] {"True", typeof (bool), false, true},
                    new object[] {"TRUE", typeof (bool), false, true},
                    new object[] {"false", typeof (bool), false, false},
                    new object[] {"False", typeof (bool), false, false},
                    new object[] {"FALSE", typeof (bool), false, false},
                    new object[] {"abcd", typeof (bool), true, null},
                    new object[] {"0", typeof (bool), true, null},
                    new object[] {"1", typeof (bool), true, null},

                    new object[] {"1.0", typeof (float), false, 1.0f},
                    new object[] {"0.0", typeof (float), false, 0.0f},
                    new object[] {"-1.0", typeof (float), false, -1.0f},
                    new object[] {"abcd", typeof (float), true, null},

                    new object[] {"1.0", typeof (double), false, 1.0},
                    new object[] {"0.0", typeof (double), false, 0.0},
                    new object[] {"-1.0", typeof (double), false, -1.0},
                    new object[] {"abcd", typeof (double), true, null},

                    new object[] {"1.0", typeof (decimal), false, 1.0m},
                    new object[] {"0.0", typeof (decimal), false, 0.0m},
                    new object[] {"-1.0", typeof (decimal), false, -1.0m},
                    new object[] {"-1.123456", typeof (decimal), false, -1.123456m},
                    new object[] {"abcd", typeof (decimal), true, null},

                    new object[] {"", typeof (string), false, ""},
                    new object[] {"abcd", typeof (string), false, "abcd"},

                    new object[] {"ValueA", typeof (TestEnum), false, TestEnum.ValueA},
                    new object[] {"VALUEA", typeof (TestEnum), false, TestEnum.ValueA},
                    new object[] {"ValueB", typeof(TestEnum), false, TestEnum.ValueB},
                    new object[] {((int) TestEnum.ValueA).ToString(), typeof (TestEnum), false, TestEnum.ValueA},
                    new object[] {((int) TestEnum.ValueB).ToString(), typeof (TestEnum), false, TestEnum.ValueB},
                    new object[] {((int) TestEnum.ValueB + 1).ToString(), typeof (TestEnum), true, null},
                    new object[] {((int) TestEnum.ValueA - 1).ToString(), typeof (TestEnum), true, null},

                    new object[] {"ValueA", typeof (TestFlagEnum), false, TestFlagEnum.ValueA},
                    new object[] {"VALUEA", typeof (TestFlagEnum), false, TestFlagEnum.ValueA},
                    new object[] {"ValueB", typeof(TestFlagEnum), false, TestFlagEnum.ValueB},
                    new object[] {"ValueA,ValueB", typeof (TestFlagEnum), false, TestFlagEnum.ValueA | TestFlagEnum.ValueB},
                    new object[] {"ValueA, ValueB", typeof (TestFlagEnum), false, TestFlagEnum.ValueA | TestFlagEnum.ValueB},
                    new object[] {"VALUEA,ValueB", typeof (TestFlagEnum), false, TestFlagEnum.ValueA | TestFlagEnum.ValueB},
                    new object[] {((int) TestFlagEnum.ValueA).ToString(), typeof (TestFlagEnum), false, TestFlagEnum.ValueA},
                    new object[] {((int) TestFlagEnum.ValueB).ToString(), typeof (TestFlagEnum), false, TestFlagEnum.ValueB},
                    new object[] {((int) (TestFlagEnum.ValueA | TestFlagEnum.ValueB)).ToString(), typeof (TestFlagEnum), false, TestFlagEnum.ValueA | TestFlagEnum.ValueB},
                    new object[] {((int) TestFlagEnum.ValueB + 2).ToString(), typeof (TestFlagEnum), true, null},
                    new object[] {((int) TestFlagEnum.ValueA - 1).ToString(), typeof (TestFlagEnum), true, null},


                    // Failed before #339
                    new object[] {"false", typeof (int), true, 0},
                    new object[] {"true", typeof (int), true, 0}
                };
            }
        }

        [Theory]
        [MemberData(nameof(ChangeType_flagCounters_source))]
        public void ChangeType_flagCounters(string[] testValue, Type destinationType, bool expectFail, object expectedResult)
        {
            Maybe<object> result = TypeConverter.ChangeType(testValue, destinationType, true, true, CultureInfo.InvariantCulture, true);

            if (expectFail)
            {
                result.MatchNothing().Should().BeTrue("should fail parsing");
            }
            else
            {
                result.MatchJust(out object matchedValue).Should().BeTrue("should parse successfully");
                Assert.Equal(matchedValue, expectedResult);
            }
        }

        public static IEnumerable<object[]> ChangeType_flagCounters_source
        {
            get
            {
                return new[]
                {
                    new object[] {new string[0], typeof (int), false, 0},
                    new object[] {new[] {"true"}, typeof (int), false, 1},
                    new object[] {new[] {"true", "true"}, typeof (int), false, 2},
                    new object[] {new[] {"true", "true", "true"}, typeof (int), false, 3},
                    new object[] {new[] {"true", "x"}, typeof (int), true, 0},
                };
            }
        }

        [Fact]
        public void ChangeType_Scalar_LastOneWins()
        {
            var values = new[] { "100", "200", "300", "400", "500" };
            var result = TypeConverter.ChangeType(values, typeof(int), true, false, CultureInfo.InvariantCulture, true);
            result.MatchJust(out var matchedValue).Should().BeTrue("should parse successfully");
            Assert.Equal(500, matchedValue);

        }
    }
}
