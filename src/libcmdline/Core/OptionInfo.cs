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
        private readonly PropertyInfo _property;
        private readonly bool _required;
        private readonly string _helpText;
        private readonly string _shortName;
        private readonly string _longName;
        private readonly string _mutuallyExclusiveSet;
        private readonly object _setValueLock = new object();

        public OptionInfo(OptionAttribute attribute, PropertyInfo property)
        {
            if (attribute != null)
            {
                _required = attribute.Required;
                _helpText = attribute.HelpText;
                _shortName = attribute.ShortName;
                _longName = attribute.LongName;
                _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
                _attribute = attribute;
            }
            else
                throw new ArgumentNullException("attribute", "The attribute is mandatory");

            if (property != null)
                _property = property;
            else
                throw new ArgumentNullException("property", "The property is mandatory");
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
            var list = ReflectionUtil.RetrievePropertyList<OptionAttribute>(target);
            if (list != null)
            {
                var map = new OptionMap(list.Count, settings);

                foreach (var pair in list)
                {
                    if (pair != null && pair.Right != null) 
                        map[pair.Right.UniqueName] = new OptionInfo(pair.Right, pair.Left);
                }

                map.RawOptions = target;

                return map;
            }

            return null;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
                return SetValueList(value, options);

            if (ReflectionUtil.IsNullableType(_property.PropertyType))
                return SetNullableValue(value, options);

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
                        array.SetValue(Convert.ChangeType(values[i], elementType, CultureInfo.InvariantCulture), i);
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
                        _property.SetValue(options, Convert.ChangeType(value, _property.PropertyType, CultureInfo.InvariantCulture), null);
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
            var nc = new NullableConverter(_property.PropertyType);

            try
            {
                lock (_setValueLock)
                {
                    _property.SetValue(options, nc.ConvertFromString(null, CultureInfo.InvariantCulture, value), null);
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
    }
}