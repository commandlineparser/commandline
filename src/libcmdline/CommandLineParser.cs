#region License
//
// Command Line Library: CommandLineParser.cs
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
using System.IO;
using System.Reflection;
using CommandLine.Internal;
using CommandLine.Text;

#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// Default implementation for <see cref="CommandLine.ICommandLineParser"/>.
    /// </summary>
    public class CommandLineParser : ICommandLineParser, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.CommandLineParser"/> class.
        /// </summary>
        public CommandLineParser()
        {
            _settings = new CommandLineParserSettings();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "singleton", Justification = "The constructor that accepts a boolean is designed to support default singleton, the parameter is ignored.")]
        private CommandLineParser(bool singleton)
        {
            _settings = new CommandLineParserSettings(false, false, Console.Error);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.CommandLineParser"/> class,
        /// configurable with a <see cref="CommandLine.CommandLineParserSettings"/> object.
        /// </summary>
        /// <param name="settings">The <see cref="CommandLine.CommandLineParserSettings"/> object is used to configure
        /// aspects and behaviors of the parser.</param>
        public CommandLineParser(CommandLineParserSettings settings)
        {
            Assumes.NotNull(settings, "settings", SR.ArgumentNullException_CommandLineParserSettingsInstanceCannotBeNull);
            _settings = settings;
        }

        /// <summary>
        /// Singleton instance created with basic defaults.
        /// </summary>
        public static ICommandLineParser Default
        {
            get { return DefaultParser; }
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

            _settings.HelpWriter = helpWriter;
            return DoParseArguments(args, options);
        }

        private bool DoParseArguments(string[] args, object options)
        {
            var pair = ReflectionUtil.RetrieveMethod<HelpOptionAttribute>(options);
            var helpWriter = _settings.HelpWriter;

            // Keep a local reference for helper methods
            _args = args;

            if (pair != null && helpWriter != null)
            {
                // If help can be handled is displayed if is requested or if parsing fails
                if (ParseHelp(args, pair.Right) || !DoParseArgumentsUsingVerbs(args, options))
                {
                    string helpText;
                    HelpOptionAttribute.InvokeMethod(options, pair, out helpText);
                    helpWriter.Write(helpText);
                    return false;
                }
                return true;
            }

            return DoParseArgumentsUsingVerbs(args, options);
        }

        private bool DoParseArgumentsCore(string[] args, object options)
        {
            bool hadError = false;
            var optionMap = OptionMap.Create(options, _settings);
            optionMap.SetDefaults();
            var target = new Target(options);

            IArgumentEnumerator arguments = new StringArrayEnumerator(args);
            while (arguments.MoveNext())
            {
                string argument = arguments.Current;
                if (!string.IsNullOrEmpty(argument))
                {
                    var parser = ArgumentParser.Create(argument, _settings.IgnoreUnknownArguments);
                    if (parser != null)
                    {
                        PresentParserState result = parser.Parse(arguments, optionMap, options);
                        if ((result & PresentParserState.Failure) == PresentParserState.Failure)
                        {
                            SetParserStateIfNeeded(options, parser.PostParsingState);
                            hadError = true;
                            continue;
                        }

                        if ((result & PresentParserState.MoveOnNextElement) == PresentParserState.MoveOnNextElement)
                        {
                            arguments.MoveNext();
                        }
                    }
                    else if (target.IsValueListDefined)
                    {
                        if (!target.AddValueItemIfAllowed(argument))
                        {
                            hadError = true;
                        }
                    }
                }
            }

            hadError |= !optionMap.EnforceRules();

            return !hadError;
        }

        private bool ParseHelp(string[] args, HelpOptionAttribute helpOption)
        {
            bool caseSensitive = _settings.CaseSensitive;

            for (int i = 0; i < args.Length; i++)
            {
                if (helpOption.ShortName != null)
                {
                    if (ArgumentParser.CompareShort(args[i], helpOption.ShortName, caseSensitive))
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrEmpty(helpOption.LongName))
                {
                    if (ArgumentParser.CompareLong(args[i], helpOption.LongName, caseSensitive))
                    {
                        return true;
                    }
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
            if (_args == null || _args.Length < 1)
            {
                return false;
            }
            return string.Compare(_args[0], verb, _settings.StringComparison) == 0;
        }

        private bool DoParseArgumentsUsingVerbs(string[] args, object options)
        {
            var verbs = ReflectionUtil.RetrievePropertyList<VerbOptionAttribute>(options);
            if (verbs.Count == 0)
            {
                // No verbs defined, hence we can run default parsing subsystem
                return DoParseArgumentsCore(args, options);
            }
            var helpInfo = ReflectionUtil.RetrieveMethod<HelpVerbOptionAttribute>(options);
            if (args.Length == 0)
            {
                if (helpInfo != null || _settings.HelpWriter != null)
                {
                    DisplayHelpVerbText(options, helpInfo, null);
                }
                return false;
            }
            var optionMap = OptionMap.Create(options, verbs, _settings);
            // Read the verb from command line arguments
            if (TryParseHelpVerb(args, options, helpInfo, optionMap))
            {
                // Since user requested help, parsing is considered a fail
                return false;
            }
            var verbOption = optionMap[args[0]];
            // User invoked a bad verb name
            if (verbOption == null)
            {
                if (helpInfo != null)
                {
                    DisplayHelpVerbText(options, helpInfo, null);
                }
                return false;
            }
            if (verbOption.GetValue(options) == null)
            {
                // Developer has not provided a default value and did not assign an instance
                verbOption.CreateInstance(options);
            }
            var verbArgs = new string[args.Length - 1];
            if (args.Length > 1)
            {
                Array.Copy(args, 1, verbArgs, 0, args.Length - 1);
            }
            var verbResult = DoParseArgumentsCore(verbArgs, verbOption.GetValue(options));
            if (!verbResult)
            {
                // Particular verb parsing failed, we try to print its help
                DisplayHelpVerbText(options, helpInfo, args[0]);
            }
            return verbResult;
        }

        private bool TryParseHelpVerb(string[] args, object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, OptionMap optionMap)
        {
            var helpWriter = _settings.HelpWriter;
            if (helpInfo != null && helpWriter != null)
            {
                if (string.Compare(args[0], helpInfo.Right.LongName, _settings.StringComparison) == 0)
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
            if (_settings.HelpWriter != null)
            {
                _settings.HelpWriter.Write(helpText);
            }
        }
        #endregion

        #region Strict Parsing
        /// <summary>
        /// Default exit code (1) used by <see cref="CommandLine.CommandLineParser.ParseArgumentsStrict(string[],object)"/>
        /// and <see cref="CommandLine.CommandLineParser.ParseArgumentsStrict(string[],object,TextWriter)"/> overloads.
        /// </summary>
        public const int DefaultExitCodeFail = 1;

        /// <summary>
        /// Parses a <see cref="System.String"/> array of command line arguments, setting values in <paramref name="options"/>
        /// parameter instance's public fields decorated with appropriate attributes. If parsing fails, the method terminates
        /// the process with <see cref="CommandLineParser.DefaultExitCodeFail"/>.
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
        /// the process with <see cref="CommandLineParser.DefaultExitCodeFail"/>.
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

            _settings.HelpWriter = helpWriter;

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

            _settings.HelpWriter = helpWriter;

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
                Console.WriteLine(string.Format("UNIT_TESTS symbol enabled.\n" +
                    "Simulating 'Environment.Exit({0})'.", exitCode));
                return false;
#endif
                #endregion
            }
            return true;
        }

        private void InvokeAutoBuildIfNeeded(object options)
        {
            if (_settings.HelpWriter == null)
            {
                return;
            }
            var hasHelpOption = ReflectionUtil.RetrieveMethod<HelpOptionAttribute>(options) != null;
            var hasVerbHelpOption = ReflectionUtil.RetrieveMethod<HelpVerbOptionAttribute>(options) != null;
            if (hasHelpOption ||
                hasVerbHelpOption)
            {
                // We do not need to anything
                return;
            }

            var hasVerbs = ReflectionUtil.RetrievePropertyList<VerbOptionAttribute>(options).Count > 0;

            // We print help text for the user
            _settings.HelpWriter.Write(HelpText.AutoBuild(options,
                current => HelpText.DefaultParsingErrorsHandler(options, current), hasVerbs));
        }
        #endregion

        private static void SetParserStateIfNeeded(object options, IEnumerable<ParsingError> errors)
        {
            var list = ReflectionUtil.RetrievePropertyList<ParserStateAttribute>(options);
            if (list.Count == 0)
            {
                return;
            }
            var property = list[0].Left;
            // Developers are entitled to provide their implementation and instance
            if (property.GetValue(options, null) == null)
            {
                // Otherwise the parser will the default one
                property.SetValue(options, new CommandLine.ParserState(), null);
            }
            var parserState = (IParserState) property.GetValue(options, null);
            foreach (var error in errors)
            {
                parserState.Errors.Add(error);
            }
        }

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
                if (_settings != null)
                {
                    _settings.Dispose();
                }
                _disposed = true;
            }
        }

        ~CommandLineParser()
        {
            Dispose(false);
        }

        private string[] _args;
        private bool _disposed;
        private static readonly ICommandLineParser DefaultParser = new CommandLineParser(true);
        private readonly CommandLineParserSettings _settings;
    }
}