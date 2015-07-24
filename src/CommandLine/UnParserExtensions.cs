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
            if (options == null) throw new ArgumentNullException("options");

            var type = options.GetType();
            var builder = new StringBuilder();

            ReflectionHelper.GetAttribute<VerbAttribute>()
                .Return(verb => builder.Append(verb.Name).Append(' '), builder);

            var specs = type.GetSpecifications(pi =>
                Tuple.Create(Specification.FromProperty(pi),
                pi.GetValue(options, null)));
            var optSpecs = specs.OfType<Tuple<OptionSpecification, object>>()
                .Where(tuple => tuple.Item1.TargetType != TargetType.Switch && !((bool)tuple.Item2));
            var valSpecs = specs.OfType<Tuple<ValueSpecification, object>>().OrderBy(v => v.Item1.Index);

            optSpecs.ForEach(opt => builder.Append(FormatOption(opt)).Append(' '));
            builder.TrimEndIfMatch(' ');
            valSpecs.ForEach(val => builder.Append(FormatValue(val.Item1, val.Item2)).Append(' '));
            builder.TrimEndIfMatch(' ');

            return builder.ToString();
        }

        private static string FormatValue(Specification spec, object value)
        {
            var builder = new StringBuilder();
            switch (spec.TargetType)
            {
                case TargetType.Scalar:
                    builder.Append(UnParseValue(value));
                    builder.Append(' ');
                    break;
                case TargetType.Sequence:
                    var sep = spec.SeperatorOrSpace();
                    Func<object, object> unParse = v
                        => sep == ' ' ? UnParseValue(v) : v;
                    var e = ((IEnumerable)value).GetEnumerator();
                    while (e.MoveNext())
                        builder.Append(unParse(e.Current)).Append(sep);
                    if (builder[builder.Length] == ' ')
                        builder.Remove(0, builder.Length - 1);
                    break;
            }
            return builder.ToString();
        }

        private static object UnParseValue(object value)
        {
            Func<string, string> doubQt = v
                => v.Contains("\"") ? v.Replace("\"", "\\\"") : v;

            return (value as string)
                .ToMaybe()
                .Return(v => v.Contains(' ') ? "\"".JoinTo(doubQt(v), "\"") : v, value);
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
            return optionSpec.LongName.Length > 0
                ? "--".JoinTo(optionSpec.LongName) : "-".JoinTo(optionSpec.ShortName);
        }
    }
}
