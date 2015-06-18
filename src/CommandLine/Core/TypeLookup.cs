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
            if (name == null) throw new ArgumentNullException("name");
            if (specifications == null) throw new ArgumentNullException("specifications");
            if (comparer == null) throw new ArgumentNullException("comparer");

            var info = specifications.SingleOrDefault(a => name.MatchName(a.ShortName, a.LongName, comparer))
                .ToMaybe()
                    .Map(
                        s => TypeDescriptor.Create(
                            s.ConversionType.ToDescriptorKind(), (s.Min < 0 && s.Max < 0) ? Maybe.Nothing<int>() : Maybe.Just(s.Max)));
            return info;
        }
    }
}