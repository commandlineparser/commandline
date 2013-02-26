#region License
// <copyright file="OptionMap.cs" company="Giacomo Stelluti Scala">
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
using System.Reflection;
using CommandLine.Extensions;
using CommandLine.Infrastructure;
#endregion

namespace CommandLine.Parsing
{
    internal sealed class OptionMap
    {
        private readonly ParserSettings _settings;
        private readonly Dictionary<string, string> _names;
        private readonly Dictionary<string, OptionInfo> _map;
        private readonly Dictionary<string, MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMap"/> class.
        /// It is internal rather than private for unit testing purpose.
        /// </summary>
        /// <param name="capacity">Initial internal capacity.</param>
        /// <param name="settings">Parser settings instance.</param>
        internal OptionMap(int capacity, ParserSettings settings) 
        {
            _settings = settings;

            IEqualityComparer<string> comparer =
                _settings.CaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
            _names = new Dictionary<string, string>(capacity, comparer);
            _map = new Dictionary<string, OptionInfo>(capacity * 2, comparer);

            if (_settings.MutuallyExclusive)
            {
                _mutuallyExclusiveSetMap = new Dictionary<string, MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
            }
        }

        internal object RawOptions
        {
            private get; set;
        }

        public OptionInfo this[string key]
        {
            get
            {
                OptionInfo option = null;

                if (_map.ContainsKey(key))
                {
                    option = _map[key];
                }
                else
                {
                    if (_names.ContainsKey(key))
                    {
                        var optionKey = _names[key];
                        option = _map[optionKey];
                    }
                }

                return option;
            }

            set
            {
                _map[key] = value;

                if (value.HasBothNames)
                {
                    // ReSharper disable PossibleInvalidOperationException
                    _names[value.LongName] = new string(value.ShortName.Value, 1);
                    // ReSharper restore PossibleInvalidOperationException
                }
            }
        }

        public static OptionMap Create(object target, ParserSettings settings)
        {
            var list = ReflectionHelper.RetrievePropertyList<BaseOptionAttribute>(target);
            if (list == null)
            {
                return null;
            }

            var map = new OptionMap(list.Count, settings);

            foreach (var pair in list)
            {
                if (pair.Left != null && pair.Right != null)
                {
                    string uniqueName;
                    if (pair.Right.AutoLongName)
                    {
                        uniqueName = pair.Left.Name.ToLowerInvariant();
                        pair.Right.LongName = uniqueName;
                    }
                    else
                    {
                        uniqueName = pair.Right.UniqueName;
                    }

                    map[uniqueName] = new OptionInfo(pair.Right, pair.Left, settings.ParsingCulture);
                }
            }

            map.RawOptions = target;
            return map;
        }

        public static OptionMap Create(
            object target,
            IList<Pair<PropertyInfo, VerbOptionAttribute>> verbs,
            ParserSettings settings)
        {
            var map = new OptionMap(verbs.Count, settings);

            foreach (var verb in verbs)
            {
                var optionInfo = new OptionInfo(verb.Right, verb.Left, settings.ParsingCulture)
                {
                    HasParameterLessCtor = verb.Left.PropertyType.GetConstructor(Type.EmptyTypes) != null
                };

                if (!optionInfo.HasParameterLessCtor && verb.Left.GetValue(target, null) == null)
                {
                    throw new ParserException("Type {0} must have a parameterless constructor or" +
                        " be already initialized to be used as a verb command.".FormatInvariant(verb.Left.PropertyType));
                }

                map[verb.Right.UniqueName] = optionInfo;
            }

            map.RawOptions = target;
            return map;
        }

        public bool EnforceRules()
        {
            return EnforceMutuallyExclusiveMap() && EnforceRequiredRule();
        }

        public void SetDefaults()
        {
            foreach (OptionInfo option in _map.Values)
            {
                option.SetDefault(RawOptions);
            }
        }

        private static void SetParserStateIfNeeded(object options, OptionInfo option, bool? required, bool? mutualExclusiveness)
        {
            var list = ReflectionHelper.RetrievePropertyList<ParserStateAttribute>(options);
            if (list.Count == 0)
            {
                return;
            }

            var property = list[0].Left;

            // This method can be called when parser state is still not intialized
            if (property.GetValue(options, null) == null)
            {
                property.SetValue(options, new CommandLine.ParserState(), null);
            }

            var parserState = (IParserState)property.GetValue(options, null);
            if (parserState == null)
            {
                return;
            }

            var error = new ParsingError
            {
                BadOption =
                {
                    ShortName = option.ShortName,
                    LongName = option.LongName
                }
            };

            if (required != null)
            {
                error.ViolatesRequired = required.Value;
            }

            if (mutualExclusiveness != null)
            {
                error.ViolatesMutualExclusiveness = mutualExclusiveness.Value;
            }

            parserState.Errors.Add(error);
        }

        private bool EnforceRequiredRule()
        {
            var requiredRulesAllMet = true;

            foreach (var option in _map.Values)
            {
                if (option.Required && !(option.IsDefined && option.ReceivedValue))
                {
                    SetParserStateIfNeeded(RawOptions, option, true, null);
                    requiredRulesAllMet = false;
                }
            }

            return requiredRulesAllMet;
        }

        private bool EnforceMutuallyExclusiveMap()
        {
            if (!_settings.MutuallyExclusive)
            {
                return true;
            }

            foreach (var option in _map.Values)
            {
                if (option.IsDefined && option.MutuallyExclusiveSet != null)
                {
                    BuildMutuallyExclusiveMap(option);
                }
            }

            foreach (var info in _mutuallyExclusiveSetMap.Values)
            {
                if (info.Occurrence > 1)
                {
                    SetParserStateIfNeeded(RawOptions, info.BadOption, null, true);
                    return false;
                }
            }

            return true;
        }

        private void BuildMutuallyExclusiveMap(OptionInfo option)
        {
            var setName = option.MutuallyExclusiveSet;
            if (!_mutuallyExclusiveSetMap.ContainsKey(setName))
            {
                _mutuallyExclusiveSetMap.Add(setName, new MutuallyExclusiveInfo(option));
            }

            _mutuallyExclusiveSetMap[setName].IncrementOccurrence();
        }
  
        private sealed class MutuallyExclusiveInfo
        {
            private int _count;

            public MutuallyExclusiveInfo(OptionInfo option)
            {
                BadOption = option;
            }

            public OptionInfo BadOption { get; private set; }

            public int Occurrence
            {
                get { return this._count; }
            }

            public void IncrementOccurrence()
            {
                ++this._count;
            }
        }
    }
}