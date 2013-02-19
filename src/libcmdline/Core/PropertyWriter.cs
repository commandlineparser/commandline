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
    internal sealed class PropertyWriter
    {
        private readonly CultureInfo parsingCulture;

        public PropertyWriter(PropertyInfo property, CultureInfo parsingCulture)
        {
            this.parsingCulture = parsingCulture;
            this.Property = property;
        }

        public PropertyInfo Property { get; private set; }

        public bool WriteScalar(string value, object target)
        {
            try
            {
                this.Property.SetValue(
                    target,
                    this.Property.PropertyType.IsEnum ?
                        Enum.Parse(this.Property.PropertyType, value, true) :
                        Convert.ChangeType(value, this.Property.PropertyType, this.parsingCulture),
                    null);
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            return true;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException, so we've to catch directly System.Exception.")]
        public bool WriteNullable(string value, object target)
        {
            var nc = new NullableConverter(this.Property.PropertyType);
            try
            {
                this.Property.SetValue(target, nc.ConvertFromString(null, this.parsingCulture, value), null);
            }

            // FormatException (thrown by ConvertFromString) is thrown as Exception.InnerException, so we've to catch directly System.Exception
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
