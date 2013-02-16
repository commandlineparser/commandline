#region License
//
// Command Line Library: ValueMapper.cs
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using CommandLine.Helpers;
#endregion

namespace CommandLine.Core
{
    /// <summary>
    /// Maps unnamed options to property using <see cref="CommandLine.ValueOptionAttribute"/> and <see cref="CommandLine.ValueListAttribute"/>.
    /// </summary>
    sealed class ValueMapper
    {
        private ValueMapper() {}

        public ValueMapper(object target, CultureInfo parsingCulture)
        {
            _target = target;
            _parsingCulture = parsingCulture;
            InitializeValueList();
            InitializeValueOption();
        }

        private bool IsValueListDefined { get { return _valueListAttribute != null; } }

        private bool IsValueOptionDefined { get { return _valueOptionAttributeList.Count > 0; } }

        public bool CanReceiveValues { get { return IsValueListDefined || IsValueOptionDefined; } }

        private bool AddValueItem(string item)
        {
            if (_valueListAttribute.MaximumElements == 0 || _valueList.Count == _valueListAttribute.MaximumElements)
            {
                return false;
            }
            _valueList.Add(item);
            return true;
        }

        public bool MapValueItem(string item)
        {
            if (IsValueOptionDefined &&
                _valueOptionIndex < _valueOptionAttributeList.Count)
            {
                var valueOption = _valueOptionAttributeList[_valueOptionIndex++];
                var propertyWriter = new PropertyWriter(valueOption.Left, _parsingCulture);
                return ReflectionUtil.IsNullableType(propertyWriter.Property.PropertyType) ?
                    propertyWriter.WriteNullable(item, _target) :
                    propertyWriter.WriteScalar(item, _target);
            }
            return IsValueListDefined && AddValueItem(item);
        }

        private void InitializeValueList()
        {
            _valueListAttribute = ValueListAttribute.GetAttribute(_target);
            if (IsValueListDefined)
            {
                _valueList = ValueListAttribute.GetReference(_target);
            }
        }

        private void InitializeValueOption()
        {
            var list = ReflectionUtil.RetrievePropertyList<ValueOptionAttribute>(_target);

            // default is index 0, so skip sorting if all have it
            _valueOptionAttributeList = list.All(x => x.Right.Index == 0)
                ? list : list.OrderBy(x => x.Right.Index).ToList();
        }

        private readonly object _target;
        private IList<string> _valueList;
        private ValueListAttribute _valueListAttribute;
        private IList<Pair<PropertyInfo,ValueOptionAttribute>> _valueOptionAttributeList;
        private int _valueOptionIndex;
        private readonly CultureInfo _parsingCulture;
    }
}