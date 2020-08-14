// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using CSharpx;

using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandLine.Core
{
    static class SpecificationPropertyRules
    {
        public static IEnumerable<Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>>>
            Lookup(
                IEnumerable<Token> tokens)
        {
            return Lookup(tokens, false);
        }

        public static IEnumerable<Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>>>
            Lookup(
                IEnumerable<Token> tokens,
                bool allowMultiInstance)
        {
            return new List<Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>>>
                {
                    EnforceMutuallyExclusiveSet(),
                    EnforceGroup(),
                    EnforceMutuallyExclusiveSetAndGroupAreNotUsedTogether(),
                    EnforceRequired(),
                    EnforceRange(),
                    EnforceSingle(tokens, allowMultiInstance)
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceMutuallyExclusiveSetAndGroupAreNotUsedTogether()
        {
            return specProps =>
            {
                var options =
                    from sp in specProps
                    where sp.Specification.IsOption()
                    let o = (OptionSpecification)sp.Specification
                    where o.SetName.Length > 0
                    where o.Group.Length > 0
                    select o;

                if (options.Any())
                {
                    return from o in options
                           select new GroupOptionAmbiguityError(new NameInfo(o.ShortName, o.LongName));
                }

                return Enumerable.Empty<Error>();
            };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceGroup()
        {
            return specProps =>
            {
                var optionsValues =
                    from sp in specProps
                    where sp.Specification.IsOption()
                    let o = (OptionSpecification)sp.Specification
                    where o.Group.Length > 0
                    select new
                    {
                        Option = o,
                        Value = sp.Value,
                        DefaultValue = sp.Specification.DefaultValue
                    };

                var groups = from o in optionsValues
                             group o by o.Option.Group into g
                             select g;

                var errorGroups = groups.Where(gr => gr.All(g => g.Value.IsNothing() && g.DefaultValue.IsNothing()));

                if (errorGroups.Any())
                {
                    return errorGroups.Select(gr => new MissingGroupOptionError(gr.Key, gr.Select(g => new NameInfo(g.Option.ShortName, g.Option.LongName))));
                }

                return Enumerable.Empty<Error>();
            };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceMutuallyExclusiveSet()
        {
            return specProps =>
            {
                var options =
                    from sp in specProps
                    where sp.Specification.IsOption()
                    where sp.Value.IsJust()
                    let o = (OptionSpecification)sp.Specification
                    where o.SetName.Length > 0
                    select o;
                var groups = from o in options
                             group o by o.SetName into g
                             select g;
                if (groups.Count() > 1)
                {
                    return
                        from o in options
                        select new MutuallyExclusiveSetError(o.FromOptionSpecification(), o.SetName);
                }
                return Enumerable.Empty<Error>();
            };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceRequired()
        {
            return specProps =>
            {
                var requiredWithValue = from sp in specProps
                                        where sp.Specification.IsOption()
                                        where sp.Specification.Required
                                        where sp.Value.IsJust()
                                        let o = (OptionSpecification)sp.Specification
                                        where o.SetName.Length > 0
                                        select sp.Specification;
                var setWithRequiredValue = (
                    from s in requiredWithValue
                    let o = (OptionSpecification)s
                    where o.SetName.Length > 0
                    select o.SetName)
                        .Distinct();
                var requiredWithoutValue = from sp in specProps
                                           where sp.Specification.IsOption()
                                           where sp.Specification.Required
                                           where sp.Value.IsNothing()
                                           let o = (OptionSpecification)sp.Specification
                                           where o.SetName.Length > 0
                                           where o.Group.Length == 0
                                           where setWithRequiredValue.ContainsIfNotEmpty(o.SetName)
                                           select sp.Specification;
                var missing =
                    requiredWithoutValue
                        .Except(requiredWithValue)
                        .Concat(
                            from sp in specProps
                            where sp.Specification.IsOption()
                            where sp.Specification.Required
                            where sp.Value.IsNothing()
                            let o = (OptionSpecification)sp.Specification
                            where o.SetName.Length == 0
                            where o.Group.Length == 0
                            select sp.Specification)
                        .Concat(
                            from sp in specProps
                            where sp.Specification.IsValue()
                            where sp.Specification.Required
                            where sp.Value.IsNothing()
                            select sp.Specification);
                return
                    from sp in missing
                    select new MissingRequiredOptionError(sp.FromSpecification());
            };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceRange()
        {
            return specProps =>
                {
                    var options = specProps
                        .Where(sp => sp.Specification.TargetType == TargetType.Sequence)
                        .Where(sp => sp.Value.IsJust())
                        .Where(sp =>
                            (sp.Specification.Min.IsJust() && ((Array)sp.Value.FromJustOrFail()).Length < sp.Specification.Min.FromJustOrFail())
                            || (sp.Specification.Max.IsJust() && ((Array)sp.Value.FromJustOrFail()).Length > sp.Specification.Max.FromJustOrFail())
                        );
                    if (options.Any())
                    {
                        return
                            from s in options
                            select new SequenceOutOfRangeError(s.Specification.FromSpecification());
                    }
                    return Enumerable.Empty<Error>();
                };
        }

        private static Func<IEnumerable<SpecificationProperty>, IEnumerable<Error>> EnforceSingle(IEnumerable<Token> tokens, bool allowMultiInstance)
        {
            return specProps =>
                {
                    if (allowMultiInstance)
                    {
                        return Enumerable.Empty<Error>();
                    }

                    var specs = from sp in specProps
                                where sp.Specification.IsOption()
                                where sp.Value.IsJust()
                                select (OptionSpecification)sp.Specification;
                    var shortOptions = from t in tokens
                                       where t.IsName()
                                       join o in specs on t.Text equals o.ShortName into to
                                       from o in to.DefaultIfEmpty()
                                       where o != null
                                       select new { o.ShortName, o.LongName };
                    var longOptions = from t in tokens
                                      where t.IsName()
                                      join o in specs on t.Text equals o.LongName into to
                                      from o in to.DefaultIfEmpty()
                                      where o != null
                                      select new { o.ShortName, o.LongName };
                    var groups = from x in shortOptions.Concat(longOptions)
                                 group x by x into g
                                 let count = g.Count()
                                 select new { Value = g.Key, Count = count };
                    var errors = from y in groups
                                 where y.Count > 1
                                 select new RepeatedOptionError(new NameInfo(y.Value.ShortName, y.Value.LongName));
                    return errors;
                };
        }

        private static bool ContainsIfNotEmpty<T>(this IEnumerable<T> sequence, T value)
        {
            if (sequence.Any())
            {
                return sequence.Contains(value);
            }
            return true;
        }
    }
}
