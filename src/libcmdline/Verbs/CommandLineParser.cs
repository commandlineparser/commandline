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
#region Preprocessor Directives
// Comment this line if you want disable support for verb commands.
// Here the symbol is provided for unit tests convenience, if you don't need
// this feature please exclude this file from your source tree.
#define CMDLINE_VERBS
#endregion
#if CMDLINE_VERBS
#region Using Directives
using System;
using System.Reflection;
using CommandLine.Internal;
#endregion
//
// Needs CMDLINE_VERBS preprocessor directive uncommented in CommandLine.cs.
//
namespace CommandLine
{
    partial class CommandLineParser
    {
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
            var optionMap = OptionInfo.CreateMap(options, verbs, _settings);
            // Read the verb from command line arguments
            if (TryParseHelpVerb(args, options, helpInfo))
            {
                // Since user requested help, parsing is considered a fail
                return false;
            }
            var verbOption = optionMap[args[0]];
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

        private bool TryParseHelpVerb(string[] args, object options, Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo)
        {
            var helpWriter = _settings.HelpWriter;
            if (helpInfo != null && helpWriter != null)
            {
                if (string.Compare(args[0], helpInfo.Right.LongName, _settings.StringComparison) == 0)
                {
                    // User explicitly requested help
                    var verb = args.Length > 1 ? args[1] : null;
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
            _settings.HelpWriter.Write(helpText);
        }
    }
}
#endif