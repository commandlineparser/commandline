#region License
//
// Command Line Library: PropertyWriter.cs
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;
#endregion

namespace CommandLine.Core
{
    /// <summary>
    /// Encapsulates property writing primitives.
    /// </summary>
    sealed class PropertyWriter
    {
        public PropertyWriter(PropertyInfo property, CultureInfo parsingCulture)
        {
            _parsingCulture = parsingCulture;
            Property = property;
        }

        public PropertyInfo Property { get; private set; }

        public bool WriteScalar(string value, object target)
        {
            try
            {
                Property.SetValue(target, Property.PropertyType.IsEnum ?
                    Enum.Parse(Property.PropertyType, value, true) :
                        Convert.ChangeType(value, Property.PropertyType,
                            _parsingCulture), null);
            }
            catch (InvalidCastException) { return false; } // Convert.ChangeType
            catch (FormatException) { return false; } // Convert.ChangeType
            catch (ArgumentException) { return false; } // Enum.Parse
            catch (OverflowException) { return false; } // Convert.ChangeType
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException, so we've to catch directly System.Exception.")]
        public bool WriteNullable(string value, object target)
        {
            var nc = new NullableConverter(Property.PropertyType);
            try
            {
                // ReSharper disable AssignNullToNotNullAttribute
                Property.SetValue(target, nc.ConvertFromString(null, _parsingCulture, value), null);
                // ReSharper restore AssignNullToNotNullAttribute
            }
            // FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException,
            // so we've to catch directly System.Exception
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private readonly CultureInfo _parsingCulture;
    }
}
