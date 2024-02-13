﻿// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private bool showHidden;
        private bool skipDefault;

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
        /// Gets or sets a value indicating whether unparsing process shall expose hidden options.
        /// </summary>
        public bool ShowHidden
        {
            get { return showHidden; }
            set { PopsicleSetter.Set(Consumed, ref showHidden, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether unparsing process shall skip options with DefaultValue.
        /// </summary>
        public bool SkipDefault
        {
            get { return skipDefault; }
            set { PopsicleSetter.Set(Consumed, ref skipDefault, value); }
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
        public static string FormatCommandLine<
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            T>(this Parser parser, T options)
        {
            return parser.FormatCommandLine(options, config => { });
        }

        /// <summary>
        /// Format a command line argument string from a parsed instance in the form of string[].
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="options"/>.</typeparam>
        /// <param name="parser">Parser instance.</param>
        /// <param name="options">A parsed (or manually correctly constructed instance).</param>
        /// <returns>A string[] with command line arguments.</returns>
        public static string[] FormatCommandLineArgs<
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            T>(this Parser parser, T options)
        {
            return parser.FormatCommandLine(options, config => { }).SplitArgs();
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
#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on method", "IL2072")]
#endif
        public static string FormatCommandLine<
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                    DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.Interfaces)]
#endif
                T>(
            this Parser parser,
            T options,
            Action<UnParserSettings> configuration)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(options);
#else
            if (options == null) throw new ArgumentNullException(nameof(options));
#endif

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
                         pi => new
                         {
                             Specification = Specification.FromProperty(pi),
                             Value = pi.GetValue(options, null).NormalizeValue(),
                             PropertyValue = pi.GetValue(options, null)
                         })
                 where !info.PropertyValue.IsEmpty(info.Specification, settings.SkipDefault)
                 select info)
                .Memoize();

            var allOptSpecs = from info in specs.Where(i => i.Specification.Tag == SpecificationType.Option)
                              let o = (OptionSpecification)info.Specification
                              where o.TargetType != TargetType.Switch ||
                                  (o.TargetType == TargetType.Switch && o.FlagCounter && ((int)info.Value > 0)) ||
                                  (o.TargetType == TargetType.Switch && ((bool)info.Value))
                              where !o.Hidden || settings.ShowHidden
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
                    info =>
                    {
                        var o = (OptionSpecification)info.Specification;
                        return o.FlagCounter
                            ? string.Concat(Enumerable.Repeat(o.ShortName, (int)info.Value))
                            : o.ShortName;
                    }).ToArray())).Append(' ')
                : builder;
            optSpecs.ForEach(
                opt =>
                    builder
                        .Append(FormatOption((OptionSpecification)opt.Specification, opt.Value, settings))
                        .Append(' ')
            );

            builder.AppendWhen(valSpecs.Any() && parser.Settings.EnableDashDash, "-- ");

            valSpecs.ForEach(
                val => builder.Append(FormatValue(val.Specification, val.Value)).Append(' '));

            return builder
                .ToString().TrimEnd(' ');
        }

        /// <summary>
        /// Format a command line argument string[] from a parsed instance.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="options"/>.</typeparam>
        /// <param name="parser">Parser instance.</param>
        /// <param name="options">A parsed (or manually correctly constructed instance).</param>
        /// <param name="configuration">The <see cref="Action{UnParserSettings}"/> lambda used to configure
        /// aspects and behaviors of the unparsersing process.</param>
        /// <returns>A string[] with command line arguments.</returns>
        public static string[] FormatCommandLineArgs<
