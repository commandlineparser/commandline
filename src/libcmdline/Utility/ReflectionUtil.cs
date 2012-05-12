#region License
//
// Command Line Library: ReflectionUtil.cs
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
using System.Reflection;
#endregion

namespace CommandLine
{
    static class ReflectionUtil
    {
        public static IList<Pair<PropertyInfo, TAttribute>> RetrievePropertyList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            IList<Pair<PropertyInfo, TAttribute>> list = new List<Pair<PropertyInfo, TAttribute>>();
            if (target != null)
            {
                var propertiesInfo = target.GetType().GetProperties();

                foreach (var property in propertiesInfo)
                {
                    if (property != null && (property.CanRead && property.CanWrite))
                    {
                        var setMethod = property.GetSetMethod();
                        if (setMethod != null && !setMethod.IsStatic)
                        {
                            var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                            if (attribute != null)
                                list.Add(new Pair<PropertyInfo, TAttribute>(property, (TAttribute)attribute));
                        }
                    }
                }
            }
            
            return list;
        }

        public static Pair<MethodInfo, TAttribute> RetrieveMethod<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var info = target.GetType().GetMethods();

            foreach (MethodInfo method in info)
            {
                if (!method.IsStatic)
                {
                    Attribute attribute =
                        Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                    if (attribute != null)
                        return new Pair<MethodInfo, TAttribute>(method, (TAttribute)attribute);
                }
            }

            return null;
        }

        public static TAttribute RetrieveMethodAttributeOnly<TAttribute>(object target)
                where TAttribute : Attribute
        {
            var info = target.GetType().GetMethods();

            foreach (MethodInfo method in info)
            {
                if (!method.IsStatic)
                {
                    Attribute attribute =
                        Attribute.GetCustomAttribute(method, typeof(TAttribute), false);
                    if (attribute != null)
                        return (TAttribute)attribute;
                }
            }

            return null;
        }

        public static IList<TAttribute> RetrievePropertyAttributeList<TAttribute>(object target)
                where TAttribute : Attribute
        {
            IList<TAttribute> list = new List<TAttribute>();
            var info = target.GetType().GetProperties();

            foreach (var property in info)
            {
                if (property != null && (property.CanRead && property.CanWrite))
                {
                    var setMethod = property.GetSetMethod();
                    if (setMethod != null && !setMethod.IsStatic)
                    {
                        var attribute = Attribute.GetCustomAttribute(property, typeof(TAttribute), false);
                        if (attribute != null)
                            list.Add((TAttribute)attribute);
                    }
                }
            }

            return list;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}