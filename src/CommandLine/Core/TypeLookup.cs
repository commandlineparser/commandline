// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class TypeLookup
    {
        public static Maybe<TypeDescriptor> FindTypeDescriptor(
            string name,
            IEnumerable<OptionSpecification> specifications,
            StringComparer comparer)
        {
            var info = specifications.SingleOrDefault(a => name.MatchName(a.ShortName, a.LongName, comparer))
                .ToMaybe()
                    .Map(
                        s => TypeDescriptor.Create(s.TargetType, s.Max));
            return info;
        }

        //private static TypeDescriptorKind ToDescriptorKind(this Type type)
        //{
        //    return type == typeof(bool)
        //               ? TypeDescriptorKind.Boolean
        //               : type == typeof(string)
        //                     ? TypeDescriptorKind.Scalar
        //                     : type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)
        //                           ? TypeDescriptorKind.Sequence
        //                           : TypeDescriptorKind.Scalar;
        //}
    }
}