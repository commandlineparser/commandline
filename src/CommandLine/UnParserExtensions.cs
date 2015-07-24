// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections;
using System.Linq;
using System.Text;
using CommandLine.Core;
using CommandLine.Infrastructure;
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
            var optSpecs = specs.OfType<Tuple<OptionSpecification, object>>()
                .Where(tuple => tuple.Item1.TargetType != TargetType.Switch && !((bool)tuple.Item2));
            var valSpecs = specs.OfType<Tuple<ValueSpecification, object>>().OrderBy(v => v.Item1.Index);

            var builder = new StringBuilder();
            optSpecs.ForEach(opt => builder.Append(FormatOption(opt)).Append(' '));
            builder.Remove(0, builder.Length);
            //value

            return builder.ToString();
        }

        private static string FormatValue(Specification spec, object value)
        {
            var builder = new StringBuilder();
            switch (spec.TargetType)
            {
                case TargetType.Scalar:
                    builder.Append(value);
                    builder.Append(' ');
                    break;
                case TargetType.Sequence:
                    //var sep = 
                    var e = ((IEnumerable)value).GetEnumerator();
                    while (e.MoveNext())
                        builder.Append(e.Current).Append(' ');
                    break;
            }
            return builder.ToString();
        }

        private static char SeperatorOrSpace(this Specification spec)
        {
            return (spec as OptionSpecification).ToMaybe()
                .Return(o => o.Separator != '\0' ? o.Separator : ' ', ' ');
        }

        private static string FormatOption(Tuple<OptionSpecification, object> optionSpec)
        {
            var spec = optionSpec.Item1;
            var value = optionSpec.Item2;
            var builder = new StringBuilder()
                    .Append(spec.FormatName())
                    .Append(' ')
                    .Append(FormatValue(spec, value));
            if (builder[builder.Length] == ' ')
                builder.Remove(0, builder.Length - 1);
            return builder.ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec)
        {
            return optionSpec.LongName.Length > 0 ? "--".JoinTo(optionSpec.LongName) : "-".JoinTo(optionSpec.ShortName);
        }
    }
}
