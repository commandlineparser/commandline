#region License
//
// Command Line Library: CommandLine.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
#endregion

namespace CommandLine.Internal
{
    internal sealed class OptionMap
    {
        private sealed class MutuallyExclusiveInfo
        {
            private MutuallyExclusiveInfo() { }

            public MutuallyExclusiveInfo(OptionInfo option)
            {
                BadOption = option;
            }

            public OptionInfo BadOption { get; private set; }

            public void IncrementOccurrence() { ++_count; }

            public int Occurrence { get { return _count; } }

            private int _count;
        }

        public OptionMap(int capacity, CommandLineParserSettings settings)
        {
            _settings = settings;

            IEqualityComparer<string> comparer;
            if (_settings.CaseSensitive)
            {
                comparer = StringComparer.Ordinal;
            }
            else
            {
                comparer = StringComparer.OrdinalIgnoreCase;
            }
            _names = new Dictionary<string, string>(capacity, comparer);
            _map = new Dictionary<string, OptionInfo>(capacity * 2, comparer);
            if (_settings.MutuallyExclusive)
            {
                _mutuallyExclusiveSetMap = new Dictionary<string, MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
            }
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
                    _names[value.LongName] = new string(value.ShortName.Value, 1);
                }
            }
        }

        internal object RawOptions { private get; set; }

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

        private bool EnforceRequiredRule()
        {
            bool requiredRulesAllMet = true;
            foreach (OptionInfo option in _map.Values)
            {
                if (option.Required && !option.IsDefined)
                {
                    BuildAndSetPostParsingStateIfNeeded(RawOptions, option, true, null);
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
            foreach (OptionInfo option in _map.Values)
            {
                if (option.IsDefined && option.MutuallyExclusiveSet != null)
                {
                    BuildMutuallyExclusiveMap(option);
                }
            }
            foreach (MutuallyExclusiveInfo info in _mutuallyExclusiveSetMap.Values)
            {
                if (info.Occurrence > 1)
                {
                    BuildAndSetPostParsingStateIfNeeded(RawOptions, info.BadOption, null, true);
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

        private static void BuildAndSetPostParsingStateIfNeeded(object options, OptionInfo option, bool? required, bool? mutualExclusiveness)
        {
            var commandLineOptionsBase = options as CommandLineOptionsBase;
            if (commandLineOptionsBase == null)
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
            if (required != null) { error.ViolatesRequired = required.Value; }
            if (mutualExclusiveness != null) { error.ViolatesMutualExclusiveness = mutualExclusiveness.Value; }
            (commandLineOptionsBase).InternalLastPostParsingState.Errors.Add(error);
        }

        private readonly CommandLineParserSettings _settings;
        private readonly Dictionary<string, string> _names;
        private readonly Dictionary<string, OptionInfo> _map;
        private readonly Dictionary<string, MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;
    }
}