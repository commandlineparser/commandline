// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal enum SpecificationType
    {
        Option,
        Value
    }

    internal enum TargetType
    {
        Switch,
        Scalar,
        Sequence
    }

    internal abstract class Specification
    {
        private readonly SpecificationType tag;
        private readonly bool required;
        private readonly Maybe<int> min;
        private readonly Maybe<int> max;
        private readonly Maybe<object> defaultValue;
        /// <summary>
        /// This information is denormalized to decouple Specification from PropertyInfo.
        /// </summary>
        private readonly Type conversionType;
        private readonly TargetType targetType;

        protected Specification(SpecificationType tag, bool required, Maybe<int> min, Maybe<int> max,
            Maybe<object> defaultValue, Type conversionType, TargetType targetType)
        {
            this.tag = tag;
            this.required = required;
            this.min = min;
            this.max = max;
            this.defaultValue = defaultValue;
            this.conversionType = conversionType;
            this.targetType = targetType;
        }

        public SpecificationType Tag 
        {
            get { return tag; }
        }

        public bool Required
        {
            get { return required; }
        }

        public Maybe<int> Min
        {
            get { return min; }
        }

        public Maybe<int> Max
        {
            get { return max; }
        }

        public Maybe<object> DefaultValue
        {
            get { return defaultValue; }
        }

        public Type ConversionType
        {
            get { return conversionType; }
        }

        public TargetType TargetType
        {
            get { return targetType; }
        }

        public static Specification FromProperty(PropertyInfo property)
        {       
            var attrs = property.GetCustomAttributes(true);
            var oa = attrs.OfType<OptionAttribute>();
            if (oa.Count() == 1)
            {
                var spec = OptionSpecification.FromAttribute(oa.Single(), property.PropertyType,
                    property.PropertyType.IsEnum
                        ? Enum.GetNames(property.PropertyType)
                        : Enumerable.Empty<string>());
                if (spec.ShortName.Length == 0 && spec.LongName.Length == 0)
                {
                    return spec.WithLongName(property.Name.ToLowerInvariant());
                }
                return spec;
            }

            var va = attrs.OfType<ValueAttribute>();
            if (va.Count() == 1)
            {
                return ValueSpecification.FromAttribute(va.Single(), property.PropertyType);
            }

            throw new InvalidOperationException();
        }
    }
}
