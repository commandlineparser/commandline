// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
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

        public static Maybe<TypeDescriptor> FindTypeDescriptorAndSibling(
            string name,
            IEnumerable<OptionSpecification> specifications,
            StringComparer comparer)
        {
            var nameIndex = specifications.IndexOf(a => name.MatchName(a.ShortName, a.LongName, comparer));
            if (nameIndex >= 0)
            {
                var infos = specifications.Skip(nameIndex).Take(2);
                if (infos.Any())
                {
                    var first = infos.First();
                    var info = TypeDescriptor.Create(first.TargetType, first.Max);
                    return infos.Skip(1).FirstOrDefault().ToMaybe()
                        .Map(second => info.WithNext(
                            Maybe.Just(TypeDescriptor.Create(second.TargetType, second.Max))));
                }
            }
            return Maybe.Nothing<TypeDescriptor>();
        }
    }
}