#region License
//
// Command Line Library: OptionInfo.cs
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
#define CMDLINE_VERBS
// Preprocessor directives for enabling/disabling extensions,
// don't operate on the following symbols, but on previous ones.
#define CMDLINE_OPEN_PARSER     // opens CommandLineParser type
#define CMDLINE_OPEN_OPTIONINFO // opens OptionInfo type
#if !CMDLINE_VERBS
#undef CMDLINE_OPEN_PARSER
#undef CMDLINE_OPEN_OPTIONINFO
#endif
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
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
#if CMDLINE_OPEN_OPTIONINFO
    internal sealed partial class OptionInfo
#else
    internal sealed class OptionInfo
#endif
    {
        public OptionInfo(OptionAttribute attribute, PropertyInfo property)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute", "The attribute is mandatory");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property", "The property is mandatory");
            }
            _required = attribute.Required;
            _helpText = attribute.HelpText;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
            _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            _defaultValue = attribute.DefaultValue;
            _hasDefaultValue = attribute.HasDefaultValue;
            _attribute = attribute;
            _property = property;
        }

#if UNIT_TESTS
        internal OptionInfo(char? shortName, string longName)
        {
            _shortName = shortName;
            _longName = longName;
        }
#endif

        public static OptionMap CreateMap(object target, CommandLineParserSettings settings)
        {
            var list = ReflectionUtil.RetrievePropertyList<OptionAttribute>(target);
            if (list == null)
            {
                return null;
            }
            var map = new OptionMap(list.Count, settings);
            foreach (var pair in list)
            {
                if (pair.Left != null && pair.Right != null)
                {
                    map[pair.Right.UniqueName] = new OptionInfo(pair.Right, pair.Left);
                }
            }
            map.RawOptions = target;
            return map;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
            {
                return SetValueList(value, options);
            }
            if (ReflectionUtil.IsNullableType(_property.PropertyType))
            {
                return SetNullableValue(value, options);
            }
            return SetValueScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            Type elementType = _property.PropertyType.GetElementType();
            Array array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    lock (_setValueLock)
                    {
                        array.SetValue(Convert.ChangeType(values[i], elementType, Thread.CurrentThread.CurrentCulture), i);
                        _property.SetValue(options, array, null);
                    }
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            return true;
        }

        private bool SetValueScalar(string value, object options)
        {
            try
            {
                if (_property.PropertyType.IsEnum)
                {
                    lock (_setValueLock)
                    {
                        _property.SetValue(options, Enum.Parse(_property.PropertyType, value, true), null);
                    }
                }
                else
                {
                    lock (_setValueLock)
                    {
                        _property.SetValue(options, Convert.ChangeType(value, _property.PropertyType, Thread.CurrentThread.CurrentCulture), null);
                    }
                }
            }
            catch (InvalidCastException) { return false; } // Convert.ChangeType
            catch (FormatException) { return false; } // Convert.ChangeType
            catch (ArgumentException) { return false; } // Enum.Parse
            catch (OverflowException) { return false; } // Convert.ChangeType
            return true;
        }

        private bool SetNullableValue(string value, object options)
        {
            var nc = new NullableConverter(_property.PropertyType);
            try
            {
                lock (_setValueLock)
                {
                    _property.SetValue(options, nc.ConvertFromString(null, Thread.CurrentThread.CurrentCulture, value), null);
                }
            }
            // the FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException,
            // so we've catch directly System.Exception
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetValue(bool value, object options)
        {
            lock (_setValueLock)
            {
                _property.SetValue(options, value, null);
                return true;
            }
        }

        private bool SetValueList(string value, object options)
        {
            lock (_setValueLock)
            {
                _property.SetValue(options, new List<string>(), null);
                var fieldRef = (IList<string>)_property.GetValue(options, null);
                var values = value.Split(((OptionListAttribute)_attribute).Separator);
                for (int i = 0; i < values.Length; i++)
                {
                    fieldRef.Add(values[i]);
                }
                return true;
            }
        }

        public void SetDefault(object options)
        {
            if (_hasDefaultValue)
            {
                lock (_setValueLock)
                {
                    try
                    {
                        _property.SetValue(options, _defaultValue, null);
                    }
                    catch (Exception e)
                    {
                        throw new CommandLineParserException("Bad default value.", e);
                    }
                }
            }
        }

        public char? ShortName
        {
            get { return _shortName; }
        }

        public string LongName
        {
            get { return _longName; }
        }

        internal string NameWithSwitch
        {
            get
            {
                if (_longName != null)
                {
                    return string.Concat("--", _longName);
                }
                return string.Concat("-", _shortName);
            }
        }

        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
        }

        public bool Required
        {
            get { return _required; }
        }

        public string HelpText
        {
            get { return _helpText; }
        }

        public bool IsBoolean
        {
            get { return _property.PropertyType == typeof(bool); }
        }

        public bool IsArray
        {
            get { return _property.PropertyType.IsArray; }
        }

        public bool IsAttributeArrayCompatible
        {
            get { return _attribute is OptionArrayAttribute; }
        }

        public bool IsDefined { get; set; }

        public bool HasBothNames
        {
            get { return (_shortName != null && _longName != null); }
        }

        private readonly OptionAttribute _attribute;
        private readonly PropertyInfo _property;
        private readonly bool _required;
        private readonly string _helpText;
        private readonly char? _shortName;
        private readonly string _longName;
        private readonly string _mutuallyExclusiveSet;
        private readonly object _defaultValue;
        private readonly bool _hasDefaultValue;
        private readonly object _setValueLock = new object();
    }
}