// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CommandLine.Infrastructure;

namespace CommandLine.Core
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<T> GetSpecifications<T>(this Type type, Func<PropertyInfo, T> selector)
        {
            return from pi in type.GetProperties()
                   let attrs = pi.GetCustomAttributes(true)
                   where
                        attrs.OfType<OptionAttribute>().Any() ||
                        attrs.OfType<ValueAttribute>().Any()
                   select selector(pi);
        }

        public static TargetType ToTargetType(this Type type)
        {
            return type == typeof(bool)
                       ? TargetType.Switch
                       : type == typeof(string)
                             ? TargetType.Scalar
                             : type.IsArray || typeof(IEnumerable).IsAssignableFrom(type)
                                   ? TargetType.Sequence
                                   : TargetType.Scalar;
        }

        public static T SetProperties<T>(
            this T instance,
            IEnumerable<SpecificationProperty> specProps,
            Func<SpecificationProperty, bool> predicate,
            Func<SpecificationProperty, object> selector)
        {
            return specProps.Where(predicate).Aggregate(
                instance,
                (current, specProp) =>
                    {
                        specProp.Property.SetValue(current, selector(specProp));
                        return instance;
                    });
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

        public static object CreateEmptyArray(this Type type)
        {
            return Array.CreateInstance(type, 0);
        }

        public static object GetDefaultValue(this Type type)
        {
            var e = Expression.Lambda<Func<object>>(
                Expression.Convert(
                    Expression.Default(type),
                    typeof(object)));
            return e.Compile()();
        }

        public static bool IsMutable(this Type type)
        {
            return ReflectionHelper.IsTypeMutable(type);
        }

        public static object CreateDefaultForImmutable(this Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0].CreateEmptyArray();
            }
            return type.GetDefaultValue();
        }
    }
}