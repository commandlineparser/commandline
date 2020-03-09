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

        public static IEnumerable<Error> SetProperties<T>(
            this T instance,
            IEnumerable<SpecificationProperty> specProps,
            Func<SpecificationProperty, bool> predicate,
            Func<SpecificationProperty, object> selector)
        {
            return specProps.Where(predicate).SelectMany(specProp => specProp.SetValue(instance, selector(specProp)));
        }

        private static IEnumerable<Error> SetValue<T>(this SpecificationProperty specProp, T instance, object value)
        {
            try
            {
                specProp.Property.SetValue(instance, value, null);
                return Enumerable.Empty<Error>();
            }
            catch (TargetInvocationException e)
            {
                return new[] { new SetValueExceptionError(specProp.Specification.FromSpecification(), e.InnerException, value) };
            }
            catch (ArgumentException e)
            {
                var argEx = new ArgumentException(InvalidAttributeConfigurationError.ErrorMessage, e);

                return new[] { new SetValueExceptionError(specProp.Specification.FromSpecification(), argEx, value) };
            }

            catch (Exception e)
            {
                return new[] { new SetValueExceptionError(specProp.Specification.FromSpecification(), e, value) };
            }

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
            if(type == typeof(object))
                return true;

            // Find all inherited defined properties and fields on the type
            var inheritedTypes = type.GetTypeInfo().FlattenHierarchy().Select(i => i.GetTypeInfo());

            foreach (var inheritedType in inheritedTypes) 
            {
                if (
                    inheritedType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(p => p.CanWrite) ||
                    inheritedType.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance).Any()
                    )
                {
                    return true;
                }
            }

            return false;
        }

        public static object CreateDefaultForImmutable(this Type type)
        {
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
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                args);
        }

        public static object StaticProperty(this Type type, string name)
        {
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new object[] { });
        }

        public static object InstanceProperty(this Type type, string name, object target)
        {
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                null,
                target,
                new object[] { });
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

        public static bool IsCustomStruct(this Type type)
        {
            var isStruct = type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsPrimitive && !type.GetTypeInfo().IsEnum &&  type != typeof(Guid);
            if (!isStruct) return false;
            var ctor = type.GetTypeInfo().GetConstructor(new[] { typeof(string) });
            return ctor != null;
        }
    }
}
