// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Text;
using CSharpx;

namespace CommandLine.Core
{
    static class ReflectionExtensions
    {
        public static IEnumerable<T> GetSpecifications<T>(this Type type, Func<PropertyInfo, T> selector)
        {
            return from pi in type.FlattenHierarchy().SelectMany(x => x.GetTypeInfo().GetProperties())
                   let attrs = pi.GetCustomAttributes(true)
                   where
                       attrs.OfType<OptionAttribute>().Any() ||
                       attrs.OfType<ValueAttribute>().Any()
                   group pi by pi.Name into g
                   select selector(g.First());
        }

        public static Maybe<VerbAttribute> GetVerbSpecification(this Type type)
        {
            return
                (from attr in
                 type.FlattenHierarchy().SelectMany(x => x.GetTypeInfo().GetCustomAttributes(typeof(VerbAttribute), true))
                 let vattr = (VerbAttribute)attr
                 select vattr)
                    .SingleOrDefault()
                    .ToMaybe();
        }

        public static Maybe<Tuple<PropertyInfo, UsageAttribute>> GetUsageData(this Type type)
        {
            return
                (from pi in type.FlattenHierarchy().SelectMany(x => x.GetTypeInfo().GetProperties())
                    let attrs = pi.GetCustomAttributes(true)
                    where attrs.OfType<UsageAttribute>().Any()
                    select Tuple.Create(pi, (UsageAttribute)attrs.First()))
                        .SingleOrDefault()
                        .ToMaybe();
        }

        private static IEnumerable<Type> FlattenHierarchy(this Type type)
        {
            if (type == null)
            {
                yield break;
            }
            yield return type;
            foreach (var @interface in type.SafeGetInterfaces())
            {
                yield return @interface;
            }
            foreach (var @interface in FlattenHierarchy(type.GetTypeInfo().BaseType))
            {
                yield return @interface;
            }
        }

        private static IEnumerable<Type> SafeGetInterfaces(this Type type)
        {
            return type == null ? Enumerable.Empty<Type>() : type.GetTypeInfo().GetInterfaces();
        }

        public static TargetType ToTargetType(this Type type)
        {
            return type == typeof(bool)
                       ? TargetType.Switch
                       : type == typeof(string)
                             ? TargetType.Scalar
                             : type.IsArray || typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(type)
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
            Action<Exception> fail = inner => {
                throw new InvalidOperationException("Cannot set value to target instance.", inner);
            };
            
            try
            {
                property.SetValue(instance, value, null);
            }
#if !PLATFORM_DOTNET
            catch (TargetException e)
            {
                fail(e);
            }
#endif
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
            Func<bool> isMutable = () => {
                var props = type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.CanWrite);
                var fields = type.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance).Any();
                return props || fields;
            };
            return type != typeof(object) ? isMutable() : true;
        }

        public static object CreateDefaultForImmutable(this Type type)
        {
            if (type == typeof(string))
            {
                return string.Empty;
            }
            if (type.GetTypeInfo().IsGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetTypeInfo().GetGenericArguments()[0].CreateEmptyArray();
            }
            return type.GetDefaultValue();
        }

        public static object AutoDefault(this Type type)
        {
            if (type.IsMutable())
            {
                return Activator.CreateInstance(type);
            }

            var ctorTypes = type.GetSpecifications(pi => pi.PropertyType).ToArray();
 
            return ReflectionHelper.CreateDefaultImmutableInstance(type, ctorTypes);
        }

        public static TypeInfo ToTypeInfo(this Type type)
        {
            return TypeInfo.Create(type);
        }

        public static object StaticMethod(this Type type, string name, params object[] args)
        {
#if NETSTANDARD1_5
            MethodInfo method = type.GetTypeInfo().GetDeclaredMethod(name);
            return method.Invoke(null, args);
#else
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                args);
#endif
        }

        public static object StaticProperty(this Type type, string name)
        {
#if NETSTANDARD1_5
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(name);
            return property.GetValue(null);
#else
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new object[] { });
#endif
        }

        public static object InstanceProperty(this Type type, string name, object target)
        {
#if NETSTANDARD1_5
            PropertyInfo property = type.GetTypeInfo().GetDeclaredProperty(name);
            return property.GetValue(target);
#else
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                null,
                target,
                new object[] { });
#endif
        }

        public static bool IsPrimitiveEx(this Type type)
        {
            return
                   (type.GetTypeInfo().IsValueType && type != typeof(Guid))
                || type.GetTypeInfo().IsPrimitive
                || new [] { 
                     typeof(string)
                    ,typeof(decimal)
                    ,typeof(DateTime)
                    ,typeof(DateTimeOffset)
                    ,typeof(TimeSpan)
                   }.Contains(type)
                || Convert.GetTypeCode(type) != TypeCode.Object;
        }


#if NET40
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#else
        public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).OfType<Attribute>().ToArray();
        }

        public static Attribute[] GetCustomAttributes(this Assembly assembly, Type attributeType, bool inherit)
        {
            return assembly.GetCustomAttributes(attributeType).ToArray();
        }
#endif
    }
}