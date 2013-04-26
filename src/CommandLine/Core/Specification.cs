// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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

    internal abstract class Specification
    {
        private readonly SpecificationType tag;
        private readonly bool required;
        private readonly int min;
        private readonly int max;
        private readonly Maybe<object> defaultValue;
        /// <summary>
        /// This information is denormalized to decouple Specification from PropertyInfo.
        /// </summary>
        private readonly System.Type conversionType;

        protected Specification(SpecificationType tag, bool required, int min, int max, Maybe<object> defaultValue, System.Type conversionType)
        {
            this.tag = tag;
            this.required = required;
            this.min = min;
            this.max = max;
            this.defaultValue = defaultValue;
            this.conversionType = conversionType;
        }

        public SpecificationType Tag 
        {
            get { return this.tag; }
        }

        public bool Required
        {
            get { return this.required; }
        }

        public int Min
        {
            get { return this.min; }
        }

        public int Max
        {
            get { return this.max; }
        }

        public Maybe<object> DefaultValue
        {
            get { return this.defaultValue; }
        }

        public System.Type ConversionType
        {
            get { return this.conversionType; }
        }

        public static Specification FromProperty(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes(true);
            var oa = attrs.OfType<OptionAttribute>();
            if (oa.Count() == 1)
            {
                var spec = OptionSpecification.FromAttribute(oa.Single(), property.PropertyType);
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
