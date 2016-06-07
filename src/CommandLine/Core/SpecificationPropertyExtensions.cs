// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
#if PLATFORM_DOTNET
using System.Reflection;
#endif
using CSharpx;
using System.Reflection;

namespace CommandLine.Core
{
    static class SpecificationPropertyExtensions
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

        public static Type GetConversionType(this SpecificationProperty specProp)
        {
            switch (specProp.Specification.TargetType)
            {
                case TargetType.Sequence:
                    return specProp.Property.PropertyType.GetTypeInfo().GetGenericArguments()
                             .SingleOrDefault()
                             .ToMaybe()
                             .FromJustOrFail(
                                 new InvalidOperationException("Sequence properties should be of type IEnumerable<T>."));
                default:
                    return specProp.Property.PropertyType;
            }
        }

        public static IEnumerable<Error> Validate(
            this IEnumerable<SpecificationProperty> specProps,
            IEnumerable<Func<IEnumerable<SpecificationProperty>,
            IEnumerable<Error>>> rules)
        {
            return rules.SelectMany(rule => rule(specProps));
        }
    }
}
