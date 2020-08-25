// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if PLATFORM_DOTNET
using System.Reflection;
#endif
using Xunit;
using CSharpx;
using RailwaySharp.ErrorHandling;
using CommandLine.Core;
using CommandLine.Tests.Fakes;

namespace CommandLine.Tests.Unit.Core
{
    public class OptionMapperTests
    {
        [Fact]
        public void Map_boolean_switch_creates_boolean_value()
        {
            // Fixture setup
            var tokenPartitions = new[]
                {
                    new KeyValuePair<string, IEnumerable<string>>("x", new [] { "true" })
                };
            var specProps = new[]
                {
                    SpecificationProperty.Create(
                        new OptionSpecification("x", string.Empty, false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', Maybe.Nothing<object>(), string.Empty, string.Empty, new List<string>(), typeof(bool), TargetType.Switch, string.Empty),
                        typeof(Simple_Options).GetProperties().Single(p => p.Name.Equals("BoolValue", StringComparison.Ordinal)),
                        Maybe.Nothing<object>())
                };

            // Exercize system 
            var result = OptionMapper.MapValues(
                specProps.Where(pt => pt.Specification.IsOption()),
                tokenPartitions,
                (vals, type, isScalar, isFlag) => TypeConverter.ChangeType(vals, type, isScalar, isFlag, CultureInfo.InvariantCulture, false),
                StringComparer.Ordinal
                );

            // Verify outcome
            Assert.NotNull(((Ok<IEnumerable<SpecificationProperty>, Error>)result).Success.Single(
                a => a.Specification.IsOption()
                && ((OptionSpecification)a.Specification).ShortName.Equals("x")
                && (bool)((Just<object>)a.Value).Value));

            // Teardown
        }

        [Fact]
        public void Map_with_multi_instance_scalar()
        {
            var tokenPartitions = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("s", new[] { "string1" }),
                new KeyValuePair<string, IEnumerable<string>>("shortandlong", new[] { "string2" }),
                new KeyValuePair<string, IEnumerable<string>>("shortandlong", new[] { "string3" }),
                new KeyValuePair<string, IEnumerable<string>>("s", new[] { "string4" }),
            };

            var specProps = new[]
            {
                SpecificationProperty.Create(
                    new OptionSpecification("s", "shortandlong", false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', Maybe.Nothing<object>(), string.Empty, string.Empty, new List<string>(), typeof(string), TargetType.Scalar, string.Empty),
                    typeof(Simple_Options).GetProperties().Single(p => p.Name.Equals(nameof(Simple_Options.ShortAndLong), StringComparison.Ordinal)),
                    Maybe.Nothing<object>()),
            };

            var result = OptionMapper.MapValues(
                specProps.Where(pt => pt.Specification.IsOption()),
                tokenPartitions,
                (vals, type, isScalar, isFlag) => TypeConverter.ChangeType(vals, type, isScalar, isFlag, CultureInfo.InvariantCulture, false),
                StringComparer.Ordinal);

            var property = result.SucceededWith().Single();
            Assert.True(property.Specification.IsOption());
            Assert.True(property.Value.MatchJust(out var stringVal));
            Assert.Equal(tokenPartitions.Last().Value.Last(), stringVal);
        }

        [Fact]
        public void Map_with_multi_instance_sequence()
        {
            var tokenPartitions = new[]
            {
                new KeyValuePair<string, IEnumerable<string>>("i", new [] { "1", "2" }),
                new KeyValuePair<string, IEnumerable<string>>("i", new [] { "3" }),
                new KeyValuePair<string, IEnumerable<string>>("i", new [] { "4", "5" }),
            };
            var specProps = new[]
            {
                SpecificationProperty.Create(
                    new OptionSpecification("i", string.Empty, false, string.Empty, Maybe.Nothing<int>(), Maybe.Nothing<int>(), '\0', Maybe.Nothing<object>(), string.Empty, string.Empty, new List<string>(), typeof(IEnumerable<int>), TargetType.Sequence, string.Empty),
                    typeof(Simple_Options).GetProperties().Single(p => p.Name.Equals(nameof(Simple_Options.IntSequence), StringComparison.Ordinal)),
                    Maybe.Nothing<object>())
            };

            var result = OptionMapper.MapValues(
                specProps.Where(pt => pt.Specification.IsOption()),
                tokenPartitions,
                (vals, type, isScalar, isFlag) => TypeConverter.ChangeType(vals, type, isScalar, isFlag, CultureInfo.InvariantCulture, false),
                StringComparer.Ordinal);

            var property = result.SucceededWith().Single();
            Assert.True(property.Specification.IsOption());
            Assert.True(property.Value.MatchJust(out var sequence));

            var expected = tokenPartitions.Aggregate(Enumerable.Empty<int>(), (prev, part) => prev.Concat(part.Value.Select(i => int.Parse(i))));
            Assert.Equal(expected, sequence);
        }
    }
}
