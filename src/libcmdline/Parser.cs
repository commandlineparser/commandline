#region License
//
// Command Line Library: Parser.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2013 Giacomo Stelluti Scala
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
//
#endregion
#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using CommandLine.Core;
using CommandLine.Extensions;
using CommandLine.Text;
using CommandLine.Helpers;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// Default implementation for <see cref="IParser"/>.
    /// </summary>
    public class Parser : IParser, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        public Parser()
        {
            Settings = new ParserSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.Parser"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should by used by the parser.</param>
        public Parser(Action<ParserConfigurator> configuration)
            : this()
        {
            var configurator = new ParserConfigurator(this);
            configuration.Invoke(configurator);
        }

        // ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "singleton", Justification = "The constructor that accepts a boolean is designed to support default singleton, the parameter is ignored.")]
        private Parser(bool singleton)
        // ReSharper restore UnusedParameter.Local
        {
            Settings = new ParserSettings(false, false, Console.Error)
                {
                    ParsingCulture = CultureInfo.InvariantCulture
                };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class,
        /// configurable with a <see cref="ParserSettings"/> object.
        /// </summary>
        /// <param name="settings">The <see cref="ParserSettings"/> object is used to configure
        /// aspects and behaviors of the parser.</param>
        public Parser(ParserSettings settings)
        {
            Assumes.NotNull(settings, "settings", SR.ArgumentNullException_CommandLineParserSettingsInstanceCannotBeNull);
            Settings = settings;
        }

        /// <summary>
        /// Singleton instance created with basic defaults.
        /// </summary>
        public static IParser Default
        {
            get { return DefaultParser; }
        }

        /// <summary>
        /// Gets the instance that implements <see cref="CommandLine.IParserSettings"/> in use.
        /// </summary>
        public IParserSettings Settings
        {
            get;
            private set;
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <returns>True if parsing process succeed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            return DoParseArguments(args, options);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes.
        /// This overload allows you to specify a <see cref="System.IO.TextWriter"/> derived instance for write text messages.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// usually <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        /// <returns>True if parsing process succeed</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options, TextWriter helpWriter)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            Settings.HelpWriter = helpWriter;
            return DoParseArguments(args, options);
        }

        private bool DoParseArguments(string[] args, object options)
        {
            var pair = ReflectionUtil.RetrieveMethod<HelpOptionAttribute>(options);
            var helpWriter = Settings.HelpWriter;

            _context = new ParserContext(args, options);

            if (pair != null && helpWriter != null)
            {
                // If help can be handled is displayed if is requested or if parsing fails
                if (ParseHelp(args, pair.Right) || !DoParseArgumentsDispatcher(_context))
                {
                    string helpText;
                    HelpOptionAttribute.InvokeMethod(options, pair, out helpText);
                    helpWriter.Write(helpText);
                    return false;
                }
                return true;
            }

            return DoParseArgumentsDispatcher(_context);
        }

        private bool DoParseArgumentsDispatcher(ParserContext context)
        {
            return context.Target.HasVerbs() ?
                DoParseArgumentsVerbs(context) :
                DoParseArgumentsCore(context);
        }

        private bool DoParseArgumentsCore(ParserContext context)
        {
            var hadError = false;
            var optionMap = OptionMap.Create(context.Target, Settings);
            optionMap.SetDefaults();
            var valueMapper = new ValueMapper(context.Target, Settings.ParsingCulture);

            var arguments = new StringArrayEnumerator(context.Arguments);
            while (arguments.MoveNext())
            {
                var argument = arguments.Current;
                if (string.IsNullOrEmpty(argument))
                {
                    continue;
                }
                var parser = ArgumentParser.Create(argument, Settings.IgnoreUnknownArguments);
                if (parser != null)
                {
                    var result = parser.Parse(arguments, optionMap, context.Target);
                    if ((result & PresentParserState.Failure) == PresentParserState.Failure)
                    {
                        SetParserStateIfNeeded(context.Target, parser.PostParsingState);
                        hadError = true;
                        continue;
                    }

                    if ((result & PresentParserState.MoveOnNextElement) == PresentParserState.MoveOnNextElement)
                    {
                        arguments.MoveNext();
                    }
                }
                else if (valueMapper.CanReceiveValues)
                {
                    if (!valueMapper.MapValueItem(argument))
                    {
                        hadError = true;
                    }
                }
            }

            hadError |= !optionMap.EnforceRules();

            return !hadError;
        }

        private bool ParseHelp(IEnumerable<string> args, HelpOptionAttribute helpOption)
        {
            var caseSensitive = Settings.CaseSensitive;
            foreach (var arg in args)
            {
                if (helpOption.ShortName != null)
                {
                    if (ArgumentParser.CompareShort(arg, helpOption.ShortName, caseSensitive))
                    {
                        return true;
                    }
                }
                if (string.IsNullOrEmpty(helpOption.LongName))
                {
                    continue;
                }
                if (ArgumentParser.CompareLong(arg, helpOption.LongName, caseSensitive))
                {
                    return true;
                }
            }
            return false;
        }

        #region Verb Commands
        /// <summary>
        /// This is an helper method designed to make the management of help for verb commands simple.
        /// Use the method within the main class instance for the management of verb commands.
        /// </summary>
        /// <param name="verb">Verb command string or null.</param>
        /// <param name="target">The main class instance for the management of verb commands.</param>
        /// <param name="found">true if <paramref name="verb"/> was found in <paramref name="target"/>.</param>
        /// <returns>The options instance for the verb command if <paramref name="found"/> is true, otherwise <paramref name="target"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "By design.")]
        public static object GetVerbOptionsInstanceByName(string verb, object target, out bool found)
        {
            found = false;
            if (string.IsNullOrEmpty(verb))
            {
                return target;
            }
            var pair = ReflectionUtil.RetrieveOptionProperty<VerbOptionAttribute>(target, verb);
            found = pair != null;
            return found ? pair.Left.GetValue(target, null) : target;
        }

        /// <summary>
        /// Determines if a particular verb option was invoked. This is a convenience helper method,
        /// do not refer to it to know if parsing occurred. If the verb command was invoked and the
        /// parser failed, it will return true.
        /// Use this method only in a verbs scenario, when parsing succeeded.
        /// </summary>
        /// <param name='verb'>Verb option to compare.</param>
        /// <returns>True if the specified verb option is the one invoked by the user.</returns>
        public bool WasVerbOptionInvoked(string verb)
        {
            if (string.IsNullOrEmpty(verb) || (verb.Length > 0 && verb[0] == '-'))
            {
                return false;
            }
            if (!_context.HasAtLeastOneArgument())
            {
                return false;
            }
            return string.Compare(_context.FirstArgument, verb, Settings.GetStringComparison()) == 0;
        }

        private bool DoParseArgumentsVerbs(ParserContext context)
        {
            var verbs = ReflectionUtil.RetrievePropertyList<VerbOptionAttribute>(context.Target);
            var helpInfo = ReflectionUtil.RetrieveMethod<HelpVerbOptionAttribute>(context.Target);
            if (context.HasNoArguments())
            {
                if (helpInfo != null || Settings.HelpWriter != null)
                {
                    DisplayHelpVerbText(context.Target, helpInfo, null);
                }
                return false;
            }
            var optionMap = OptionMap.Create(context.Target, verbs, Settings);
            // Read the verb from command line arguments
            if (TryParseHelpVerb(context.Arguments, context.Target, helpInfo, optionMap))
            {
                // Since user requested help, parsing is considered a fail
                return false;
            }
            var verbOption = optionMap[context.FirstArgument];
            // User invoked a bad verb name
            if (verbOption == null)
            {
                if (helpInfo != null)
                {
                    DisplayHelpVerbText(context.Target, helpInfo, null);
                }
                return false;
            }
            if (verbOption.GetValue(context.Target) == null)
            {
                // Developer has not provided a default value and did not assign an instance
                verbOption.CreateInstance(context.Target);
            }
            var verbResult = DoParseArgumentsCore(context.ToCoreInstance(verbOption));
            if (!verbResult && helpInfo != null)
            {
                // Particular verb parsing failed, we try to print its help
                DisplayHelpVerbText(context.Target, helpInfo, context.FirstArgument);
            }
            return verbResult;
        }

        private bool TryParseHelpVerb(string[] args, object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, OptionMap optionMap)
        {
            var helpWriter = Settings.HelpWriter;
            if (helpInfo != null && helpWriter != null)
            {
                if (string.Compare(args[0], helpInfo.Right.LongName, Settings.GetStringComparison()) == 0)
                {
                    // User explicitly requested help
                    var verb = args.Length > 1 ? args[1] : null;
                    if (verb != null)
                    {
                        var verbOption = optionMap[verb];
                        if (verbOption != null)
                        {
                            if (verbOption.GetValue(options) == null)
                            {
                                // We need to create an instance also to render help
                                verbOption.CreateInstance(options);
                            }
                        }
                    }
                    DisplayHelpVerbText(options, helpInfo, verb);
                    return true;
                }
            }
            return false;
        }

        private void DisplayHelpVerbText(object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, string verb)
        {
            string helpText;
            if (verb == null)
            {
                HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, null, out helpText);
            }
            else
            {
                HelpVerbOptionAttribute.InvokeMethod(options, helpInfo, verb, out helpText);
            }
            if (Settings.HelpWriter != null)
            {
                Settings.HelpWriter.Write(helpText);
            }
        }
        #endregion

        #region Strict Parsing
        /// <summary>
        /// Default exit code (1) used by <see cref="Parser.ParseArgumentsStrict(string[],object)"/>
        /// and <see cref="Parser.ParseArgumentsStrict(string[],object,TextWriter)"/> overloads.
        /// </summary>
        public const int DefaultExitCodeFail = 1;

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes. If parsing fails, the method terminates
        /// the process with <see cref="Parser.DefaultExitCodeFail"/>.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <returns>True if parsing process succeed, otherwise exits the application.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArgumentsStrict(string[] args, object options)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            return DoParseArgumentsStrict(args, options, DefaultExitCodeFail);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes. If parsing fails, the method terminates
        /// the process with <paramref name="exitCode"/>
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <param name="exitCode">The exit code to use when quitting the application. It should be greater than zero.</param>
        /// <returns>True if parsing process succeed, otherwise exits the application.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArgumentsStrict(string[] args, object options, int exitCode)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            return DoParseArgumentsStrict(args, options, exitCode);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes. If parsing fails, the method terminates
        /// the process with <see cref="Parser.DefaultExitCodeFail"/>.
        /// This overload allows you to specify a <see cref="System.IO.TextWriter"/> derived instance for write text messages.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// usually <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        /// <returns>True if parsing process succeed, otherwise exits the application.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArgumentsStrict(string[] args, object options, TextWriter helpWriter)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            Settings.HelpWriter = helpWriter;

            return DoParseArgumentsStrict(args, options, DefaultExitCodeFail);
        }

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes. If parsing fails, the method terminates
        /// the process with <paramref name="exitCode"/>
        /// This overload allows you to specify a <see cref="System.IO.TextWriter"/> derived instance for write text messages.
        /// </summary>
        /// <param name="args">A <see cref="System.String"/> array of command line arguments.</param>
        /// <param name="options">An object's instance used to receive values.
        /// Parsing rules are defined using <see cref="CommandLine.BaseOptionAttribute"/> derived types.</param>
        /// <param name="helpWriter">Any instance derived from <see cref="System.IO.TextWriter"/>,
        /// usually <see cref="System.Console.Error"/>. Setting this argument to null, will disable help screen.</param>
        /// <param name="exitCode">The exit code to use when quitting the application. It should be greater than zero.</param>
        /// <returns>True if parsing process succeed, otherwise exits the application.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options, TextWriter helpWriter, int exitCode)
        {
            Assumes.NotNull(args, "args", SR.ArgumentNullException_ArgsStringArrayCannotBeNull);
            Assumes.NotNull(options, "options", SR.ArgumentNullException_OptionsInstanceCannotBeNull);

            Settings.HelpWriter = helpWriter;

            return DoParseArgumentsStrict(args, options, exitCode);
        }

        private bool DoParseArgumentsStrict(string[] args, object options, int exitCode)
        {
            if (!DoParseArguments(args, options))
            {
                InvokeAutoBuildIfNeeded(options);
                #region Unit Tests Code
#if !UNIT_TESTS
                Environment.Exit(exitCode);
#else
                Console.WriteLine("UNIT_TESTS symbol enabled.\n" +
                    "Simulating 'Environment.Exit({0})'.", exitCode);
                return false;
#endif
                #endregion
            }
            return true;
        }

        private void InvokeAutoBuildIfNeeded(object options)
        {
            if (Settings.HelpWriter == null ||
                options.HasHelp() ||
                options.HasVerbHelp())
            {
                return;
            }

            // We print help text for the user
            Settings.HelpWriter.Write(HelpText.AutoBuild(options,
                current => HelpText.DefaultParsingErrorsHandler(options, current), options.HasVerbs()));
        }
        #endregion

        private static void SetParserStateIfNeeded(object options, IEnumerable<ParsingError> errors)
        {
            if (!options.CanReceiveParserState())
            {
                return;
            }
            var property = ReflectionUtil.RetrievePropertyList<ParserStateAttribute>(options)[0].Left;
            // Developers are entitled to provide their implementation and instance
            if (property.GetValue(options, null) == null)
            {
                // Otherwise the parser will the default one
                property.SetValue(options, new ParserState(), null);
            }
            var parserState = (IParserState) property.GetValue(options, null);
            foreach (var error in errors)
            {
                parserState.Errors.Add(error);
            }
        }

        /// <summary>
        /// Frees resources owned by the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (Settings != null)
                {
                    Settings.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Class destructor.
        /// </summary>
        ~Parser()
        {
            Dispose(false);
        }

        private ParserContext _context;
        private bool _disposed;
        private static readonly IParser DefaultParser = new Parser(true);
    }
}