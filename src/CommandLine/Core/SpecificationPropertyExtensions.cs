// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class SpecificationPropertyExtensions
    {
        public static SpecificationProperty WithSpecification(this SpecificationProperty specProp, Specification newSpecification)
        {
            if (specProp == null) throw new ArgumentNullException("specProp");
            if (newSpecification == null) throw new ArgumentNullException("newSpecification");

            return SpecificationProperty.Create(newSpecification, specProp.Property, specProp.Value);
        }

        public static SpecificationProperty WithValue(this SpecificationProperty specProp, Maybe<object> newValue)
        {
            if (specProp == null) throw new ArgumentNullException("specProp");
            if (newValue == null) throw new ArgumentNullException("newValue");

            return SpecificationProperty.Create(specProp.Specification, specProp.Property, newValue);
        }

        public static System.Type GetConversionType(this SpecificationProperty specProp)
        {
            if (specProp == null) throw new ArgumentNullException("specProp");

            switch (specProp.Specification.ConversionType.ToDescriptor())
            {
                case DescriptorType.Sequence:
                    return specProp.Property.PropertyType.GetGenericArguments()
                             .SingleOrDefault()
                             .ToMaybe()
                             .FromJust(
                                 new InvalidOperationException("Sequence properties should be of type IEnumerable<T>."));
                default:
                    return specProp.Property.PropertyType;
            }
        }

        public static IEnumerable<Maybe<Error>> Validate(
            this IEnumerable<SpecificationProperty> specProps,
            IEnumerable<Func<IEnumerable<SpecificationProperty>,
            IEnumerable<Maybe<Error>>>> rules)
        {
            if (specProps == null) throw new ArgumentNullException("specProps");

            return rules.SelectMany(rule => rule(specProps));
        }
    }
}
