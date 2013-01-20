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
using System.IO;
using CommandLine.Internal;
using CommandLine.Text;

#endregion

namespace CommandLine
{
    partial class CommandLineParser
    {
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
    }
}