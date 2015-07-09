// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    static class SpecificationPropertyRules
    {
        public static IEnumerable<Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>>>
            Lookup(
                IEnumerable<Token> tokens)
        {
            return new List<Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>>>
                {
                    EnforceMutuallyExclusiveSet(),
                    EnforceRequired(),
                    EnforceRange(),
                    EnforceSingle(tokens)
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceMutuallyExclusiveSet()
        {
            return specProps =>
                {
                    var options = specProps
                            .Where(sp => sp.Specification.IsOption())
                            .Where(sp => ((OptionSpecification)sp.Specification).SetName.Length > 0
                                   && sp.Value.IsJust());
                    var groups = options.GroupBy(g => ((OptionSpecification)g.Specification).SetName);
                    if (groups.Count() > 1)
                    {
                        return options.Select(s =>
                            new MutuallyExclusiveSetError(
                                ((OptionSpecification)s.Specification).FromOptionSpecification()));
                    }
                    return Enumerable.Empty<Error>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceRequired()
        {
            return specProps =>
            {
                var setsWithTrue =
                    specProps
                        .Where(sp => sp.Specification.IsOption()
                            && sp.Value.IsJust() && sp.Specification.Required)
                        .Select(s => ((OptionSpecification)s.Specification).SetName).ToList();
                
                var requiredButEmpty =
                    specProps
                        .Where(sp => sp.Specification.IsOption())
                        .Where(sp => sp.Value.IsNothing()
                            && sp.Specification.Required
                            && (
                                ((OptionSpecification)sp.Specification).SetName.Length == 0 ||
                                !setsWithTrue.Contains(((OptionSpecification)sp.Specification).SetName)
                            ))                          
                    .Concat(specProps
                        .Where(sp => sp.Specification.IsValue()
                            && sp.Value.IsNothing()
                            && sp.Specification.Required)).ToList();
                    if (requiredButEmpty.Any()) {
                        return requiredButEmpty.Select(s =>new MissingRequiredOptionError(
                            s.Specification.FromSpecification()));
                    }
                    return Enumerable.Empty<Error>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceRange()
        {
            return specProps =>
                {
                    var options = specProps.Where(
                        sp => sp.Specification.TargetType == TargetType.Sequence
                        && sp.Value.IsJust()
                        && (
                            (sp.Specification.Min.IsJust() && ((Array)sp.Value.FromJust()).Length < sp.Specification.Min.FromJust())
                            || (sp.Specification.Max.IsJust() && ((Array)sp.Value.FromJust()).Length > sp.Specification.Max.FromJust())
                        )
                    );
                    if (options.Any())
                    {
                        return options.Select(s => new SequenceOutOfRangeError(
                            s.Specification.FromSpecification()));
                    }
                    return Enumerable.Empty<Error>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceSingle(IEnumerable<Token> tokens)
        {
            return specProps =>
                {
                    var specs = from sp in specProps
                                where sp.Specification.IsOption() && sp.Value.IsJust()
                                select (OptionSpecification)sp.Specification;
                    var options = from t in tokens.Where(t => t.IsName())
                                  join o in specs on t.Text equals o.UniqueName() into to
                                  from o in to.DefaultIfEmpty()
                                  where o != null
                                  select new { o.ShortName, o.LongName };
                    var groups = from x in options
                                 group x by x into g
                                 let count = g.Count()
                                 select new { Value = g.Key, Count = count };
                    var errors = from y in groups
                                 where y.Count > 1
                                 select new RepeatedOptionError(new NameInfo(y.Value.ShortName, y.Value.LongName));
                    return errors;
                };
        }
    }
}