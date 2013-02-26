#region License
// <copyright file="HelpText.cs" company="Giacomo Stelluti Scala">
//   Copyright 2015-2013 Giacomo Stelluti Scala
// </copyright>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using CommandLine.Extensions;
using CommandLine.Infrastructure;

#endregion

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
        private const string DefaultRequiredWord = "Required.";
        private readonly StringBuilder _preOptionsHelp;
        private readonly StringBuilder _postOptionsHelp;
        private readonly BaseSentenceBuilder _sentenceBuilder;
        private int? _maximumDisplayWidth;
        private string _heading;
        private string _copyright;
        private bool _additionalNewLineAfterOption;
        private StringBuilder _optionsHelp;
        private bool _addDashesToOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class.
        /// </summary>
        public HelpText()
        {
            _preOptionsHelp = new StringBuilder(BuilderCapacity);
            _postOptionsHelp = new StringBuilder(BuilderCapacity);

            _sentenceBuilder = BaseSentenceBuilder.CreateBuiltIn();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class 
        /// specifying the sentence builder.
        /// </summary>
        /// <param name="sentenceBuilder">
        /// A <see cref="BaseSentenceBuilder"/> instance.
        /// </param>
        public HelpText(BaseSentenceBuilder sentenceBuilder)
            : this()
        {
            Assumes.NotNull(sentenceBuilder, "sentenceBuilder");

            _sentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading string.
        /// </summary>
        /// <param name="heading">An heading string or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown when parameter <paramref name="heading"/> is null or empty string.</exception>
        public HelpText(string heading)
            : this()
        {
            Assumes.NotNullOrEmpty(heading, "heading");

            _heading = heading;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying the sentence builder and heading string.
        /// </summary>
        /// <param name="sentenceBuilder">A <see cref="BaseSentenceBuilder"/> instance.</param>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        public HelpText(BaseSentenceBuilder sentenceBuilder, string heading)
            : this(heading)
        {
            Assumes.NotNull(sentenceBuilder, "sentenceBuilder");

            _sentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        public HelpText(string heading, string copyright)
            : this()
        {
            Assumes.NotNullOrEmpty(heading, "heading");
            Assumes.NotNullOrEmpty(copyright, "copyright");

            _heading = heading;
            _copyright = copyright;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="sentenceBuilder">A <see cref="BaseSentenceBuilder"/> instance.</param>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        public HelpText(BaseSentenceBuilder sentenceBuilder, string heading, string copyright)
            : this(heading, copyright)
        {
            Assumes.NotNull(sentenceBuilder, "sentenceBuilder");

            _sentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "When DoAddOptions is called with fireEvent=false virtual member is not called")]
        public HelpText(string heading, string copyright, object options)
            : this()
        {
            Assumes.NotNullOrEmpty(heading, "heading");
            Assumes.NotNullOrEmpty(copyright, "copyright");
            Assumes.NotNull(options, "options");

            _heading = heading;
            _copyright = copyright;
            DoAddOptions(options, DefaultRequiredWord, MaximumDisplayWidth, fireEvent: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Text.HelpText"/> class
        /// specifying heading and copyright strings.
        /// </summary>
        /// <param name="sentenceBuilder">A <see cref="BaseSentenceBuilder"/> instance.</param>
        /// <param name="heading">A string with heading or an instance of <see cref="CommandLine.Text.HeadingInfo"/>.</param>
        /// <param name="copyright">A string with copyright or an instance of <see cref="CommandLine.Text.CopyrightInfo"/>.</param>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <exception cref="System.ArgumentException">Thrown when one or more parameters <paramref name="heading"/> are null or empty strings.</exception>
        public HelpText(BaseSentenceBuilder sentenceBuilder, string heading, string copyright, object options)
            : this(heading, copyright, options)
        {
            Assumes.NotNull(sentenceBuilder, "sentenceBuilder");

            _sentenceBuilder = sentenceBuilder;
        }

        /// <summary>
        /// Occurs when an option help text is formatted.
        /// </summary>
        public event EventHandler<FormatOptionHelpTextEventArgs> FormatOptionHelpText;

        /// <summary>
        /// Gets or sets the heading string.
        /// You can directly assign a <see cref="CommandLine.Text.HeadingInfo"/> instance.
        /// </summary>
        public string Heading
        {
            get
            {
                return _heading;
            }

            set
            {
                Assumes.NotNullOrEmpty(value, "value");
                _heading = value;
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
                return _heading;
            }

            set
            {
                Assumes.NotNullOrEmpty(value, "value");
                _copyright = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the display.  This determines word wrap when displaying the text.
        /// </summary>
        /// <value>The maximum width of the display.</value>
        public int MaximumDisplayWidth
        {
            get { return _maximumDisplayWidth.HasValue ? _maximumDisplayWidth.Value : DefaultMaximumLength; }

            set { _maximumDisplayWidth = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the format of options should contain dashes.
        /// It modifies behavior of <see cref="AddOptions(System.Object)"/> method.
        /// </summary>
        public bool AddDashesToOption
        {
            get { return this._addDashesToOption; }
            set { this._addDashesToOption = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to add an additional line after the description of the option.
        /// </summary>
        public bool AdditionalNewLineAfterOption
        {
            get { return _additionalNewLineAfterOption; }
            set { _additionalNewLineAfterOption = value; }
        }

        /// <summary>
        /// Gets the <see cref="BaseSentenceBuilder"/> instance specified in constructor.
        /// </summary>
        public BaseSentenceBuilder SentenceBuilder
        {
            get { return _sentenceBuilder; }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <param name='options'>The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        public static HelpText AutoBuild(object options)
        {
            return AutoBuild(options, (Action<HelpText>)null);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class using common defaults.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <param name='options'>The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <param name='onError'>A delegate used to customize the text block for reporting parsing errors.</param>
        /// <param name="verbsIndex">If true the output style is consistent with verb commands (no dashes), otherwise it outputs options.</param>
        public static HelpText AutoBuild(object options, Action<HelpText> onError, bool verbsIndex = false)
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
                var list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
                if (list != null)
                {
                    onError(auto);
                }
            }

            var license = ReflectionHelper.GetAttribute<AssemblyLicenseAttribute>();
            if (license != null)
            {
                license.AddToHelpText(auto, true);
            }

            var usage = ReflectionHelper.GetAttribute<AssemblyUsageAttribute>();
            if (usage != null)
            {
                usage.AddToHelpText(auto, true);
            }

            auto.AddOptions(options);

            return auto;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CommandLine.Text.HelpText"/> class using common defaults,
        /// for verb commands scenario.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="CommandLine.Text.HelpText"/> class.
        /// </returns>
        /// <param name='options'>The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <param name="verb">The verb command invoked.</param>
        public static HelpText AutoBuild(object options, string verb)
        {
            bool found;
            var instance = Parser.InternalGetVerbOptionsInstanceByName(verb, options, out found);
            var verbsIndex = verb == null || !found;
            var target = verbsIndex ? options : instance;
            return HelpText.AutoBuild(target, current => HelpText.DefaultParsingErrorsHandler(target, current), verbsIndex);
        }

        /// <summary>
        /// Supplies a default parsing error handler implementation.
        /// </summary>
        /// <param name="options">The instance that collects parsed arguments parsed and associates <see cref="CommandLine.ParserStateAttribute"/>
        /// to a property of type <see cref="IParserState"/>.</param>
        /// <param name="current">The <see cref="CommandLine.Text.HelpText"/> instance.</param>
        public static void DefaultParsingErrorsHandler(object options, HelpText current)
        {
            var list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
            if (list.Count == 0)
            {
                return;
            }

            var parserState = (IParserState)list[0].Left.GetValue(options, null);
            if (parserState == null || parserState.Errors.Count == 0)
            {
                return;
            }

            var errors = current.RenderParsingErrorsText(options, 2); // indent with two spaces
            if (!string.IsNullOrEmpty(errors))
            {
                current.AddPreOptionsLine(string.Concat(Environment.NewLine, current.SentenceBuilder.ErrorsHeadingText));
                var lines = errors.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var line in lines)
                {
                    current.AddPreOptionsLine(line);
                }
            }
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
        public void AddPreOptionsLine(string value)
        {
            AddPreOptionsLine(value, MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text line at the bottom, after options usage string.
        /// </summary>
        /// <param name="value">A <see cref="System.String"/> instance.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="value"/> is null or empty string.</exception>
        public void AddPostOptionsLine(string value)
        {
            AddLine(_postOptionsHelp, value);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with <see cref="Parser"/> class.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        public void AddOptions(object options)
        {
            AddOptions(options, DefaultRequiredWord);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with the <see cref="Parser"/> class.</param>
        /// <param name="requiredWord">The word to use when the option is required.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="requiredWord"/> is null or empty string.</exception>
        public void AddOptions(object options, string requiredWord)
        {
            Assumes.NotNull(options, "options");
            Assumes.NotNullOrEmpty(requiredWord, "requiredWord");

            AddOptions(options, requiredWord, MaximumDisplayWidth);
        }

        /// <summary>
        /// Adds a text block with options usage string.
        /// </summary>
        /// <param name="options">The instance that collected command line arguments parsed with the <see cref="Parser"/> class.</param>
        /// <param name="requiredWord">The word to use when the option is required.</param>
        /// <param name="maximumLength">The maximum length of the help documentation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="options"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when parameter <paramref name="requiredWord"/> is null or empty string.</exception>
        public void AddOptions(object options, string requiredWord, int maximumLength)
        {
            Assumes.NotNull(options, "options");
            Assumes.NotNullOrEmpty(requiredWord, "requiredWord");

            DoAddOptions(options, requiredWord, maximumLength);
        }

        /// <summary>
        /// Builds a string that contains a parsing error message.
        /// </summary>
        /// <param name="options">An options target instance that collects parsed arguments parsed with the <see cref="CommandLine.ParserStateAttribute"/>
        /// associated to a property of type <see cref="IParserState"/>.</param>
        /// <param name="indent">Number of spaces used to indent text.</param>
        /// <returns>The <see cref="System.String"/> that contains the parsing error message.</returns>
        public string RenderParsingErrorsText(object options, int indent)
        {
            var list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
            if (list.Count == 0)
            {
                return string.Empty; // Or exception?
            }

            var parserState = (IParserState)list[0].Left.GetValue(options, null);
            if (parserState == null || parserState.Errors.Count == 0)
            {
                return string.Empty;
            }

            var text = new StringBuilder();
            foreach (var e in parserState.Errors)
            {
                var line = new StringBuilder();
                line.Append(indent.Spaces());
                if (e.BadOption.ShortName != null)
                {
                    line.Append('-');
                    line.Append(e.BadOption.ShortName);
                    if (!string.IsNullOrEmpty(e.BadOption.LongName))
                    {
                        line.Append('/');
                    }
                }

                if (!string.IsNullOrEmpty(e.BadOption.LongName))
                {
                    line.Append("--");
                    line.Append(e.BadOption.LongName);
                }

                line.Append(" ");
                line.Append(e.ViolatesRequired ?
                    _sentenceBuilder.RequiredOptionMissingText :
                    _sentenceBuilder.OptionWord);
                if (e.ViolatesFormat)
                {
                    line.Append(" ");
                    line.Append(_sentenceBuilder.ViolatesFormatText);
                }

                if (e.ViolatesMutualExclusiveness)
                {
                    if (e.ViolatesFormat || e.ViolatesRequired)
                    {
                        line.Append(" ");
                        line.Append(_sentenceBuilder.AndWord);
                    }

                    line.Append(" ");
                    line.Append(_sentenceBuilder.ViolatesMutualExclusivenessText);
                }

                line.Append('.');
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
            var builder = new StringBuilder(GetLength(_heading) + GetLength(_copyright) +
                GetLength(_preOptionsHelp) + GetLength(this._optionsHelp) + ExtraLength);

            builder.Append(_heading);
            if (!string.IsNullOrEmpty(_copyright))
            {
                builder.Append(Environment.NewLine);
                builder.Append(_copyright);
            }

            if (_preOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(_preOptionsHelp);
            }

            if (this._optionsHelp != null && this._optionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(Environment.NewLine);
                builder.Append(this._optionsHelp);
            }

            if (_postOptionsHelp.Length > 0)
            {
                builder.Append(Environment.NewLine);
                builder.Append(_postOptionsHelp);
            }

            return builder.ToString();
        }

        /// <summary>
        /// The OnFormatOptionHelpText method also allows derived classes to handle the event without attaching a delegate.
        /// This is the preferred technique for handling the event in a derived class.
        /// </summary>
        /// <param name="e">Data for the <see cref="FormatOptionHelpText"/> event.</param>
        protected virtual void OnFormatOptionHelpText(FormatOptionHelpTextEventArgs e)
        {
            EventHandler<FormatOptionHelpTextEventArgs> handler = FormatOptionHelpText;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static int GetLength(string value)
        {
            if (value == null)
            {
                return 0;
            }

            return value.Length;
        }

        private static int GetLength(StringBuilder value)
        {
            if (value == null)
            {
                return 0;
            }

            return value.Length;
        }

        private static void AddLine(StringBuilder builder, string value, int maximumLength)
        {
            Assumes.NotNull(value, "value");

            if (builder.Length > 0)
            {
                builder.Append(Environment.NewLine);
            }

            do
            {
                int wordBuffer = 0;
                string[] words = value.Split(new[] { ' ' });
                for (int i = 0; i < words.Length; i++)
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

        private void DoAddOptions(object options, string requiredWord, int maximumLength, bool fireEvent = true)
        {
            var optionList = ReflectionHelper.RetrievePropertyAttributeList<BaseOptionAttribute>(options);
            var optionHelp = ReflectionHelper.RetrieveMethodAttributeOnly<HelpOptionAttribute>(options);

            if (optionHelp != null)
            {
                optionList.Add(optionHelp);
            }

            if (optionList.Count == 0)
            {
                return;
            }

            int maxLength = GetMaxLength(optionList);
            this._optionsHelp = new StringBuilder(BuilderCapacity);
            int remainingSpace = maximumLength - (maxLength + 6);
            foreach (BaseOptionAttribute option in optionList)
            {
                AddOption(requiredWord, maxLength, option, remainingSpace, fireEvent);
            }
        }

        private void AddPreOptionsLine(string value, int maximumLength)
        {
            AddLine(_preOptionsHelp, value, maximumLength);
        }

        private void AddOption(string requiredWord, int maxLength, BaseOptionAttribute option, int widthOfHelpText, bool fireEvent = true)
        {
            this._optionsHelp.Append("  ");
            var optionName = new StringBuilder(maxLength);
            if (option.HasShortName)
            {
                if (this._addDashesToOption)
                {
                    optionName.Append('-');
                }

                optionName.AppendFormat("{0}", option.ShortName);
                
                if (option.HasMetaValue)
                {
                    optionName.AppendFormat(" {0}", option.MetaValue);
                }

                if (option.HasLongName)
                {
                    optionName.Append(", ");
                }
            }

            if (option.HasLongName)
            {
                if (this._addDashesToOption)
                {
                    optionName.Append("--");
                }

                optionName.AppendFormat("{0}", option.LongName);

                if (option.HasMetaValue)
                {
                    optionName.AppendFormat("={0}", option.MetaValue);
                }
            }

            this._optionsHelp.Append(optionName.Length < maxLength ?
                optionName.ToString().PadRight(maxLength) :
                optionName.ToString());

            this._optionsHelp.Append("    ");
            if (option.HasDefaultValue)
            {
                option.HelpText = "(Default: {0}) ".FormatLocal(option.DefaultValue) + option.HelpText;
            }

            if (option.Required)
            {
                option.HelpText = "{0} ".FormatInvariant(requiredWord) + option.HelpText;
            }

            if (fireEvent)
            {
                var e = new FormatOptionHelpTextEventArgs(option);
                OnFormatOptionHelpText(e);
                option.HelpText = e.Option.HelpText;
            }

            if (!string.IsNullOrEmpty(option.HelpText))
            {
                do
                {
                    int wordBuffer = 0;
                    var words = option.HelpText.Split(new[] { ' ' });
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].Length < (widthOfHelpText - wordBuffer))
                        {
                            this._optionsHelp.Append(words[i]);
                            wordBuffer += words[i].Length;
                            if ((widthOfHelpText - wordBuffer) > 1 && i != words.Length - 1)
                            {
                                this._optionsHelp.Append(" ");
                                wordBuffer++;
                            }
                        }
                        else if (words[i].Length >= widthOfHelpText && wordBuffer == 0)
                        {
                            this._optionsHelp.Append(words[i].Substring(0, widthOfHelpText));
                            wordBuffer = widthOfHelpText;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    option.HelpText = option.HelpText.Substring(
                        Math.Min(wordBuffer, option.HelpText.Length)).Trim();
                    if (option.HelpText.Length > 0)
                    {
                        this._optionsHelp.Append(Environment.NewLine);
                        this._optionsHelp.Append(new string(' ', maxLength + 6));
                    }
                }
                while (option.HelpText.Length > widthOfHelpText);
            }

            this._optionsHelp.Append(option.HelpText);
            this._optionsHelp.Append(Environment.NewLine);
            if (_additionalNewLineAfterOption)
            {
                this._optionsHelp.Append(Environment.NewLine);
            }
        }

        private void AddLine(StringBuilder builder, string value)
        {
            Assumes.NotNull(value, "value");

            AddLine(builder, value, MaximumDisplayWidth);
        }

        private int GetMaxLength(IEnumerable<BaseOptionAttribute> optionList)
        {
            int length = 0;
            foreach (BaseOptionAttribute option in optionList)
            {
                int optionLength = 0;
                bool hasShort = option.HasShortName;
                bool hasLong = option.HasLongName;
                int metaLength = 0;
                if (option.HasMetaValue)
                {
                    metaLength = option.MetaValue.Length + 1;
                }

                if (hasShort)
                {
                    ++optionLength;
                    if (AddDashesToOption)
                    {
                        ++optionLength;
                    }

                    optionLength += metaLength;
                }

                if (hasLong)
                {
                    optionLength += option.LongName.Length;
                    if (AddDashesToOption)
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