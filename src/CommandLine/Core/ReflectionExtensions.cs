// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using CommandLine.Infrastructure;
using CommandLine.Text;
using CSharpx;

namespace CommandLine.Core
{
    internal static class ReflectionExtensions
    {
#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing type annotation", "IL2070")]
#endif
        public static IEnumerable<T> GetSpecifications<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type,
            Func<PropertyInfo, T> selector)
        {
            return from pi in type.FlattenHierarchy().Select(x => x.GetTypeInfo()).SelectMany(x => x.GetProperties())
                   let attrs = pi.GetCustomAttributes(true)
                   where
                       attrs.OfType<OptionAttribute>().Any() ||
                       attrs.OfType<ValueAttribute>().Any()
                   group pi by pi.Name
                   into g
                   select selector(g.First());
        }

        public static Maybe<VerbAttribute> GetVerbSpecification(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type)
        {
            return
                (from attr in
                     type.FlattenHierarchy()
                         .SelectMany(x => x.GetTypeInfo().GetCustomAttributes(typeof(VerbAttribute), true))
                 let vattr = (VerbAttribute)attr
                 select vattr)
                .SingleOrDefault()
                .ToMaybe();
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Accessed via reflection", "IL2070")]
#endif
        public static Maybe<Tuple<PropertyInfo, UsageAttribute>> GetUsageData(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type)
        {
            return
                (from pi in type.FlattenHierarchy().SelectMany(x => x.GetTypeInfo().GetProperties())
                    let attrs = pi.GetCustomAttributes(typeof(UsageAttribute), true)
                    where attrs.Any()
                    select Tuple.Create(pi, (UsageAttribute)attrs.First()))
                        .SingleOrDefault()
                        .ToMaybe();
        }

        private static IEnumerable<Type> FlattenHierarchy(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type)
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

        private static Type[] SafeGetInterfaces(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type)
        {
            return type.GetTypeInfo().GetInterfaces();
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
                return new[]
                    { new SetValueExceptionError(specProp.Specification.FromSpecification(), e.InnerException, value) };
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

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing type annotation", "IL2067")]
#endif
        internal static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Requires reflection on members", "IL2075")]
#endif
        public static bool IsMutable(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                DynamicallyAccessedMemberTypes.PublicMethods |
                DynamicallyAccessedMemberTypes.Interfaces)]
#endif
            this Type type)
        {
            if (type == typeof(object))
                return true;

            // Find all inherited defined properties and fields on the type
            var inheritedTypes = type.GetTypeInfo().FlattenHierarchy().Select(i => i.GetTypeInfo());

            foreach (var inheritedType in inheritedTypes)
            {
                if (
                    inheritedType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Any(p => p.CanWrite) ||
                    inheritedType.GetTypeInfo().GetFields(BindingFlags.Public | BindingFlags.Instance).Any()
                )
                {
                    return true;
                }
            }

            return false;
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing type annotation", "IL2070")]
#endif
        public static object CreateDefaultForImmutable(
            this Type type)
        {
            if (type.GetTypeInfo().IsGenericType &&
                type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetTypeInfo().GetGenericArguments()[0].CreateEmptyArray();
            }

            return type.GetDefaultValue();
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing type annotation", "IL2067")]
#endif
        public static object AutoDefault(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
            this Type type)
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

        public static object StaticMethod(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            this Type type,
            string name,
            params object[] args)
        {
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                args);
        }

        public static object StaticProperty(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            this Type type,
            string name)
        {
            return type.GetTypeInfo().InvokeMember(
                name,
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new object[] { });
        }

        public static object InstanceProperty(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
            this Type type,
            string name,
            object target)
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
             || new[]
                {
                    typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeSpan)
                }.Contains(type)
             || Convert.GetTypeCode(type) != TypeCode.Object;
        }

        public static bool IsCustomStruct(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
            this Type type)
        {
            var isStruct = type.GetTypeInfo().IsValueType && !type.GetTypeInfo().IsPrimitive &&
                !type.GetTypeInfo().IsEnum && type != typeof(Guid);
            if (!isStruct) return false;
            var ctor = type.GetTypeInfo().GetConstructor(new[] { typeof(string) });
            return ctor != null;
        }
    }
}
