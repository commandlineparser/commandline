#region License
//
// Command Line Library: CommandLineVerb.cs
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
using System.Collections.Generic;
using System.Reflection;
using CommandLine.Internal;
#endregion
//
// Needs CMDLINE_VERBS preprocessor directive uncommented in CommandLine.cs.
//
namespace CommandLine
{
    #region Core
    namespace Internal
    {
        partial class OptionInfo
        {
            public bool HasParameterLessCtor
            {
                get { return _hasParameterLessCtor; }
                set
                {
                    lock (_setValueLock)
                    {
                        _hasParameterLessCtor = value;
                    }
                }
            }

            public object GetValue(object target)
            {
                lock (_setValueLock)
                {
                    return _property.GetValue(target, null);
                }
            }

            public void CreateInstance(object target)
            {
                lock (_setValueLock)
                {
                    try
                    {
                        _property.SetValue(target, Activator.CreateInstance(_property.PropertyType), null);
                    }
                    catch (Exception e)
                    {
                        throw new CommandLineParserException("Instance defined for verb command could not be created.", e);
                    }
                }
            }

            public static OptionMap CreateMap(object target,
                IList<Pair<PropertyInfo, VerbOptionAttribute>> verbs, CommandLineParserSettings settings)
            {
                var map = new OptionMap(verbs.Count, settings);
                foreach (var verb in verbs)
                {
                    var optionInfo = new OptionInfo(verb.Right, verb.Left)
                        {
                            HasParameterLessCtor = verb.Left.PropertyType.GetConstructor(Type.EmptyTypes) != null

                        };
                    if (!optionInfo.HasParameterLessCtor && verb.Left.GetValue(target, null) == null)
                    {
                        throw new CommandLineParserException(string.Format("Type {0} must have a parameterless constructor or" +
                            " be already initialized to be used as a verb command.", verb.Left.PropertyType));
                    }
                    map[verb.Right.UniqueName] = optionInfo;
                }
                map.RawOptions = target;
                return map;
            }

            private bool _hasParameterLessCtor;
        }
    }
    #endregion

    #region Attributes
    /// <summary>
    /// Models a verb command specification.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class VerbOptionAttribute : OptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.VerbOptionAttribute"/> class.
        /// </summary>
        /// <param name="longName">The long name of the verb command.</param>
        public VerbOptionAttribute(string longName)
            : base(longName)
        {
            Assumes.NotNullOrEmpty(longName, "longName");
        }

        /// <summary>
        /// Verb commands do not support short name by design.
        /// </summary>
        public override char? ShortName
        {
            get { return null; }
            internal set {}
        }

        /// <summary>
        /// Verb commands cannot be mandatory since are mutually exclusive by design.
        /// </summary>
        public override bool Required
        {
            get { return false; }
            set {}
        }
    }

    /// <summary>
    /// Indicates the instance method that must be invoked when it becomes necessary show your help screen.
    /// The method signature is an instance method with that accepts and returns a <see cref="System.String"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class HelpVerbOptionAttribute : BaseOptionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.HelpVerbOptionAttribute"/> class.
        /// Although it is possible, it is strongly discouraged redefine the long name for this option
        /// not to disorient your users.
        /// </summary>
        public HelpVerbOptionAttribute()
            : this("help")
        {
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.HelpVerbOptionAttribute"/> class
        /// with the specified long name. Use parameterless constructor instead.
        /// </summary>
        /// <param name="longName"></param>
        public HelpVerbOptionAttribute(string longName)
        {
            LongName = longName;
            HelpText = DefaultHelpText;
        }

        /// <summary>
        /// Help verb command do not support short name by design.
        /// </summary>
        public override char? ShortName
        {
            get { return null; }
            internal set { throw new InvalidOperationException("Help verb command do not support short name by design."); }
        }

        /// <summary>
        /// Help verb command like ordinary help option cannot be mandatory by design.
        /// </summary>
        public override bool Required
        {
            get { return false; }
            set { throw new InvalidOperationException("Help verb command cannot be mandatory by design."); }
        }

        internal static void InvokeMethod(object target,
            Pair<MethodInfo, HelpVerbOptionAttribute> helpInfo, string verb, out string text)
        {
            text = null;
            var method = helpInfo.Left;
            if (!CheckMethodSignature(method))
            {
                throw new MemberAccessException(string.Format(
                    "{0} has an incorrect signature. " +
                    "Help verb command requires a method that accepts and returns a string.", method.Name));
            }
            text = (string) method.Invoke(target, new object[] {verb});
        }

        private static bool CheckMethodSignature(MethodInfo value)
        {
            if (value.ReturnType == typeof(string) && value.GetParameters().Length == 1)
            {
                return value.GetParameters()[0].ParameterType == typeof(string);
            }
            return false;
        }

        private const string DefaultHelpText = "Display more information on a specific command.";
    }
    #endregion

    #region Parser
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
    #endregion
}
#endif