// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

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
            if (newSpecification == null) throw new ArgumentNullException("newSpecification");

            return SpecificationProperty.Create(newSpecification, specProp.Property, specProp.Value);
        }

        public static SpecificationProperty WithValue(this SpecificationProperty specProp, Maybe<object> newValue)
        {
            if (newValue == null) throw new ArgumentNullException("newValue");

            return SpecificationProperty.Create(specProp.Specification, specProp.Property, newValue);
        }

        public static System.Type GetConversionType(this SpecificationProperty specProp)
        {
            switch (specProp.Specification.ConversionType.ToDescriptorKind())
            {
                case TypeDescriptorKind.Sequence:
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
            return rules.SelectMany(rule => rule(specProps));
        }
    }
}
