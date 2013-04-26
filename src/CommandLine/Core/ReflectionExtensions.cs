// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommandLine.Core
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<T> GetSpecifications<T>(this System.Type type, Func<PropertyInfo, T> selector)
        {
            return from pi in type.GetProperties()
                   let attrs = pi.GetCustomAttributes(true)
                   where
                        attrs.OfType<OptionAttribute>().Any() ||
                        attrs.OfType<ValueAttribute>().Any()
                   select selector(pi);
        }

        public static DescriptorType ToDescriptor(this System.Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type == typeof(bool)
                       ? DescriptorType.Boolean
                       : type == typeof(string)
                             ? DescriptorType.Scalar
                             : type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)
                                   ? DescriptorType.Sequence
                                   : DescriptorType.Scalar;
        }

        public static bool IsScalar(this System.Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return type == typeof(string) || !type.IsArray && !typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static T SetProperties<T>(
            this T instance,
            IEnumerable<SpecificationProperty> specProps,
            Func<SpecificationProperty, bool> predicate,
            Func<SpecificationProperty, object> selector)
        {
            return specProps.Where(predicate)
                .Aggregate(
                    instance,
                    (current, specProp) =>
                        specProp.Property.SetValue(current, selector(specProp)));
        }

        private static T SetValue<T>(this PropertyInfo property, T instance, object value)
        {
            Action<Exception> fail = inner => { throw new ApplicationException("Cannot set value to target instance.", inner); };
            
            try
            {
                property.SetValue(instance, value, null);
            }
            catch (TargetException e)
            {
                fail(e);
            }
            catch (TargetParameterCountException e)
            {
                fail(e);
            }
            catch (MethodAccessException e)
            {
                fail(e);
            }
            catch (TargetInvocationException e)
            {
                fail(e);
            }

            return instance;
        }

        public static object CreateEmptyArray(this System.Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            return Array.CreateInstance(type, 0);
        }
    }
}