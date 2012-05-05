#region License
//
// Command Line Library: OptionMap.cs
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
using System;
using System.Collections.Generic;
#endregion

namespace CommandLine
{
    sealed class OptionMap
    {
		sealed class MutuallyExclusiveInfo
		{
			int count = 0;
			
			public MutuallyExclusiveInfo(OptionInfo option)
			{
				//BadOption = new BadOptionInfo();
				BadOption = option; 
			}
			
			//public BadOptionInfo BadOption { get; private set; }
			public OptionInfo BadOption { get; set; }
			
			public void IncrementOccurrence() { ++count; }
			
			public int Occurrence { get { return count; } }
		}
		
        readonly CommandLineParserSettings _settings;
        Dictionary<string, string> _names;
        Dictionary<string, OptionInfo> _map;
        //Dictionary<string, int> _mutuallyExclusiveSetMap;
		Dictionary<string, MutuallyExclusiveInfo> _mutuallyExclusiveSetMap;

        public OptionMap(int capacity, CommandLineParserSettings settings)
        {
            _settings = settings;

            IEqualityComparer<string> comparer;
            if (_settings.CaseSensitive)
                comparer = StringComparer.Ordinal;
            else
                comparer = StringComparer.OrdinalIgnoreCase;

            _names = new Dictionary<string, string>(capacity, comparer);
            _map = new Dictionary<string, OptionInfo>(capacity * 2, comparer);

            if (_settings.MutuallyExclusive)
			{
                //_mutuallyExclusiveSetMap = new Dictionary<string, int>(capacity, StringComparer.OrdinalIgnoreCase);
				_mutuallyExclusiveSetMap = new Dictionary<string, MutuallyExclusiveInfo>(capacity, StringComparer.OrdinalIgnoreCase);
			}
        }

        public OptionInfo this[string key]
        {
            get
            {
                OptionInfo option = null;

                if (_map.ContainsKey(key))
                    option = _map[key];
                else
                {
                    string optionKey = null;
                    if (_names.ContainsKey(key))
                    {
                        optionKey = _names[key];
                        option = _map[optionKey];
                    }
                }

                return option;
            }
            set
            {
                _map[key] = value;

                if (value.HasBothNames)
                    _names[value.LongName] = value.ShortName;
            }
        }

        internal object RawOptions { private get; set; }

        public bool EnforceRules()
        {
            return EnforceMutuallyExclusiveMap() && EnforceRequiredRule();
        }

        private bool EnforceRequiredRule()
        {
            foreach (OptionInfo option in _map.Values)
            {
                if (option.Required && !option.IsDefined)
                {
                    BuildAndSetPostParsingStateIfNeeded(this.RawOptions, option, true, null);
                    return false;
                }
            }
            return true;
        }

        private bool EnforceMutuallyExclusiveMap()
        {
            if (!_settings.MutuallyExclusive)
                return true;

            foreach (OptionInfo option in _map.Values)
            {
                if (option.IsDefined && option.MutuallyExclusiveSet != null)
                    BuildMutuallyExclusiveMap(option);
            }

            //foreach (int occurrence in _mutuallyExclusiveSetMap.Values)
			foreach (MutuallyExclusiveInfo info in _mutuallyExclusiveSetMap.Values)
            {
                if (info.Occurrence > 1) //if (occurrence > 1)
                {
                    //BuildAndSetPostParsingStateIfNeeded(this.RawOptions, null, null, true);
					BuildAndSetPostParsingStateIfNeeded(this.RawOptions, info.BadOption, null, true);
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
                //_mutuallyExclusiveSetMap.Add(setName, 0);
				_mutuallyExclusiveSetMap.Add(setName, new MutuallyExclusiveInfo(option));
			}

            //_mutuallyExclusiveSetMap[setName]++;
			_mutuallyExclusiveSetMap[setName].IncrementOccurrence();
        }

        private static void BuildAndSetPostParsingStateIfNeeded(object options, OptionInfo option, bool? required, bool? mutualExclusiveness)
        {
            if (options is CommandLineOptionsBase)
            {
                ParsingError error = new ParsingError();
                //if (option != null)
                //{
                    error.BadOption.ShortName = option.ShortName; //error.BadOptionShortName = option.ShortName;
                    error.BadOption.LongName = option.LongName; //error.BadOptionLongName = option.LongName;
                //}
                if (required != null) error.ViolatesRequired = required.Value;
                if (mutualExclusiveness != null) error.ViolatesMutualExclusiveness = mutualExclusiveness.Value;

                ((CommandLineOptionsBase)options).InternalLastPostParsingState.Errors.Add(error);
            }
        }
    }
}