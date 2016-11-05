// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine.Core;
using CSharpx;

namespace CommandLine.Infrastructure
{
    static class ReflectionHelper
    {
        /// <summary>
        /// Per thread assembly attribute overrides for testing.
        /// </summary>
        [ThreadStatic] private static IDictionary<Type, Attribute> _overrides;

        /// <summary>
        /// Assembly attribute overrides for testing.
        /// </summary>
        /// <remarks>
        /// The implementation will fail if two or more attributes of the same type
        /// are included in <paramref name="overrides"/>.
        /// </remarks>
        /// <param name="overrides">
        /// Attributes that replace the existing assembly attributes or null,
        /// to clear any testing attributes.
        /// </param>
        public static void SetAttributeOverride(IEnumerable<Attribute> overrides)
        {
            if (overrides != null)
            {
                _overrides = overrides.ToDictionary(attr => attr.GetType(), attr => attr);
            }
            else
            {
                _overrides = null;
            }
        }

        public static Maybe<TAttribute> GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            // Test support
            if (_overrides != null)
            {
                return 
                    _overrides.ContainsKey(typeof(TAttribute)) ?
                        Maybe.Just((TAttribute)_overrides[typeof(TAttribute)]) :
                        Maybe.Nothing< TAttribute>();
            }

            var assembly = GetExecutingOrEntryAssembly();

#if NET40
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);
#else
            var attributes = assembly.GetCustomAttributes<TAttribute>().ToArray();
#endif

            return attributes.Length > 0
                ? Maybe.Just((TAttribute)attributes[0])
                : Maybe.Nothing<TAttribute>();
        }

        public static string GetAssemblyName()
        {
            var assembly = GetExecutingOrEntryAssembly();
            return assembly.GetName().Name;
        }

        public static string GetAssemblyVersion()
        {
            var assembly = GetExecutingOrEntryAssembly();
            return assembly.GetName().Version.ToStringInvariant();
        }

        public static bool IsFSharpOptionType(Type type)
        {
            return type.FullName.StartsWith(
                "Microsoft.FSharp.Core.FSharpOption`1", StringComparison.Ordinal);
        }

        public static T CreateDefaultImmutableInstance<T>(Type[] constructorTypes)
        {
            var t = typeof(T);
            var ctor = t.GetTypeInfo().GetConstructor(constructorTypes);
            var values = (from prms in ctor.GetParameters()
                          select prms.ParameterType.CreateDefaultForImmutable()).ToArray();
            return (T)ctor.Invoke(values);
        }

        public static object CreateDefaultImmutableInstance(Type type, Type[] constructorTypes)
        {
            var ctor = type.GetTypeInfo().GetConstructor(constructorTypes);
            var values = (from prms in ctor.GetParameters()
                          select prms.ParameterType.CreateDefaultForImmutable()).ToArray();
            return ctor.Invoke(values);
        }

        private static Assembly GetExecutingOrEntryAssembly()
        {
            var assembly = Assembly.GetEntryAssembly();

#if !NETSTANDARD1_5
            assembly = assembly ?? Assembly.GetExecutingAssembly();
#endif

            return assembly;
        }
    }
}