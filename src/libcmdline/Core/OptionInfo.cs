#region License
//
// Command Line Library: OptionInfo.cs
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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;
#endregion

namespace CommandLine
{
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    sealed class OptionInfo
    {
        private readonly OptionAttribute _attribute;
        private readonly FieldInfo _field;
        private bool _required;
        private string _helpText;
        private string _shortName;
        private string _longName;
        private string _mutuallyExclusiveSet;

        private object _setValueLock = new object();

        public OptionInfo(OptionAttribute attribute, FieldInfo field)
        {
            _required = attribute.Required;
            _helpText = attribute.HelpText;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
            _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            _field = field;
            _attribute = attribute;
        }

#if UNIT_TESTS
        internal OptionInfo(string shortName, string longName)
        {
            _shortName = shortName;
            _longName = longName;
        }
#endif
        public static OptionMap CreateMap(object target, CommandLineParserSettings settings)
        {
            var list = ReflectionUtil.RetrieveFieldList<OptionAttribute>(target);
            OptionMap map = new OptionMap(list.Count, settings);

            foreach (Pair<FieldInfo, OptionAttribute> pair in list)
            {
                map[pair.Right.UniqueName] = new OptionInfo(pair.Right, pair.Left);
            }

            map.RawOptions = target;

            return map;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
                return SetValueList(value, options);

            if (ReflectionUtil.IsNullableType(_field.FieldType))
                return SetNullableValue(value, options);

            return SetValueScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            Type elementType = _field.FieldType.GetElementType();
            Array array = Array.CreateInstance(elementType, values.Count);
            
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    lock (_setValueLock)
                    {
                        array.SetValue(Convert.ChangeType(values[i], elementType, CultureInfo.InvariantCulture), i);
                        _field.SetValue(options, array);
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
                if (_field.FieldType.IsEnum)
                {
                    lock (_setValueLock)
                    {
                        _field.SetValue(options, Enum.Parse(_field.FieldType, value, true));
                    }
                }
                else
                {
                    lock (_setValueLock)
                    {
                        _field.SetValue(options, Convert.ChangeType(value, _field.FieldType, CultureInfo.InvariantCulture));
                    }
                }
            }
            catch (InvalidCastException) // Convert.ChangeType
            {
                return false;
            }
            catch (FormatException) // Convert.ChangeType
            {
                return false;
            }
            catch (ArgumentException) // Enum.Parse
            {
                return false;
            }

            return true;
        }

        private bool SetNullableValue(string value, object options)
        {
            var nc = new NullableConverter(_field.FieldType);

            try
            {
                lock (_setValueLock)
                {
                    _field.SetValue(options, nc.ConvertFromString(null, CultureInfo.InvariantCulture, value));
                }
            }
            // the FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException,
            // so we've catch directly Exception
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
                _field.SetValue(options, value);

                return true;
            }
        }

        private bool SetValueList(string value, object options)
        {
            lock (_setValueLock)
            {
                _field.SetValue(options, new List<string>());

                var fieldRef = (IList<string>)_field.GetValue(options);
                var values = value.Split(((OptionListAttribute)_attribute).Separator);

                for (int i = 0; i < values.Length; i++)
                {
                    fieldRef.Add(values[i]);
                }

                return true;
            }
        }

        public string ShortName
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
                    return string.Concat("--", _longName);

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
            get { return _field.FieldType == typeof(bool); }
        }

        public bool IsArray
        {
            get { return _field.FieldType.IsArray; }
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
    }
}