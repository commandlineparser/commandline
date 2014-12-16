// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CommandLine.Infrastructure;
using CommandLine.Core;

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
        private int? maximumDisplayWidth;
        private string heading;
        private string copyright;
        private bool additionalNewLineAfterOption;
        private StringBuilder optionsHelp;
        private bool addDashesToOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class.
        /// </summary>
        public HelpText()
            : this(SentenceBuilder.CreateDefault(), string.Empty, string.Empty)
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
            : this(SentenceBuilder.CreateDefault(), heading, string.Empty)
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
            : this(SentenceBuilder.CreateDefault(), heading, copyright)
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

            this.preOptionsHelp = new StringBuilder(BuilderCapacity);
            this.postOptionsHelp = new StringBuilder(BuilderCapacity);

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
            get
            {
                return this.heading;
            }

            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.heading = value;
            }
        }

        /// <summary>
        /// Gets or sets the copyright string.
        /// You can directly assign a <see cref="CommandLine.Text.CopyrightInfo"/> instance.
        /// </summary>
        public string Copyright
        {
            get
            {
                return this.copyright;
            }

            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.copyright = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        /// <value>The maximum width of the display.</value>
        public int MaximumDisplayWidth
        {
            get { return this.maximumDisplayWidth.HasValue ? this.maximumDisplayWidth.Value : DefaultMaximumLength; }
            set { this.maximumDisplayWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format of options should contain dashes.
        /// It modifies behavior of <see cref="AddOptions{T}(T)"/> method.
        /// </summary>
        public bool AddDashesToOption
        {
            get { return this.addDashesToOption; }
            set { this.addDashesToOption = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add an additional line after the description of the option.
        /// </summary>
        public bool AdditionalNewLineAfterOption
        {
            get { return this.additionalNewLineAfterOption; }
            set { this.additionalNewLineAfterOption = value; }
        }

        /// <summary>
        /// Gets the <see cref="SentenceBuilder"/> instance specified in constructor.
        /// </summary>
        public SentenceBuilder SentenceBuilder
        {
            get { return this.sentenceBuilder; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name='onError'>A delegate used to customize the text block of reporting parsing errors text block.</param>
        /// <param name="verbsIndex">If true the output style is consistent with verb commands (no dashes), otherwise it outputs options.</param>
        /// <remarks>The parameter <paramref name="verbsIndex"/> is not ontly a metter of formatting, it controls whether to handle verbs or options.</remarks>
        public static HelpText AutoBuild<T>(ParserResult<T> parserResult, Func<HelpText, HelpText> onError, bool verbsIndex = false)
        {
            var auto = new HelpText
                {
                    Heading = HeadingInfo.Default,
                    Copyright = CopyrightInfo.Default,
                    AdditionalNewLineAfterOption = true,
                    AddDashesToOption = !verbsIndex
                };

            if (onError != null)
            {
                if (FilterMeaningfulErrors(parserResult.Errors).Any())
                {
                    auto = onError(auto);
                }
            }

            var license = ReflectionHelper.GetAttribute<AssemblyLicenseAttribute>();
            if (license.IsJust())
            {
                license.FromJust().AddToHelpText(auto, true);
            }

            var usage = ReflectionHelper.GetAttribute<AssemblyUsageAttribute>();
            if (usage.IsJust())
            {
                usage.FromJust().AddToHelpText(auto, true);
            }

            if (verbsIndex && parserResult.VerbTypes.IsJust())
            {
                auto.AddVerbs(parserResult.VerbTypes.FromJust().ToArray());
            }
            else
            {
                auto.AddOptions(parserResult.Value);
            }

            return auto;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class,
        /// automatically handling verbs or options scenario.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <remarks>This feature is meant to be invoked automatically by the parser, setting the HelpWriter property
        /// of <see cref="CommandLine.ParserSettings"/>.</remarks>
        public static HelpText AutoBuild<T>(ParserResult<T> parserResult)
        {
            switch (parserResult.Tag)
            {
                case ParserResultType.Options:
                    return HelpText.AutoBuild(parserResult, current => HelpText.DefaultParsingErrorsHandler(parserResult, current));

                case ParserResultType.Verbs:
                    var helpVerbErr = parserResult.Errors.OfType<HelpVerbRequestedError>();
                    if (helpVerbErr.Any())
                    {
                        var err = helpVerbErr.Single();
                        if (err.Matched)
                        {
                            var pr = ParserResult.Create(ParserResultType.Options, Activator.CreateInstance(err.Type), Enumerable.Empty<Error>());
                            return HelpText.AutoBuild(pr, current => HelpText.DefaultParsingErrorsHandler(pr, current));
                        }
                    }
                    return HelpText.AutoBuild(parserResult, current => HelpText.DefaultParsingErrorsHandler(parserResult, current), true);

                default:
                    throw new InvalidOperationException();
            }
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

            if (FilterMeaningfulErrors(parserResult.Errors).Empty())
            {
                return current;
            }

            var errors = HelpText.RenderParsingErrorsText(parserResult, current.SentenceBuilder.FormatError, 2); // indent with two spaces
            if (string.IsNullOrEmpty(errors))
            {
                return current;
            }

            current.AddPreOptionsLine(string.Concat(Environment.NewLine, current.SentenceBuilder.ErrorsHeadingText()));
            var lines = errors.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                current.AddPreOptionsLine(line);
            }

            return current;
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
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public HelpText AddPreOptionsLine(string value)
        {
            return this.AddPreOptionsLine(value, MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text line at the bottom, after options usage string.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public HelpText AddPostOptionsLine(string value)
        {
            return this.AddLine(this.postOptionsHelp, value);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        public HelpText AddOptions<T>(T options)
        {
            if (object.Equals(options, default(T))) throw new ArgumentNullException("options");

            return this.AddOptionsImpl(this.GetOptionListFromType(options), SentenceBuilder.RequiredWord(), MaximumDisplayWidth);
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

            return this.AddOptionsImpl(this.AdaptVerbListToOptionList(types), SentenceBuilder.RequiredWord(), MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="maximumLength">The maximum length of the help screen.</param>
        /// <param name="options">The instance that collected command line arguments parsed with the <see cref="Parser"/> class.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>    
        public HelpText AddOptions<T>(int maximumLength, T options)
        {
            if (object.Equals(options, default(T))) throw new ArgumentNullException("options");

            return this.AddOptionsImpl(this.GetOptionListFromType(options), SentenceBuilder.RequiredWord(), maximumLength);
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

            return this.AddOptionsImpl(this.AdaptVerbListToOptionList(types), SentenceBuilder.RequiredWord(), maximumLength);
        }

        /// <summary>
        /// Builds a string that contains a parsing error message.
        /// </summary>
        /// <param name='parserResult'>The <see cref="CommandLine.ParserResult{T}"/> containing the instance that collected command line arguments parsed with <see cref="CommandLine.Parser"/> class.</param>
        /// <param name="formatError">The error formatting delegate.</param>
        /// <param name="indent">Number of spaces used to indent text.</param>
        /// <returns>The <see cref="System.String"/> that contains the parsing error message.</returns>
        public static string RenderParsingErrorsText<T>(ParserResult<T> parserResult, Func<Error, string> formatError, int indent)
        {
            if (parserResult == null) throw new ArgumentNullException("parserResult");

            var meaningfulErrors = FilterMeaningfulErrors(parserResult.Errors);
            if (meaningfulErrors.Empty())
            {
                return string.Empty;
            }

            var text = new StringBuilder();
            foreach (var error in meaningfulErrors)
            {
                var line = new StringBuilder();
                line.Append(indent.Spaces());

                line.Append(formatError(error));

                text.AppendLine(line.ToString());
            }

            return text.ToString();
        }

        /// <summary>
        /// Returns the help screen as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>The <see cref="System.String"/> that contains the help screen.</returns>
        public override string ToString()
        {
            const int ExtraLength = 10;
            var builder = new StringBuilder(GetLength(this.heading) + GetLength(this.copyright) +
                GetLength(this.preOptionsHelp) + GetLength(this.optionsHelp) + ExtraLength);

            builder.Append(this.heading);
            if (!string.IsNullOrEmpty(this.copyright))
            {
                builder.Append(Environment.NewLine);
                builder.Append(this.copyright);
            }

            if (this.preOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(this.preOptionsHelp);
            }

            if (this.optionsHelp != null && this.optionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append(this.optionsHelp);
            }

            if (this.postOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(this.postOptionsHelp);
            }

            return builder.ToString();
        }

        private static IEnumerable<Error> FilterMeaningfulErrors(IEnumerable<Error> errors)
        {
            return errors.Where(
                e => e.Tag != ErrorType.HelpRequestedError && e.Tag != ErrorType.HelpVerbRequestedError);
        }

        private static int GetLength(string value)
        {
            return value == null ? 0 : value.Length;
        }

        private static int GetLength(StringBuilder value)
        {
            return value == null ? 0 : value.Length;
        }

        private static void AddLine(StringBuilder builder, string value, int maximumLength)
        {
            if (builder.Length > 0)
            {
                builder.Append(Environment.NewLine);
            }

            do
            {
                var wordBuffer = 0;
                var words = value.Split(new[] { ' ' });
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
                    {
                        break;
                    }
                }

                value = value.Substring(Math.Min(wordBuffer, value.Length));
                if (value.Length > 0)
                {
                    builder.Append(Environment.NewLine);
                }
            }
            while (value.Length > maximumLength);

            builder.Append(value);
        }

        private IEnumerable<OptionSpecification> GetOptionListFromType<T>(T options)
        {
             return options.GetType().GetSpecifications(pi => Specification.FromProperty(pi))
                .OfType<OptionSpecification>()
                .Concat(new[] { this.CreateHelpEntry() });
        }

        private IEnumerable<OptionSpecification> AdaptVerbListToOptionList(IEnumerable<Type> types)
        {
            return (from verbTuple in Verb.SelectFromTypes(types)
                   select new OptionSpecification(
                       string.Empty,
                       verbTuple.Item1.Name,
                       false,
                       string.Empty,
                       -1,
                       -1,
                       Maybe.Nothing<object>(),
                       typeof(bool),
                       verbTuple.Item1.HelpText,
                       string.Empty))
                    .Concat(new[] { this.CreateHelpEntry() });
        }

        private HelpText AddOptionsImpl(IEnumerable<OptionSpecification> optionList, string requiredWord, int maximumLength)
        {
            var maxLength = GetMaxLength(optionList);

            this.optionsHelp = new StringBuilder(BuilderCapacity);

            var remainingSpace = maximumLength - (maxLength + 6);

            foreach (var option in optionList)
            {
                AddOption(requiredWord, maxLength, option, remainingSpace);
            }

            return this;
        }

        private OptionSpecification CreateHelpEntry()
        {
            return new OptionSpecification(string.Empty, "help", false, string.Empty, -1, -1, Maybe.Nothing<object>(), typeof(bool),
                this.sentenceBuilder.HelpCommandText(this.AddDashesToOption), string.Empty);
        }

        private HelpText AddPreOptionsLine(string value, int maximumLength)
        {
            AddLine(this.preOptionsHelp, value, maximumLength);

            return this;
        }

        private HelpText AddOption(string requiredWord, int maxLength, OptionSpecification option, int widthOfHelpText)
        {
            this.optionsHelp.Append("  ");
            var optionName = new StringBuilder(maxLength);
            if (option.ShortName.Length > 0)
            {
                if (this.addDashesToOption)
                {
                    optionName.Append('-');
                }

                optionName.AppendFormat("{0}", option.ShortName);
                
                if (option.MetaValue.Length > 0)
                {
                    optionName.AppendFormat(" {0}", option.MetaValue);
                }

                if (option.LongName.Length > 0)
                {
                    optionName.Append(", ");
                }
            }

            if (option.LongName.Length > 0)
            {
                if (this.addDashesToOption)
                {
                    optionName.Append("--");
                }

                optionName.AppendFormat("{0}", option.LongName);

                if (option.MetaValue.Length > 0)
                {
                    optionName.AppendFormat("={0}", option.MetaValue);
                }
            }

            this.optionsHelp.Append(optionName.Length < maxLength ?
                optionName.ToString().PadRight(maxLength) :
                optionName.ToString());

            this.optionsHelp.Append("    ");
            var optionHelpText = option.HelpText;
            if (option.DefaultValue.IsJust())
            {
                optionHelpText = "(Default: {0}) ".FormatLocal(option.DefaultValue.FromJust()) + optionHelpText;
            }

            if (option.Required)
            {
                optionHelpText = "{0} ".FormatInvariant(requiredWord) + optionHelpText;
            }

            if (!string.IsNullOrEmpty(optionHelpText))
            {
                do
                {
                    var wordBuffer = 0;
                    var words = optionHelpText.Split(new[] { ' ' });
                    for (var i = 0; i < words.Length; i++)
                    {
                        if (words[i].Length < (widthOfHelpText - wordBuffer))
                        {
                            this.optionsHelp.Append(words[i]);
                            wordBuffer += words[i].Length;
                            if ((widthOfHelpText - wordBuffer) > 1 && i != words.Length - 1)
                            {
                                this.optionsHelp.Append(" ");
                                wordBuffer++;
                            }
                        }
                        else if (words[i].Length >= widthOfHelpText && wordBuffer == 0)
                        {
                            this.optionsHelp.Append(words[i].Substring(0, widthOfHelpText));
                            wordBuffer = widthOfHelpText;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    optionHelpText = optionHelpText.Substring(
                        Math.Min(wordBuffer, optionHelpText.Length)).Trim();
                    if (optionHelpText.Length > 0)
                    {
                        this.optionsHelp.Append(Environment.NewLine);
                        this.optionsHelp.Append(new string(' ', maxLength + 6));
                    }
                }
                while (optionHelpText.Length > widthOfHelpText);
            }

            this.optionsHelp.Append(optionHelpText);
            this.optionsHelp.Append(Environment.NewLine);
            if (this.additionalNewLineAfterOption)
            {
                this.optionsHelp.Append(Environment.NewLine);
            }

            return this;
        }

        private HelpText AddLine(StringBuilder builder, string value)
        {
            AddLine(builder, value, MaximumDisplayWidth);

            return this;
        }

        private int GetMaxLength(IEnumerable<OptionSpecification> optionList)
        {
            var length = 0;
            foreach (var option in optionList)
            {
                var optionLength = 0;
                var hasShort = option.ShortName.Length > 0;
                var hasLong = option.LongName.Length > 0;
                var metaLength = 0;
                if (option.MetaValue.Length > 0)
                {
                    metaLength = option.MetaValue.Length + 1;
                }

                if (hasShort)
                {
                    ++optionLength;
                    if (this.AddDashesToOption)
                    {
                        ++optionLength;
                    }

                    optionLength += metaLength;
                }

                if (hasLong)
                {
                    optionLength += option.LongName.Length;
                    if (this.AddDashesToOption)
                    {
                        optionLength += 2;
                    }

                    optionLength += metaLength;
                }

                if (hasShort && hasLong)
                {
                    optionLength += 2; // ", "
                }

                length = Math.Max(length, optionLength);
            }

            return length;
        }
    }
}