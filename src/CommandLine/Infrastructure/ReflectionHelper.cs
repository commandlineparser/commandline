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

        private static Assembly _programAssembly;

        public static Assembly ProgramAssembly
        {
            get => _programAssembly ?? GetExecutingOrEntryAssembly();
            set => _programAssembly = value;
        }

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
                        Maybe.Nothing<TAttribute>();
            }

#if NET40
            var attributes = ProgramAssembly.GetCustomAttributes(typeof(TAttribute), false);
#else
            var attributes = ProgramAssembly.GetCustomAttributes<TAttribute>().ToArray();
#endif

            return attributes.Length > 0
                ? Maybe.Just((TAttribute)attributes[0])
                : Maybe.Nothing<TAttribute>();
        }

        public static string GetAssemblyName()
        {
            return ProgramAssembly.GetName().Name;
        }

        public static string GetAssemblyVersion()
        {
            return ProgramAssembly.GetName().Version.ToStringInvariant();
        }

        public static bool IsFSharpOptionType(Type type)
        {
            return type.FullName.StartsWith(
                "Microsoft.FSharp.Core.FSharpOption`1", StringComparison.Ordinal);
        }

        public static T CreateDefaultImmutableInstance<T>(Type[] constructorTypes)
        {
            var t = typeof(T);
            return (T)CreateDefaultImmutableInstance(t, constructorTypes);
        }

        public static object CreateDefaultImmutableInstance(Type type, Type[] constructorTypes)
        {
            var ctor = type.GetTypeInfo().GetConstructor(constructorTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException($"Type {type.FullName} appears to be immutable, but no constructor found to accept values.");
            }

            var values = (from prms in ctor.GetParameters()
                          select prms.ParameterType.CreateDefaultForImmutable()).ToArray();
            return ctor.Invoke(values);
        }

        private static Assembly GetExecutingOrEntryAssembly()
        {
            //resolve issues of null EntryAssembly in Xunit Test #392,424,389
            //return Assembly.GetEntryAssembly();
            return Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        }

       public static IEnumerable<string> GetNamesOfEnum(Type t)
        {
            if (t.IsEnum)
                return Enum.GetNames(t);
            Type u = Nullable.GetUnderlyingType(t);
            if (u != null && u.IsEnum)
                return Enum.GetNames(u);
            return Enumerable.Empty<string>();
        }
    }
}
