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
#region Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using CommandLine.Helpers;
#endregion

namespace CommandLine.Core
{ 
    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    sealed class OptionInfo
    {
        public OptionInfo(BaseOptionAttribute attribute, PropertyInfo property, CultureInfo parsingCulture)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute", SR.ArgumentNullException_AttributeCannotBeNull);
            }
            if (property == null)
            {
                throw new ArgumentNullException("property", SR.ArgumentNullException_PropertyCannotBeNull);
            }
            _required = attribute.Required;
            _shortName = attribute.ShortName;
            _longName = attribute.LongName;
            _mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            _defaultValue = attribute.DefaultValue;
            _hasDefaultValue = attribute.HasDefaultValue;
            _attribute = attribute;
            _property = property;
            _parsingCulture = parsingCulture;
            _propertyWriter = new PropertyWriter(_property, _parsingCulture);
        }

        /// <summary>
        /// Alternate constructor for testing purpose.
        /// </summary>
        internal OptionInfo(char? shortName, string longName)
        {
            _shortName = shortName;
            _longName = longName;
        }

        public bool SetValue(string value, object options)
        {
            if (_attribute is OptionListAttribute)
            {
                return SetValueList(value, options);
            }
            if (ReflectionUtil.IsNullableType(_property.PropertyType))
            {
                //return SetNullableValue(value, options);
                return _propertyWriter.WriteNullable(value, options);
            }
            //return SetValueScalar(value, options);
            return _propertyWriter.WriteScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            var elementType = _property.PropertyType.GetElementType();
            var array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array.SetValue(Convert.ChangeType(values[i], elementType, _parsingCulture), i);
                        _property.SetValue(options, array, null);
                }
                catch (FormatException)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SetValue(bool value, object options)
        {
            _property.SetValue(options, value, null);
            return true;
        }

        private bool SetValueList(string value, object options)
        {
            _property.SetValue(options, new List<string>(), null);
            var fieldRef = (IList<string>)_property.GetValue(options, null);
            var values = value.Split(((OptionListAttribute)_attribute).Separator);
            foreach (var item in values)
            {
                fieldRef.Add(item);
            }
            return true;
        }

        public void SetDefault(object options)
        {
            if (_hasDefaultValue)
            {
                try
                {
                    _property.SetValue(options, _defaultValue, null);
                }
                catch (Exception e)
                {
                    throw new ParserException("Bad default value.", e);
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

        public string MutuallyExclusiveSet
        {
            get { return _mutuallyExclusiveSet; }
        }

        public bool Required
        {
            get { return _required; }
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

        public bool HasParameterLessCtor { get; set; }

        public object GetValue(object target)
        {
            return _property.GetValue(target, null);
        }

        public void CreateInstance(object target)
        {
            try
            {
                _property.SetValue(target, Activator.CreateInstance(_property.PropertyType), null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }
        }

        private readonly CultureInfo _parsingCulture;
        private readonly BaseOptionAttribute _attribute;
        private readonly PropertyInfo _property;
        private readonly PropertyWriter _propertyWriter;
        private readonly bool _required;
        private readonly char? _shortName;
        private readonly string _longName;
        private readonly string _mutuallyExclusiveSet;
        private readonly object _defaultValue;
        private readonly bool _hasDefaultValue;
    }
}