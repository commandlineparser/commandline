#region License
// <copyright file="ValueListAttribute.cs" company="Giacomo Stelluti Scala">
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
#region Using Directives
using System;
using System.Collections.Generic;
using System.Reflection;

using CommandLine.Infrastructure;
#endregion

namespace CommandLine
{
    /// <summary>
    /// Models a list of command line arguments that are not options.
    /// Must be applied to a field compatible with an <see cref="System.Collections.Generic.IList&lt;T&gt;"/> interface
    /// of <see cref="System.String"/> instances.
    /// </summary>
    /// <remarks>To map individual values use instead <see cref="CommandLine.ValueOptionAttribute"/>.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ValueListAttribute : Attribute
    {
        private readonly Type _concreteType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLine.ValueListAttribute"/> class.
        /// </summary>
        /// <param name="concreteType">A type that implements <see cref="System.Collections.Generic.IList&lt;T&gt;"/>.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="concreteType"/> is null.</exception>
        public ValueListAttribute(Type concreteType)
            : this()
        {
            if (concreteType == null)
            {
                throw new ArgumentNullException("concreteType");
            }

            if (!typeof(IList<string>).IsAssignableFrom(concreteType))
            {
                throw new ParserException(SR.CommandLineParserException_IncompatibleTypes);
            }

            _concreteType = concreteType;
        }

        private ValueListAttribute()
        {
            MaximumElements = -1;
        }

        /// <summary>
        /// Gets or sets the maximum element allow for the list managed by <see cref="CommandLine.ValueListAttribute"/> type.
        /// If lesser than 0, no upper bound is fixed.
        /// If equal to 0, no elements are allowed.
        /// </summary>
        public int MaximumElements { get; set; }

        /// <summary>
        /// Gets the concrete type specified during initialization.
        /// </summary>
        public Type ConcreteType
        {
            get { return _concreteType; }
        }

        internal static IList<string> GetReference(object target)
        {
            Type concreteType;
            var property = GetProperty(target, out concreteType);
            if (property == null || concreteType == null)
            {
                return null;
            }

            property.SetValue(target, Activator.CreateInstance(concreteType), null);

            return (IList<string>)property.GetValue(target, null);
        }

        internal static ValueListAttribute GetAttribute(object target)
        {
            var list = ReflectionHelper.RetrievePropertyList<ValueListAttribute>(target);
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (list.Count > 1)
            {
                throw new InvalidOperationException();
            }

            var pairZero = list[0];
            return pairZero.Right;
        }

        private static PropertyInfo GetProperty(object target, out Type concreteType)
        {
            concreteType = null;
            var list = ReflectionHelper.RetrievePropertyList<ValueListAttribute>(target);
            if (list == null || list.Count == 0)
            {
                return null;
            }

            if (list.Count > 1)
            {
                throw new InvalidOperationException();
            }

            var pairZero = list[0];
            concreteType = pairZero.Right.ConcreteType;
            return pairZero.Left;
        }
    }
}