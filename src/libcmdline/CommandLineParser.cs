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
using System.IO;
using CommandLine.Internal;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// Default implementation for <see cref="CommandLine.ICommandLineParser"/>.
    /// </summary>
    public partial class CommandLineParser : ICommandLineParser, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.CommandLineParser"/> class.
        /// </summary>
        public CommandLineParser()
        {
            _settings = new CommandLineParserSettings();
        }

        // special constructor for singleton instance, parameter ignored
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
            var target = new TargetWrapper(options);

            IArgumentEnumerator arguments = new StringArrayEnumerator(args);
            while (arguments.MoveNext())
            {
                string argument = arguments.Current;
                if (!string.IsNullOrEmpty(argument))
                {
                    var parser = ArgumentParser.Create(argument, _settings.IgnoreUnknownArguments);
                    if (parser != null)
                    {
                        Internal.ParserState result = parser.Parse(arguments, optionMap, options);
                        if ((result & Internal.ParserState.Failure) == Internal.ParserState.Failure)
                        {
                            SetParserStateIfNeeded(options, parser.PostParsingState);
                            hadError = true;
                            continue;
                        }

                        if ((result & Internal.ParserState.MoveOnNextElement) == Internal.ParserState.MoveOnNextElement)
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