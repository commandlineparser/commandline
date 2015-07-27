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
    public enum NameStyleFormat
    {
        PreferLongName,
        PerferShortName,
    }

    public class UnParserSettings
    {
        private NameStyleFormat nameStyleFormat;
        private bool groupSwitches;
        private bool useEqualToken;

        public UnParserSettings()
        {
            nameStyleFormat = NameStyleFormat.PreferLongName;
        }

        public NameStyleFormat NameStyleFormat
        {
            get { return nameStyleFormat; }
            set { PopsicleSetter.Set(Consumed, ref nameStyleFormat, value); }
        }

        public bool GroupSwitches
        {
            get { return groupSwitches; }
            set { PopsicleSetter.Set(Consumed, ref groupSwitches, value); }
        }

        public bool UseEqualToken
        {
            get { return useEqualToken; }
            set { PopsicleSetter.Set(Consumed, ref useEqualToken, value); }
        }

        internal bool Consumed { get; set; }
    }

    /// <summary>
    /// Adds a method to unparse options instance.
    /// </summary>
    public static class UnParserExtensions
    {
        /// <summary>
        /// Format a command line argument string from a parsed instance. 
        /// </summary>
        /// <typeparam name="T">Type of <see cref="options"/>.</typeparam>
        /// <param name="parser">Parser instance.</param>
        /// <param name="options">A parsed (or manually correctly constructed instance).</param>
        /// <returns>A string with command line arguments.</returns>
        public static string FormatCommandLine<T>(this Parser parser, T options)
        {
            if (options == null) throw new ArgumentNullException("options");

            var type = options.GetType();
            var builder = new StringBuilder();

            type.GetVerbSpecification()
                .Return(verb => builder.Append(verb.Name).Append(' '), builder);

            var specs =
                (from info in
                    type.GetSpecifications(
                        pi => new { Specification = Specification.FromProperty(pi),
                            Value = pi.GetValue(options, null).NormalizeValue(), PropertyValue = pi.GetValue(options, null) })
                where !info.PropertyValue.IsEmpty()
                select info)
                    .Memorize();
            var optSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Option)
                let o = (OptionSpecification)info.Specification
                where o.TargetType != TargetType.Switch || (o.TargetType == TargetType.Switch && ((bool)info.Value))
                orderby o.UniqueName()
                select info;
            var valSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Value)
                let v = (ValueSpecification)info.Specification
                orderby v.Index
                select info;

            optSpecs.ForEach(
                opt => builder.Append(FormatOption((OptionSpecification)opt.Specification, opt.Value)).Append(' '));
            if (!valSpecs.Any() || builder.TrailingSpaces() > 1)
                builder.TrimEndIfMatch(' ');
            valSpecs.ForEach(
                val => builder.Append(FormatValue(val.Specification, val.Value)).Append(' '));

            return builder
                .ToString().TrimEnd(' ');
        }

        private static string FormatValue(Specification spec, object value)
        {
            var builder = new StringBuilder();
            switch (spec.TargetType)
            {
                case TargetType.Scalar:
                    builder.Append(FormatWithQuotesIfString(value));
                    break;
                case TargetType.Sequence:
                    var sep = spec.SeperatorOrSpace();
                    Func<object, object> format = v
                        => sep == ' ' ? FormatWithQuotesIfString(v) : v;
                    var e = ((IEnumerable)value).GetEnumerator();
                    while (e.MoveNext())
                        builder.Append(format(e.Current)).Append(sep);
                    builder.TrimEndIfMatch(' ');
                    break;
            }
            return builder.ToString();
        }

        private static object FormatWithQuotesIfString(object value)
        {
            Func<string, string> doubQt = v
                => v.Contains("\"") ? v.Replace("\"", "\\\"") : v;

            return (value as string)
                .ToMaybe()
                .Return(v => v.Contains(' ') || v.Contains("\"")
                    ? "\"".JoinTo(doubQt(v), "\"") : v, value);
        }

        private static char SeperatorOrSpace(this Specification spec)
        {
            return (spec as OptionSpecification).ToMaybe()
                .Return(o => o.Separator != '\0' ? o.Separator : ' ', ' ');
        }

        private static string FormatOption(OptionSpecification spec, object value)
        {
            return new StringBuilder()
                    .Append(spec.FormatName())
                    .Append(' ')
                    .AppendWhen(spec.TargetType != TargetType.Switch, FormatValue(spec, value))
                .ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec)
        {
            return optionSpec.LongName.Length > 0
                ? "--".JoinTo(optionSpec.LongName) : "-".JoinTo(optionSpec.ShortName);
        }

        private static object NormalizeValue(this object value)
        {
            if (value != null
                && ReflectionHelper.IsFSharpOptionType(value.GetType())
                && FSharpOptionHelper.IsSome(value))
            {
                return FSharpOptionHelper.ValueOf(value);
            }
            return value;
        }

        private static bool IsEmpty(this object value)
        {
            if (value == null) return true;
            if (ReflectionHelper.IsFSharpOptionType(value.GetType()) && !FSharpOptionHelper.IsSome(value)) return true;
            if (value is ValueType && value.Equals(value.GetType().GetDefaultValue())) return true;
            if (value is string && ((string)value).Length == 0) return true;
            if (value is IEnumerable && !((IEnumerable)value).GetEnumerator().MoveNext()) return true;
            return false;
        }
    }
}
