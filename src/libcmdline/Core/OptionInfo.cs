#region License
// <copyright file="OptionInfo.cs" company="Giacomo Stelluti Scala">
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using CommandLine.Helpers;
    #endregion

    [DebuggerDisplay("ShortName = {ShortName}, LongName = {LongName}")]
    internal sealed class OptionInfo
    {
        private readonly CultureInfo parsingCulture;
        private readonly BaseOptionAttribute attribute;
        private readonly PropertyInfo property;
        private readonly PropertyWriter propertyWriter;
        private readonly bool required;
        private readonly char? shortName;
        private readonly string longName;
        private readonly string mutuallyExclusiveSet;
        private readonly object defaultValue;
        private readonly bool hasDefaultValue;

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

            this.required = attribute.Required;
            this.shortName = attribute.ShortName;
            this.longName = attribute.LongName;
            this.mutuallyExclusiveSet = attribute.MutuallyExclusiveSet;
            this.defaultValue = attribute.DefaultValue;
            this.hasDefaultValue = attribute.HasDefaultValue;
            this.attribute = attribute;
            this.property = property;
            this.parsingCulture = parsingCulture;
            this.propertyWriter = new PropertyWriter(this.property, this.parsingCulture);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionInfo"/> class. Used for unit testing purpose.
        /// </summary>
        /// <param name="shortName">Option short name.</param>
        /// <param name="longName">Option long name.</param>
        internal OptionInfo(char? shortName, string longName)
        {
            this.shortName = shortName;
            this.longName = longName;
        }

        public char? ShortName
        {
            get { return this.shortName; }
        }

        public string LongName
        {
            get { return this.longName; }
        }

        public string MutuallyExclusiveSet
        {
            get { return this.mutuallyExclusiveSet; }
        }

        public bool Required
        {
            get { return this.required; }
        }

        public bool IsBoolean
        {
            get { return this.property.PropertyType == typeof(bool); }
        }

        public bool IsArray
        {
            get { return this.property.PropertyType.IsArray; }
        }

        public bool IsAttributeArrayCompatible
        {
            get { return this.attribute is OptionArrayAttribute; }
        }

        public bool IsDefined
        {
            get; set;
        }

        public bool ReceivedValue
        {
            get; private set;
        }

        public bool HasBothNames
        {
            get
            {
                return this.shortName != null && this.longName != null;
            }
        }

        public bool HasParameterLessCtor
        {
            get; set;
        }

        public object GetValue(object target)
        {
            return this.property.GetValue(target, null);
        }

        public void CreateInstance(object target)
        {
            try
            {
                this.property.SetValue(target, Activator.CreateInstance(this.property.PropertyType), null);
            }
            catch (Exception e)
            {
                throw new ParserException(SR.CommandLineParserException_CannotCreateInstanceForVerbCommand, e);
            }
        }

        public bool SetValue(string value, object options)
        {
            if (this.attribute is OptionListAttribute)
            {
                return this.SetValueList(value, options);
            }

            if (ReflectionUtil.IsNullableType(this.property.PropertyType))
            {
                return this.ReceivedValue = this.propertyWriter.WriteNullable(value, options);
            }

            return this.ReceivedValue = this.propertyWriter.WriteScalar(value, options);
        }

        public bool SetValue(IList<string> values, object options)
        {
            var elementType = this.property.PropertyType.GetElementType();
            var array = Array.CreateInstance(elementType, values.Count);

            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array.SetValue(Convert.ChangeType(values[i], elementType, this.parsingCulture), i);
                    this.property.SetValue(options, array, null);
                }
                catch (FormatException)
                {
                    return false;
                }
            }

            return this.ReceivedValue = true;
        }

        public bool SetValue(bool value, object options)
        {
            this.property.SetValue(options, value, null);
            return this.ReceivedValue = true;
        }

        public void SetDefault(object options)
        {
            if (this.hasDefaultValue)
            {
                try
                {
                    this.property.SetValue(options, this.defaultValue, null);
                }
                catch (Exception e)
                {
                    throw new ParserException("Bad default value.", e);
                }
            }
        }

        private bool SetValueList(string value, object options)
        {
            this.property.SetValue(options, new List<string>(), null);
            var fieldRef = (IList<string>)this.property.GetValue(options, null);
            var values = value.Split(((OptionListAttribute)this.attribute).Separator);
            foreach (var item in values)
            {
                fieldRef.Add(item);
            }

            return this.ReceivedValue = true;
        }
    }
}