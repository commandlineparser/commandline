using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using FluentAssertions;
using CSharpx;
using CommandLine.Core;
using System.ComponentModel;

namespace CommandLine.Tests.Unit.Core
{

    //DateOnly is NET6 class that is not registered with type converters, but it is usefull as commandline parameters
    //Converters cannot be removed, we need local class that can't interfere with anything else..
    public struct DateOnly
    {
        public DateTime RawDate { get; }

        public DateOnly(DateTime dateTime) : this()
        {
            RawDate = dateTime;
        }

        internal static DateOnly Parse(string v, CultureInfo culture)
        {
            return new DateOnly(DateTime.ParseExact(v, "d", culture));
        }

        internal string ToString(CultureInfo culture)
        {
            return RawDate.ToString("d", culture);
        }
    }

    //Same code as DateOnly, only used to check everything fail without registrations
    public struct DateOnlyNotregistered
    {
        public DateTime RawDate { get; }

        public DateOnlyNotregistered(DateTime dateTime) : this()
        {
            RawDate = dateTime;
        }

        internal static DateOnlyNotregistered Parse(string v, CultureInfo culture)
        {
            return new DateOnlyNotregistered(DateTime.ParseExact(v, "d", culture));
        }

        internal string ToString(CultureInfo culture)
        {
            return RawDate.ToString("d", culture);
        }
    }

    public class DateOnlyTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
          CultureInfo culture, object value)
        {
            if (value is string v)
                return DateOnly.Parse(v, culture);

            return base.ConvertFrom(context, culture, value);
        }


        public override object ConvertTo(ITypeDescriptorContext context,
          CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateOnly dt)
                return dt.ToString(culture);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class CustomTypeConverterTests
    {
        public CustomTypeConverterTests()
        {
            //There is now way to remove this once registered, used type should not interfere with anything else
            System.ComponentModel.TypeDescriptor.AddAttributes(typeof(DateOnly), new TypeConverterAttribute(typeof(DateOnlyTypeConverter)));
        }


        [Theory]
        [MemberData(nameof(DateOnly_Source))]
        public void Parse_DateOnly_without_registered_converter(string testValue, bool expectFail, object expectedResult)
        {
            try
            {
                Maybe<object> result = CommandLine.Core.TypeConverter.ChangeType(new[] { testValue }, typeof(DateOnlyNotregistered), true, false, CultureInfo.InvariantCulture, true);
                result.MatchNothing().Should().BeTrue("should fail every time without registered type converter");
            }
            catch (Exception ex)
            {
                ex.Should().BeOfType(typeof(NotSupportedException), "everything should fail parsing when type converter is not registered");
            }
        }

        [Theory]
        [MemberData(nameof(DateOnly_Source))]
        public void Parse_DateOnly(string testValue, bool expectFail, object expectedResult)
        {
            Maybe<object> result = CommandLine.Core.TypeConverter.ChangeType(new[] { testValue }, typeof(DateOnly), true, false, CultureInfo.InvariantCulture, true);

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

        public static IEnumerable<object[]> DateOnly_Source
        {
            get
            {
                return new[]
                {
                    new object[] {"07/03/2022", false, new DateOnly(new DateTime(2022,7,3))},
                    new object[] { DateTime.MinValue.ToString("d", CultureInfo.InvariantCulture), false, new DateOnly(DateTime.MinValue.Date)},
                    new object[] { DateTime.MaxValue.ToString("d", CultureInfo.InvariantCulture), false, new DateOnly(DateTime.MaxValue.Date)},
                    new object[] {"07/03/2022  15:45", true, null},
                    new object[] {"1234", true, null},
                    new object[] {"random", true, null},
                    new object[] {"2022-07-03T15:45:30", true, null},
                    new object[] {"", true, null},
                };
            }
        }
    }
}
