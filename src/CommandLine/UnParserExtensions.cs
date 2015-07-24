// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandLine.Core;
using CommandLine.Infrastructure;
using CommandLine.Text; // TODO: move down StringBuilderExtensions type
using CSharpx;

namespace CommandLine
{
    /// <summary>
    /// Defines overloads to unparse options instance.
    /// </summary>
    public static class UnParserExtensions
    {
        public static string FormatCommandLine<T>(T options)
        {
            var specs = options.GetType().GetSpecifications(pi =>
                Tuple.Create(Specification.FromProperty(pi),
                pi.GetValue(options, null)));
            var optSpecs = specs.OfType<Tuple<OptionSpecification, object>>();
            var valSpecs = specs.OfType<Tuple<ValueSpecification, object>>().OrderBy(v => v.Item1.Index);

            var builder = new StringBuilder();
            optSpecs.ForEach(opt => builder.Append(FormatOption(opt)).Append(' '));
            builder.Remove(0, builder.Length);
            //value

            return builder.ToString();
        }

        private static string FormatOption(Tuple<OptionSpecification, object> optionSpec)
        {
            var spec = optionSpec.Item1;
            var value = optionSpec.Item2;
            return new StringBuilder()
                    .Append(spec.FormatName())
                    .Append(' ')
                    .Append(value)
                      .ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec)
        {
            return optionSpec.LongName.Length > 0 ? "--".JoinTo(optionSpec.LongName) : "-".JoinTo(optionSpec.ShortName);
        }
    }
}
