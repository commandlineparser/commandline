// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Core;
using CSharpx;

namespace CommandLine.Text
{
    /// <summary>
    /// Provides means to format an help screen.
    /// You can assign it in place of a <see cref="System.String"/> instance.
    /// </summary>
    public class HelpText
    {
        private const int BuilderCapacity = 128;
        private const int DefaultMaximumLength = 80; // default console width
        private readonly StringBuilder preOptionsHelp;
        private readonly StringBuilder postOptionsHelp;
        private readonly SentenceBuilder sentenceBuilder;
        private int maximumDisplayWidth;
        private string heading;
        private string copyright;
        private bool additionalNewLineAfterOption;
        private StringBuilder optionsHelp;
        private bool addDashesToOption;
        private bool addEnumValuesToHelpText;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class.
        /// </summary>
        public HelpText()
            : this(SentenceBuilder.Create(), string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class 
        /// specifying the sentence builder.
        /// </summary>
        /// <param name="sentenceBuilder">
        /// A <see cref="SentenceBuilder"/> instance.
        /// </param>
        public HelpText(SentenceBuilder sentenceBuilder)
            : this(sentenceBuilder, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading string.
        /// </summary>
        /// <param name="heading">An heading string or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="heading"/> is null or empty string.</exception>
        public HelpText(string heading)
            : this(SentenceBuilder.Create(), heading, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying the sentence builder and heading string.
        /// </summary>
        /// <param name="sentenceBuilder">A <see cref="SentenceBuilder"/> instance.</param>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        public HelpText(SentenceBuilder sentenceBuilder, string heading)
            : this(sentenceBuilder, heading, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one or more parameters are null or empty strings.</exception>
        public HelpText(string heading, string copyright)
            : this(SentenceBuilder.Create(), heading, copyright)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="sentenceBuilder">A <see cref="SentenceBuilder"/> instance.</param>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when one or more parameters are null or empty strings.</exception>
        public HelpText(SentenceBuilder sentenceBuilder, string heading, string copyright)
        {
            if (sentenceBuilder == null) throw new ArgumentNullException("sentenceBuilder");
            if (heading == null) throw new ArgumentNullException("heading");
            if (copyright == null) throw new ArgumentNullException("copyright");

            preOptionsHelp = new StringBuilder(BuilderCapacity);
            postOptionsHelp = new StringBuilder(BuilderCapacity);
            try
            {
                maximumDisplayWidth = Console.WindowWidth;
            }
            catch (IOException)
            {
                maximumDisplayWidth = DefaultMaximumLength;
            }
            this.sentenceBuilder = sentenceBuilder;
            this.heading = heading;
            this.copyright = copyright;
        }

        /// <summary>
        /// Gets or sets the heading string.
        /// You can directly assign a <see cref="CommandLine.Text.HeadingInfo"/> instance.
        /// </summary>
        public string Heading
        {
            get { return heading; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                heading = value;
            }
        }

        /// <summary>
        /// Gets or sets the copyright string.
        /// You can directly assign a <see cref="CommandLine.Text.CopyrightInfo"/> instance.
        /// </summary>
        public string Copyright
        {
            get { return copyright; }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                copyright = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        /// <value>The maximum width of the display.</value>
        public int MaximumDisplayWidth
        {
            get { return maximumDisplayWidth; }
            set { maximumDisplayWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format of options should contain dashes.
        /// It modifies behavior of <see cref="AddOptions{T}(ParserResult{T})"/> method.
        /// </summary>
        public bool AddDashesToOption
        {
            get { return addDashesToOption; }
            set { addDashesToOption = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add an additional line after the description of the specification.
        /// </summary>
        public bool AdditionalNewLineAfterOption
        {
            get { return additionalNewLineAfterOption; }
            set { additionalNewLineAfterOption = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add the values of an enum after the description of the specification.
        /// </summary>
        public bool AddEnumValuesToHelpText
        {
            get { return addEnumValuesToHelpText; }
            set { addEnumValuesToHelpText = value; }
        }

        /// <summary>
        /// Gets the <see cref="SentenceBuilder"/> instance specified in constructor.
        /// </summary>
        public SentenceBuilder SentenceBuilder
        {
            get { return sentenceBuilder; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name='onError'>A delegate used to customize the text block of reporting parsing errors text block.</param>
        /// <param name='onExample'>A delegate used to customize <see cref="CommandLine.Text.Example"/> model used to render text block of usage examples.</param>
        /// <param name="verbsIndex">If true the output style is consistent with verb commands (no dashes), otherwise it outputs options.</param>
        /// <param name="maxDisplayWidth">The maximum width of the display.</param>
        /// <remarks>The parameter <paramref name="verbsIndex"/> is not ontly a metter of formatting, it controls whether to handle verbs or options.</remarks>
        public static HelpText AutoBuild<T>(
            ParserResult<T> parserResult,
            Func<HelpText, HelpText> onError,
            Func<Example, Example> onExample,
            bool verbsIndex = false,
            int maxDisplayWidth = DefaultMaximumLength)
        {
            var auto = new HelpText
            {
                Heading = HeadingInfo.Empty,
                Copyright = CopyrightInfo.Empty,
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = !verbsIndex,
                MaximumDisplayWidth = maxDisplayWidth
            };

            try
            {
                auto.Heading = HeadingInfo.Default;
                auto.Copyright = CopyrightInfo.Default;
            }
            catch (Exception)
            {
                auto = onError(auto);
            }

            var errors = Enumerable.Empty<Error>();

            if (onError != null && parserResult.Tag == ParserResultType.NotParsed)
            {
                errors = ((NotParsed<T>)parserResult).Errors;

                if (errors.OnlyMeaningfulOnes().Any())
                    auto = onError(auto);
            }

            ReflectionHelper.GetAttribute<AssemblyLicenseAttribute>()
                .Do(license => license.AddToHelpText(auto, true));

            var usageAttr = ReflectionHelper.GetAttribute<AssemblyUsageAttribute>();
            var usageLines = HelpText.RenderUsageTextAsLines(parserResult, onExample).ToMaybe();

            if (usageAttr.IsJust() || usageLines.IsJust())
            {
                var heading = auto.SentenceBuilder.UsageHeadingText();
                if (heading.Length > 0)
                    auto.AddPreOptionsLine(heading);
            }

            usageAttr.Do(
                usage => usage.AddToHelpText(auto, true));

            usageLines.Do(
                lines => auto.AddPreOptionsLines(lines));

            if ((verbsIndex && parserResult.TypeInfo.Choices.Any())
                || errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
            {
                auto.AddDashesToOption = false;
                auto.AddVerbs(parserResult.TypeInfo.Choices.ToArray());
            }
            else
                auto.AddOptions(parserResult);

            return auto;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class,
        /// automatically handling verbs or options scenario.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name="maxDisplayWidth">The maximum width of the display.</param>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <remarks>This feature is meant to be invoked automatically by the parser, setting the HelpWriter property
        /// of <see cref="CommandLine.ParserSettings"/>.</remarks>
        public static HelpText AutoBuild<T>(ParserResult<T> parserResult, int maxDisplayWidth = DefaultMaximumLength)
        {
            if (parserResult.Tag != ParserResultType.NotParsed)
                throw new ArgumentException("Excepting NotParsed<T> type.", "parserResult");

            var errors = ((NotParsed<T>)parserResult).Errors;

            if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
                return new HelpText(HeadingInfo.Default){MaximumDisplayWidth = maxDisplayWidth }.AddPreOptionsLine(Environment.NewLine);

            if (!errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError))
                return AutoBuild(parserResult, current => DefaultParsingErrorsHandler(parserResult, current), e => e, maxDisplayWidth: maxDisplayWidth);

            var err = errors.OfType<HelpVerbRequestedError>().Single();
            var pr = new NotParsed<object>(TypeInfo.Create(err.Type), Enumerable.Empty<Error>());
            return err.Matched
                ? AutoBuild(pr, current => DefaultParsingErrorsHandler(pr, current), e => e, maxDisplayWidth: maxDisplayWidth)
                : AutoBuild(parserResult, current => DefaultParsingErrorsHandler(parserResult, current), e => e, true, maxDisplayWidth);
        }

        /// <summary>
        /// Supplies a default parsing error handler implementation.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name="current">The <see cref="CommandLine.Text.HelpText"/> instance.</param>
        public static HelpText DefaultParsingErrorsHandler<T>(ParserResult<T> parserResult, HelpText current)
        {
            if (parserResult == null) throw new ArgumentNullException("parserResult");
            if (current == null) throw new ArgumentNullException("current");

            if (((NotParsed<T>)parserResult).Errors.OnlyMeaningfulOnes().Empty())
                return current;

            var errors = RenderParsingErrorsTextAsLines(parserResult,
                current.SentenceBuilder.FormatError,
                current.SentenceBuilder.FormatMutuallyExclusiveSetErrors,
                2); // indent with two spaces
            if (errors.Empty())
                return current;

            return current
                .AddPreOptionsLine(
                    string.Concat(Environment.NewLine, current.SentenceBuilder.ErrorsHeadingText()))
                .AddPreOptionsLines(errors);
        }

        /// <summary>
        /// Converts the help instance to a <see cref="System.String"/>.
        /// </summary>
        /// <param name="info">This <see cref="CommandLine.Text.HelpText"/> instance.</param>
        /// <returns>The <see cref="System.String"/> that contains the help screen.</returns>
        public static implicit operator string(HelpText info)
        {
            return info.ToString();
        }

        /// <summary>
        /// Adds a text line after copyright and before options usage strings.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public HelpText AddPreOptionsLine(string value)
        {
            return AddPreOptionsLine(value, MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text line at the bottom, after options usage string.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public HelpText AddPostOptionsLine(string value)
        {
            return AddLine(postOptionsHelp, value);
        }

        /// <summary>
        /// Adds text lines after copyright and before options usage strings.
        /// </summary>
        /// <param name="lines">A <see cref="System.String"/> sequence of line to add.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        public HelpText AddPreOptionsLines(IEnumerable<string> lines)
        {
            lines.ForEach(line => AddPreOptionsLine(line));
            return this;
        }

        /// <summary>
        /// Adds text lines at the bottom, after options usage string.
        /// </summary>
        /// <param name="lines">A <see cref="System.String"/> sequence of line to add.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        public HelpText AddPostOptionsLines(IEnumerable<string> lines)
        {
            lines.ForEach(line => AddPostOptionsLine(line));
            return this;
        }

        /// <summary>
        /// Adds a text block of lines after copyright and before options usage strings.
        /// </summary>
        /// <param name="text">A <see cref="System.String"/> text block.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        public HelpText AddPreOptionsText(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines.ForEach(line => AddPreOptionsLine(line));
            return this;
        }

        /// <summary>
        /// Adds a text block of lines at the bottom, after options usage string.
        /// </summary>
        /// <param name="text">A <see cref="System.String"/> text block.</param>
        /// <returns>Updated <see cref="CommandLine.Text.HelpText"/> instance.</returns>
        public HelpText AddPostOptionsText(string text)
        {
            var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            lines.ForEach(line => AddPostOptionsLine(line));
            return this;
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="result">A parsing computation result.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="result"/> is null.</exception>
        public HelpText AddOptions<T>(ParserResult<T> result)
        {
            if (result == null) throw new ArgumentNullException("result");

            return AddOptionsImpl(
                GetSpecificationsFromType(result.TypeInfo.Current),
                SentenceBuilder.RequiredWord(),
                MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text block with verbs usage string.
        /// </summary>
        /// <param name="types">The array of <see cref="System.Type"/> with verb commands.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="types"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="types"/> array is empty.</exception>
        public HelpText AddVerbs(params Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0) throw new ArgumentOutOfRangeException("types");

            return AddOptionsImpl(
                AdaptVerbsToSpecifications(types),
                SentenceBuilder.RequiredWord(),
                MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="maximumLength">The maximum length of the help screen.</param>
        /// <param name="result">A parsing computation result.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="result"/> is null.</exception>    
        public HelpText AddOptions<T>(int maximumLength, ParserResult<T> result)
        {
            if (result == null) throw new ArgumentNullException("result");

            return AddOptionsImpl(
                GetSpecificationsFromType(result.TypeInfo.Current),
                SentenceBuilder.RequiredWord(),
                maximumLength);
        }

        /// <summary>
        /// Adds a text block with verbs usage string.
        /// </summary>
        /// <param name="maximumLength">The maximum length of the help screen.</param>
        /// <param name="types">The array of <see cref="System.Type"/> with verb commands.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="types"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="types"/> array is empty.</exception>
        public HelpText AddVerbs(int maximumLength, params Type[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0) throw new ArgumentOutOfRangeException("types");

            return AddOptionsImpl(
                AdaptVerbsToSpecifications(types),
                SentenceBuilder.RequiredWord(),
                maximumLength);
        }

        /// <summary>
        /// Builds a string that contains a parsing error message.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name="formatError">The error formatting delegate.</param>
        /// <param name="formatMutuallyExclusiveSetErrors">The specialized <see cref="CommandLine.MutuallyExclusiveSetError"/> sequence formatting delegate.</param>
        /// <param name="indent">Number of spaces used to indent text.</param>
        /// <returns>The <see cref="System.String"/> that contains the parsing error message.</returns>
        public static string RenderParsingErrorsText<T>(
            ParserResult<T> parserResult,
            Func<Error, string> formatError,
            Func<IEnumerable<MutuallyExclusiveSetError>, string> formatMutuallyExclusiveSetErrors,
            int indent)
        {
            return string.Join(
                Environment.NewLine,
                RenderParsingErrorsTextAsLines(parserResult, formatError, formatMutuallyExclusiveSetErrors, indent));
        }

        /// <summary>
        /// Builds a sequence of string that contains a parsing error message.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name="formatError">The error formatting delegate.</param>
        /// <param name="formatMutuallyExclusiveSetErrors">The specialized <see cref="CommandLine.MutuallyExclusiveSetError"/> sequence formatting delegate.</param>
        /// <param name="indent">Number of spaces used to indent text.</param>
        /// <returns>A sequence of <see cref="System.String"/> that contains the parsing error message.</returns>
        public static IEnumerable<string> RenderParsingErrorsTextAsLines<T>(
            ParserResult<T> parserResult,
            Func<Error, string> formatError,
            Func<IEnumerable<MutuallyExclusiveSetError>, string> formatMutuallyExclusiveSetErrors,
            int indent)
        {
            if (parserResult == null) throw new ArgumentNullException("parserResult");

            var meaningfulErrors =
                ((NotParsed<T>)parserResult).Errors.OnlyMeaningfulOnes();
            if (meaningfulErrors.Empty())
                yield break;

            foreach(var error in  meaningfulErrors
                .Where(e => e.Tag != ErrorType.MutuallyExclusiveSetError))
            {
                var line = new StringBuilder(indent.Spaces())
                    .Append(formatError(error));
                yield return line.ToString();
            }

            var mutuallyErrs =
                formatMutuallyExclusiveSetErrors(
                    meaningfulErrors.OfType<MutuallyExclusiveSetError>());
            if (mutuallyErrs.Length > 0)
            {
                var lines = mutuallyErrs
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in lines)
                    yield return line;
            }
        }

        /// <summary>
        /// Builds a string with usage text block created using <see cref="CommandLine.Text.UsageAttribute"/> data and metadata.
        /// </summary>
        /// <typeparam name="T">Type of parsing computation result.</typeparam>
        /// <param name="parserResult">A parsing computation result.</param>
        /// <returns>Resulting formatted text.</returns>
        public static string RenderUsageText<T>(ParserResult<T> parserResult)
        {
            return RenderUsageText(parserResult, example => example);
        }

        /// <summary>
        /// Builds a string with usage text block created using <see cref="CommandLine.Text.UsageAttribute"/> data and metadata.
        /// </summary>
        /// <typeparam name="T">Type of parsing computation result.</typeparam>
        /// <param name="parserResult">A parsing computation result.</param>
        /// <param name="mapperFunc">A mapping lambda normally used to translate text in other languages.</param>
        /// <returns>Resulting formatted text.</returns>
        public static string RenderUsageText<T>(ParserResult<T> parserResult, Func<Example, Example> mapperFunc)
        {
            return string.Join(Environment.NewLine, RenderUsageTextAsLines(parserResult, mapperFunc));
        }

        /// <summary>
        /// Builds a string sequence with usage text block created using <see cref="CommandLine.Text.UsageAttribute"/> data and metadata.
        /// </summary>
        /// <typeparam name="T">Type of parsing computation result.</typeparam>
        /// <param name="parserResult">A parsing computation result.</param>
        /// <param name="mapperFunc">A mapping lambda normally used to translate text in other languages.</param>
        /// <returns>Resulting formatted text.</returns>
        public static IEnumerable<string> RenderUsageTextAsLines<T>(ParserResult<T> parserResult, Func<Example, Example> mapperFunc)
        {
            if (parserResult == null) throw new ArgumentNullException("parserResult");

            var usage = GetUsageFromType(parserResult.TypeInfo.Current);
            if (usage.MatchNothing())
                yield break;

            var usageTuple = usage.FromJustOrFail();
            var examples = usageTuple.Item2;
            var appAlias = usageTuple.Item1.ApplicationAlias ?? ReflectionHelper.GetAssemblyName();

            foreach (var e in examples)
            {
                var example = mapperFunc(e);
                var exampleText = new StringBuilder(example.HelpText)
                    .Append(':');
                yield return exampleText.ToString();
                var styles = example.GetFormatStylesOrDefault();
                foreach (var s in styles)
                {
                    var commandLine = new StringBuilder(2.Spaces())
                        .Append(appAlias)
                        .Append(' ')
                        .Append(Parser.Default.FormatCommandLine(example.Sample,
                            config =>
                            {
                                config.PreferShortName = s.PreferShortName;
                                config.GroupSwitches = s.GroupSwitches;
                                config.UseEqualToken = s.UseEqualToken;
                            }));
                    yield return commandLine.ToString();
                }
            }
        }

        /// <summary>
        /// Returns the help screen as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the help screen.</returns>
        public override string ToString()
        {
            const int ExtraLength = 10;
            return
                new StringBuilder(
                    heading.SafeLength() + copyright.SafeLength() + preOptionsHelp.SafeLength() +
                        optionsHelp.SafeLength() + ExtraLength).Append(heading)
                    .AppendWhen(!string.IsNullOrEmpty(copyright), Environment.NewLine, copyright)
                    .AppendWhen(preOptionsHelp.Length > 0, Environment.NewLine, preOptionsHelp.ToString())
                    .AppendWhen(
                        optionsHelp != null && optionsHelp.Length > 0,
                        Environment.NewLine,
                        Environment.NewLine,
                        optionsHelp.SafeToString())
                    .AppendWhen(postOptionsHelp.Length > 0, Environment.NewLine, postOptionsHelp.ToString())
                .ToString();
        }

        internal static void AddLine(StringBuilder builder, string value, int maximumLength)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (maximumLength < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            value = value.Trim();

            builder.AppendWhen(builder.Length > 0, Environment.NewLine);
            do
            {
                var wordBuffer = 0;
                var words = value.Split(' ');
                for (var i = 0; i < words.Length; i++)
                {
                    if (words[i].Length < (maximumLength - wordBuffer))
                    {
                        builder.Append(words[i]);
                        wordBuffer += words[i].Length;
                        if ((maximumLength - wordBuffer) > 1 && i != words.Length - 1)
                        {
                            builder.Append(" ");
                            wordBuffer++;
                        }
                    }
                    else if (words[i].Length >= maximumLength && wordBuffer == 0)
                    {
                        builder.Append(words[i].Substring(0, maximumLength));
                        wordBuffer = maximumLength;
                        break;
                    }
                    else
                        break;
                }
                value = value.Substring(Math.Min(wordBuffer, value.Length));
                builder.AppendWhen(value.Length > 0, Environment.NewLine);
            }
            while (value.Length > maximumLength);

            builder.Append(value);
        }

        private IEnumerable<Specification> GetSpecificationsFromType(Type type)
        {
            var specs = type.GetSpecifications(Specification.FromProperty);
            var optionSpecs = specs
                .OfType<OptionSpecification>()
                .Concat(new[] { MakeHelpEntry(), MakeVersionEntry() });
            var valueSpecs = specs
                .OfType<ValueSpecification>()
                .OrderBy(v => v.Index);
            return Enumerable.Empty<Specification>()
                .Concat(optionSpecs)
                .Concat(valueSpecs);
        }

        private static Maybe<Tuple<UsageAttribute, IEnumerable<Example>>> GetUsageFromType(Type type)
        {
            return type.GetUsageData().Map(
                tuple =>
                {
                    var prop = tuple.Item1;
                    var attr = tuple.Item2;

#if NETSTANDARD1_5
                    var examples = (IEnumerable<Example>)prop.GetValue(null);
#else
                    var examples = (IEnumerable<Example>)prop
                        .GetValue(null, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, null);
#endif

                    return Tuple.Create(attr, examples);
                });
        }

        private IEnumerable<Specification> AdaptVerbsToSpecifications(IEnumerable<Type> types)
        {
            return (from verbTuple in Verb.SelectFromTypes(types)
                    select
                        OptionSpecification.NewSwitch(
                            string.Empty,
                            verbTuple.Item1.Name,
                            false,
                            verbTuple.Item1.HelpText,
                            string.Empty,
                            verbTuple.Item1.Hidden)).Concat(new[] { MakeHelpEntry(), MakeVersionEntry() });
        }

        private HelpText AddOptionsImpl(
            IEnumerable<Specification> specifications,
            string requiredWord,
            int maximumLength)
        {
            var maxLength = GetMaxLength(specifications);

            optionsHelp = new StringBuilder(BuilderCapacity);

            var remainingSpace = maximumLength - (maxLength + 6);

            specifications.ForEach(
                option =>
                    AddOption(requiredWord, maxLength, option, remainingSpace));

            return this;
        }

        private OptionSpecification MakeHelpEntry()
        {
            return OptionSpecification.NewSwitch(
                string.Empty,
                "help",
                false,
                sentenceBuilder.HelpCommandText(AddDashesToOption),
                string.Empty,
                false);
        }

        private OptionSpecification MakeVersionEntry()
        {
            return OptionSpecification.NewSwitch(
                string.Empty,
                "version",
                false,
                sentenceBuilder.VersionCommandText(AddDashesToOption),
                string.Empty,
                false);
        }

        private HelpText AddPreOptionsLine(string value, int maximumLength)
        {
            AddLine(preOptionsHelp, value, maximumLength);

            return this;
        }

        private HelpText AddOption(string requiredWord, int maxLength, Specification specification, int widthOfHelpText)
        {
            if (specification.Hidden)
                return this;

            optionsHelp.Append("  ");
            var name = new StringBuilder(maxLength)
                .BimapIf(
                    specification.Tag == SpecificationType.Option,
                    it => it.Append(AddOptionName(maxLength, (OptionSpecification)specification)),
                    it => it.Append(AddValueName(maxLength, (ValueSpecification)specification)));

            optionsHelp
                .Append(name.Length < maxLength ? name.ToString().PadRight(maxLength) : name.ToString())
                .Append("    ");

            var optionHelpText = specification.HelpText;

            if (addEnumValuesToHelpText && specification.EnumValues.Any())
                optionHelpText += " Valid values: " + string.Join(", ", specification.EnumValues);

            specification.DefaultValue.Do(
                defaultValue => optionHelpText = "(Default: {0}) ".FormatInvariant(FormatDefaultValue(defaultValue)) + optionHelpText);

            if (specification.Required)
                optionHelpText = "{0} ".FormatInvariant(requiredWord) + optionHelpText;

            if (!string.IsNullOrEmpty(optionHelpText))
            {
                do
                {
                    var wordBuffer = 0;
                    var words = optionHelpText.Split(' ');
                    for (var i = 0; i < words.Length; i++)
                    {
                        if (words[i].Length < (widthOfHelpText - wordBuffer))
                        {
                            optionsHelp.Append(words[i]);
                            wordBuffer += words[i].Length;
                            if ((widthOfHelpText - wordBuffer) > 1 && i != words.Length - 1)
                            {
                                optionsHelp.Append(" ");
                                wordBuffer++;
                            }
                        }
                        else if (words[i].Length >= widthOfHelpText && wordBuffer == 0)
                        {
                            optionsHelp.Append(words[i].Substring(0, widthOfHelpText));
                            wordBuffer = widthOfHelpText;
                            break;
                        }
                        else
                            break;
                    }

                    optionHelpText = optionHelpText.Substring(Math.Min(wordBuffer, optionHelpText.Length)).Trim();
                    optionsHelp.AppendWhen(optionHelpText.Length > 0, Environment.NewLine,
                        new string(' ', maxLength + 6));
                }
                while (optionHelpText.Length > widthOfHelpText);
            }

            optionsHelp
                .Append(optionHelpText)
                .Append(Environment.NewLine)
                .AppendWhen(additionalNewLineAfterOption, Environment.NewLine);

            return this;
        }

        private string AddOptionName(int maxLength, OptionSpecification specification)
        {
            return
                new StringBuilder(maxLength)
                    .MapIf(
                        specification.ShortName.Length > 0,
                        it => it
                            .AppendWhen(addDashesToOption, '-')
                            .AppendFormat("{0}", specification.ShortName)
                            .AppendFormatWhen(specification.MetaValue.Length > 0, " {0}", specification.MetaValue)
                            .AppendWhen(specification.LongName.Length > 0, ", "))
                    .MapIf(
                        specification.LongName.Length > 0,
                        it => it
                            .AppendWhen(addDashesToOption, "--")
                            .AppendFormat("{0}", specification.LongName)
                            .AppendFormatWhen(specification.MetaValue.Length > 0, "={0}", specification.MetaValue))
                    .ToString();
        }

        private string AddValueName(int maxLength, ValueSpecification specification)
        {
            return new StringBuilder(maxLength)
                .BimapIf(
                    specification.MetaName.Length > 0,
                    it => it.AppendFormat("{0} (pos. {1})", specification.MetaName, specification.Index),
                    it => it.AppendFormat("value pos. {0}", specification.Index))
                .AppendFormatWhen(
                    specification.MetaValue.Length > 0, " {0}", specification.MetaValue)
                .ToString();
        }

        private HelpText AddLine(StringBuilder builder, string value)
        {
            AddLine(builder, value, MaximumDisplayWidth);

            return this;
        }

        private int GetMaxLength(IEnumerable<Specification> specifications)
        {
            return specifications.Aggregate(0,
                (length, spec) =>
                {
                    if (spec.Hidden)
                        return length;
                    var specLength = spec.Tag == SpecificationType.Option
                            ? GetMaxOptionLength((OptionSpecification)spec)
                            : GetMaxValueLength((ValueSpecification)spec);

                    return Math.Max(length, specLength);
                });
        }


        private int GetMaxOptionLength(OptionSpecification spec)
        {
            var specLength = 0;

            var hasShort = spec.ShortName.Length > 0;
            var hasLong = spec.LongName.Length > 0;

            var metaLength = 0;
            if (spec.MetaValue.Length > 0)
                metaLength = spec.MetaValue.Length + 1;

            if (hasShort)
            {
                ++specLength;
                if (AddDashesToOption)
                    ++specLength;

                specLength += metaLength;
            }

            if (hasLong)
            {
                specLength += spec.LongName.Length;
                if (AddDashesToOption)
                    specLength += 2;

                specLength += metaLength;
            }

            if (hasShort && hasLong)
                specLength += 2; // ", "

            return specLength;
        }

        private int GetMaxValueLength(ValueSpecification spec)
        {
            var specLength = 0;

            var hasMeta = spec.MetaName.Length > 0;

            var metaLength = 0;
            if (spec.MetaValue.Length > 0)
                metaLength = spec.MetaValue.Length + 1;

            if (hasMeta)
                specLength += spec.MetaName.Length + spec.Index.ToStringInvariant().Length + 8; //METANAME (pos. N)
            else
                specLength += spec.Index.ToStringInvariant().Length + 11; // "value pos. N"

            specLength += metaLength;

            return specLength;
        }

        private static string FormatDefaultValue<T>(T value)
        {
            if (value is bool)
                return value.ToStringLocal().ToLowerInvariant();

            if (value is string)
                return value.ToStringLocal();

            var asEnumerable = value as IEnumerable;
            if (asEnumerable == null)
                return value.ToStringLocal();

            var builder = new StringBuilder();
            foreach (var item in asEnumerable)
                builder
                    .Append(item.ToStringLocal())
                    .Append(" ");

            return builder.Length > 0
                ? builder.ToString(0, builder.Length - 1)
                : string.Empty;
        }
    }
}