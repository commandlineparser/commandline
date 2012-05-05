#region License
//
// Command Line Library: CommandLineParser.cs
//
// Author:
//   Giacomo Stelluti Scala (gsscoder@gmail.com)
//
// Copyright (C) 2005 - 2012 Giacomo Stelluti Scala
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Provides methods to parse command line arguments.
    /// Default implementation for <see cref="CommandLine.ICommandLineParser"/>.
    /// </summary>
    public class CommandLineParser : ICommandLineParser
    {
        private CommandLineParserSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.CommandLineParser"/> class.
        /// </summary>
        public CommandLineParser()
        {
            _settings = new CommandLineParserSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.CommandLineParser"/> class,
        /// configurable with a <see cref="CommandLine.CommandLineParserSettings"/> object.
        /// </summary>
        /// <param name="settings">The <see cref="CommandLine.CommandLineParserSettings"/> object is used to configure
        /// aspects and behaviors of the parser.</param>
        public CommandLineParser(CommandLineParserSettings settings)
        {
            Assumes.NotNull(settings, "settings");

            _settings = settings;
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
            return ParseArguments(args, options, _settings.HelpWriter);
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
        /// <returns>True if parsing process succeed.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="options"/> is null.</exception>
        public virtual bool ParseArguments(string[] args, object options, TextWriter helpWriter)
        {
            Assumes.NotNull(args, "args");
            Assumes.NotNull(options, "options");

            var pair = ReflectionUtil.RetrieveMethod<HelpOptionAttribute>(options);

            if (pair != null && helpWriter != null)
            {
                if (ParseHelp(args, pair.Right) || !DoParseArguments(args, options))
                {
                    string helpText;
                    HelpOptionAttribute.InvokeMethod(options, pair, out helpText);
                    helpWriter.Write(helpText);
                    return false;
                }
                return true;
            }

            return DoParseArguments(args, options);
        }

        private bool DoParseArguments(string[] args, object options)
        {
            bool hadError = false;
            var optionMap = OptionInfo.CreateMap(options, _settings);
            var target = new TargetWrapper(options);

            IArgumentEnumerator arguments = new StringArrayEnumerator(args);
            while (arguments.MoveNext())
            {
                string argument = arguments.Current;
                if (argument != null && argument.Length > 0)
                {
                    ArgumentParser parser = ArgumentParser.Create(argument);
                    if (parser != null)
                    {
                        ParserState result = parser.Parse(arguments, optionMap, options);
                        if ((result & ParserState.Failure) == ParserState.Failure)
                        {
                            SetPostParsingStateIfNeeded(options, parser.PostParsingState);
                            hadError = true;
                            continue; //break;
                        }

                        if ((result & ParserState.MoveOnNextElement) == ParserState.MoveOnNextElement)
                            arguments.MoveNext();
                    }
                    else if (target.IsValueListDefined)
                    {
                        if (!target.AddValueItemIfAllowed(argument))
                        {
                            hadError = true;
                            continue; //break;
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
                if (!string.IsNullOrEmpty(helpOption.ShortName))
                {
                    if (ArgumentParser.CompareShort(args[i], helpOption.ShortName, caseSensitive))
                        return true;
                }

                if (!string.IsNullOrEmpty(helpOption.LongName))
                {
                    if (ArgumentParser.CompareLong(args[i], helpOption.LongName, caseSensitive))
                        return true;
                }
            }

            return false;
        }

        //private static void SetPostParsingStateIfNeeded(object options, PostParsingState state)
        private static void SetPostParsingStateIfNeeded(object options, List<ParsingError> state)
        {
            if (options is CommandLineOptionsBase)
                ((CommandLineOptionsBase)options).InternalLastPostParsingState.Errors.AddRange(state);
        }
    }
}