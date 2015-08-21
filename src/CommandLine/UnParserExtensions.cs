// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

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
    /// Provides settings for when formatting command line from an options instance../>.
    /// </summary>
    public class UnParserSettings
    {
        private bool preferShortName;
        private bool groupSwitches;
        private bool useEqualToken;

        /// <summary>
        /// Gets or sets a value indicating whether unparsing process shall prefer short or long names.
        /// </summary>
        public bool PreferShortName
        {
            get { return preferShortName; }
            set { PopsicleSetter.Set(Consumed, ref preferShortName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether unparsing process shall group switches.
        /// </summary>
        public bool GroupSwitches
        {
            get { return groupSwitches; }
            set { PopsicleSetter.Set(Consumed, ref groupSwitches, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether unparsing process shall use equal sign with long names.
        /// </summary>
        public bool UseEqualToken
        {
            get { return useEqualToken; }
            set { PopsicleSetter.Set(Consumed, ref useEqualToken, value); }
        }

        /// <summary>
        /// Factory method that creates an instance of <see cref="CommandLine.UnParserSettings"/> with GroupSwitches set to true.
        /// </summary>
        /// <returns>A properly initalized <see cref="CommandLine.UnParserSettings"/> instance.</returns>
        public static UnParserSettings WithGroupSwitchesOnly()
        {
            return new UnParserSettings { GroupSwitches = true };
        }

        /// <summary>
        /// Factory method that creates an instance of <see cref="CommandLine.UnParserSettings"/> with UseEqualToken set to true.
        /// </summary>
        /// <returns>A properly initalized <see cref="CommandLine.UnParserSettings"/> instance.</returns>
        public static UnParserSettings WithUseEqualTokenOnly()
        {
            return new UnParserSettings { UseEqualToken = true };
        }

        internal bool Consumed { get; set; }
    }

    /// <summary>
    /// Provides overloads to unparse options instance.
    /// </summary>
    public static class UnParserExtensions
    {
        /// <summary>
        /// Format a command line argument string from a parsed instance. 
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="options"/>.</typeparam>
        /// <param name="parser">Parser instance.</param>
        /// <param name="options">A parsed (or manually correctly constructed instance).</param>
        /// <returns>A string with command line arguments.</returns>
        public static string FormatCommandLine<T>(this Parser parser, T options)
        {
            return parser.FormatCommandLine(options, config => {});
        }

        /// <summary>
        /// Format a command line argument string from a parsed instance. 
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="options"/>.</typeparam>
        /// <param name="parser">Parser instance.</param>
        /// <param name="options">A parsed (or manually correctly constructed instance).</param>
        /// <param name="configuration">The <see cref="Action{UnParserSettings}"/> lambda used to configure
        /// aspects and behaviors of the unparsersing process.</param>
        /// <returns>A string with command line arguments.</returns>
        public static string FormatCommandLine<T>(this Parser parser, T options, Action<UnParserSettings> configuration)
        {
            if (options == null) throw new ArgumentNullException("options");

            var settings = new UnParserSettings();
            configuration(settings);
            settings.Consumed = true;

            var type = options.GetType();
            var builder = new StringBuilder();

            type.GetVerbSpecification()
                .MapValueOrDefault(verb => builder.Append(verb.Name).Append(' '), builder);

            var specs =
                (from info in
                    type.GetSpecifications(
                        pi => new { Specification = Specification.FromProperty(pi),
                            Value = pi.GetValue(options, null).NormalizeValue(), PropertyValue = pi.GetValue(options, null) })
                where !info.PropertyValue.IsEmpty()
                select info)
                    .Memorize();

            var allOptSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Option)
                let o = (OptionSpecification)info.Specification
                where o.TargetType != TargetType.Switch || (o.TargetType == TargetType.Switch && ((bool)info.Value))
                orderby o.UniqueName()
                select info;

            var shortSwitches = from info in allOptSpecs
                let o = (OptionSpecification)info.Specification
                where o.TargetType == TargetType.Switch
                where o.ShortName.Length > 0
                orderby o.UniqueName()
                select info;

            var optSpecs = settings.GroupSwitches
                ? allOptSpecs.Where(info => !shortSwitches.Contains(info))
                : allOptSpecs;

            var valSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Value)
                let v = (ValueSpecification)info.Specification
                orderby v.Index
                select info;

            builder = settings.GroupSwitches && shortSwitches.Any()
                ? builder.Append('-').Append(string.Join(string.Empty, shortSwitches.Select(
                    info => ((OptionSpecification)info.Specification).ShortName).ToArray())).Append(' ')
                : builder;
            builder
                .TrimEndIfMatchWhen(!optSpecs.Any() || builder.TrailingSpaces() > 1, ' ');
            optSpecs.ForEach(
                opt =>
                    builder
                        .TrimEndIfMatchWhen(builder.TrailingSpaces() > 1, ' ')
                        .Append(FormatOption((OptionSpecification)opt.Specification, opt.Value, settings))
                        .Append(' ')
                );
            builder
                .TrimEndIfMatchWhen(!valSpecs.Any() || builder.TrailingSpaces() > 1, ' ');
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
                .MapValueOrDefault(v => v.Contains(' ') || v.Contains("\"")
                    ? "\"".JoinTo(doubQt(v), "\"") : v, value);
        }

        private static char SeperatorOrSpace(this Specification spec)
        {
            return (spec as OptionSpecification).ToMaybe()
                .MapValueOrDefault(o => o.Separator != '\0' ? o.Separator : ' ', ' ');
        }

        private static string FormatOption(OptionSpecification spec, object value, UnParserSettings settings)
        {
            return new StringBuilder()
                    .Append(spec.FormatName(settings))
                    .AppendWhen(spec.TargetType != TargetType.Switch, FormatValue(spec, value))
                .ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec, UnParserSettings settings)
        {
            var longName =
                optionSpec.LongName.Length > 0
                && !settings.PreferShortName;

            return
                new StringBuilder(longName
                    ? "--".JoinTo(optionSpec.LongName)
                    : "-".JoinTo(optionSpec.ShortName))
                        .AppendIf(longName && settings.UseEqualToken && optionSpec.ConversionType != typeof(bool), "=", " ")
                    .ToString();
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
