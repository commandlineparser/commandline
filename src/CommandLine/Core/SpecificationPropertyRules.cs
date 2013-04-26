// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    static class SpecificationPropertyRules
    {
        public static readonly IEnumerable<Func<IEnumerable<SpecificationProperty>, IEnumerable<Maybe<Error>>>> Lookup = new List<Func<IEnumerable<SpecificationProperty>, IEnumerable<Maybe<Error>>>>
            {
                EnforceMutuallyExclusiveSet(),
                EnforceRequired(),
                EnforceRange()
            };

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Maybe<Error>>> EnforceMutuallyExclusiveSet()
        {
            return specProps =>
                {
                    var options = specProps.Where(sp => sp.Specification.IsOption() && sp.Value.IsJust());
                    var groups = options.GroupBy(g => ((OptionSpecification)g.Specification).SetName);
                    if (groups.Count() > 1)
                    {
                        return options.Select(s => Maybe.Just<Error>(new MutuallyExclusiveSetError(NameInfo.FromOptionSpecification((OptionSpecification)s.Specification))));
                    }
                    return Enumerable.Empty<Nothing<Error>>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Maybe<Error>>> EnforceRequired()
        {
            return specProps =>
                {
                    var options = specProps.Where(sp => sp.Value.IsNothing() && sp.Specification.Required);
                    if (options.Any())
                    {
                        return options.Select(s => Maybe.Just<Error>(new MissingRequiredOptionError(
                            NameInfo.FromSpecification(s.Specification))));
                    }
                    return Enumerable.Empty<Nothing<Error>>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Maybe<Error>>> EnforceRange()
        {
            return specProps =>
                {
                    var options = specProps.Where(
                        sp => sp.Specification.ConversionType.ToDescriptor() == DescriptorType.Sequence
                        && sp.Value.IsJust()
                        && ((Array)sp.Value.FromJust()).Length < sp.Specification.Min);
                    if (options.Any())
                    {
                        return options.Select(s => Maybe.Just<Error>(new SequenceOutOfRangeError(
                            NameInfo.FromSpecification(s.Specification))));
                    }
                    return Enumerable.Empty<Nothing<Error>>();
                };
        }
    }
}