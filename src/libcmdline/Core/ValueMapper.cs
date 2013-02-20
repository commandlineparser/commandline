#region License
// <copyright file="ValueMapper.cs" company="Giacomo Stelluti Scala">
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

namespace CommandLine.Core
{
    #region Using Directives
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using CommandLine.Helpers;
    #endregion

    /// <summary>
    /// Maps unnamed options to property using <see cref="CommandLine.ValueOptionAttribute"/> and <see cref="CommandLine.ValueListAttribute"/>.
    /// </summary>
    internal sealed class ValueMapper
    {
        private readonly CultureInfo parsingCulture;
        private readonly object target;
        private IList<string> valueList;
        private ValueListAttribute valueListAttribute;
        private IList<Pair<PropertyInfo, ValueOptionAttribute>> valueOptionAttributeList;
        private int valueOptionIndex;

        public ValueMapper(object target, CultureInfo parsingCulture)
        {
            this.target = target;
            this.parsingCulture = parsingCulture;
            this.InitializeValueList();
            this.InitializeValueOption();
        }

        public bool CanReceiveValues
        {
            get { return this.IsValueListDefined || this.IsValueOptionDefined; }
        }

        private bool IsValueListDefined
        {
            get { return this.valueListAttribute != null; }
        }

        private bool IsValueOptionDefined
        {
            get { return this.valueOptionAttributeList.Count > 0; }
        }

        public bool MapValueItem(string item)
        {
            if (this.IsValueOptionDefined &&
                this.valueOptionIndex < this.valueOptionAttributeList.Count)
            {
                var valueOption = this.valueOptionAttributeList[this.valueOptionIndex++];
                var propertyWriter = new PropertyWriter(valueOption.Left, this.parsingCulture);
                return ReflectionUtil.IsNullableType(propertyWriter.Property.PropertyType) ?
                    propertyWriter.WriteNullable(item, this.target) :
                    propertyWriter.WriteScalar(item, this.target);
            }

            return this.IsValueListDefined && this.AddValueItem(item);
        }

        private bool AddValueItem(string item)
        {
            if (this.valueListAttribute.MaximumElements == 0 ||
                this.valueList.Count == this.valueListAttribute.MaximumElements)
            {
                return false;
            }

            this.valueList.Add(item);
            return true;
        }

        private void InitializeValueList()
        {
            this.valueListAttribute = ValueListAttribute.GetAttribute(this.target);
            if (this.IsValueListDefined)
            {
                this.valueList = ValueListAttribute.GetReference(this.target);
            }
        }

        private void InitializeValueOption()
        {
            var list = ReflectionUtil.RetrievePropertyList<ValueOptionAttribute>(this.target);

            // default is index 0, so skip sorting if all have it
            this.valueOptionAttributeList = list.All(x => x.Right.Index == 0)
                ? list : list.OrderBy(x => x.Right.Index).ToList();
        }
    }
}