#if NET8_0_OR_GREATER
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                    DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.Interfaces)]
#endif
                T>(
            this Parser parser,
            T options,
            Action<UnParserSettings> configuration)
        {
            return FormatCommandLine<T>(parser, options, configuration).SplitArgs();
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
                    builder.TrimEndIfMatch(sep);
                    break;
            }

            return builder.ToString();
        }

        private static object FormatWithQuotesIfString(object value)
        {
            string s = value.ToString();
            if (!string.IsNullOrEmpty(s) && !s.Contains("\"") && s.Contains(" "))
                return $"\"{s}\"";

            Func<string, string> doubQt = v
                => v.Contains("\"") ? v.Replace("\"", "\\\"") : v;

            return s.ToMaybe()
                .MapValueOrDefault(v => v.Contains(' ') || v.Contains("\"")
                    ? "\"".JoinTo(doubQt(v), "\"")
                    : v, value);
        }

        private static char SeperatorOrSpace(this Specification spec)
        {
            return (spec as OptionSpecification).ToMaybe()
                .MapValueOrDefault(o => o.Separator != '\0' ? o.Separator : ' ', ' ');
        }

        private static string FormatOption(OptionSpecification spec, object value, UnParserSettings settings)
        {
            return new StringBuilder()
                .Append(spec.FormatName(value, settings))
                .AppendWhen(spec.TargetType != TargetType.Switch, FormatValue(spec, value))
                .ToString();
        }

        private static string FormatName(this OptionSpecification optionSpec, object value, UnParserSettings settings)
        {
            // Have a long name and short name not preferred? Go with long!
            // No short name? Has to be long!
            var longName = (optionSpec.LongName.Length > 0 && !settings.PreferShortName)
             || optionSpec.ShortName.Length == 0;

            var formattedName =
                new StringBuilder(longName
                        ? "--".JoinTo(optionSpec.LongName)
                        : "-".JoinTo(optionSpec.ShortName))
                    .AppendWhen(optionSpec.TargetType != TargetType.Switch,
                        longName && settings.UseEqualToken ? "=" : " ")
                    .ToString();
            return optionSpec.FlagCounter
                ? string.Join(" ", Enumerable.Repeat(formattedName, (int)value))
                : formattedName;
        }

        private static object NormalizeValue(this object value)
        {
#if !SKIP_FSHARP
            if (value != null
                && ReflectionHelper.IsFSharpOptionType(value.GetType())
                && FSharpOptionHelper.IsSome(value))
            {
                return FSharpOptionHelper.ValueOf(value);
            }
#endif
            return value;
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Types are safe", "IL2072")]
#endif
        private static bool IsEmpty(
            this object value,
            Specification specification,
            bool skipDefault)
        {
            if (value == null) return true;

            if (skipDefault && value.Equals(specification.DefaultValue.FromJust())) return true;
            if (Nullable.GetUnderlyingType(specification.ConversionType) != null) return false; //nullable

#if !SKIP_FSHARP
            if (ReflectionHelper.IsFSharpOptionType(value.GetType()) && !FSharpOptionHelper.IsSome(value)) return true;
#endif
            if (value is ValueType && value.Equals(value.GetType().GetDefaultValue())) return true;
            if (value is string && ((string)value).Length == 0) return true;
            if (value is IEnumerable && !((IEnumerable)value).GetEnumerator().MoveNext()) return true;
            return false;
        }


        #region splitter

        /// <summary>
        /// Returns a string array that contains the substrings in this instance that are delimited by space considering string between double quote.
        /// </summary>
        /// <param name="command">the commandline string</param>
        /// <param name="keepQuote">don't remove the quote</param>
        /// <returns>a string array that contains the substrings in this instance</returns>
        public static string[] SplitArgs(this string command, bool keepQuote = false)
        {
            if (string.IsNullOrEmpty(command))
                return new string[0];

            var inQuote = false;
            var chars = command.ToCharArray().Select(v =>
            {
                if (v == '"')
                    inQuote = !inQuote;
                return !inQuote && v == ' ' ? '\n' : v;
            }).ToArray();

            return new string(chars).Split('\n')
                .Select(x => keepQuote ? x : x.Trim('"'))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        #endregion
    }
}
