// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System;
using System.Reflection;

namespace CommandLine.Infrastructure
{
    internal static class ReflectionHelper
    {
        public static Maybe<TAttribute> GetAttribute<TAttribute>()
            where TAttribute : Attribute
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);

            return attributes.Length > 0
                ? Maybe.Just((TAttribute)attributes[0])
                : Maybe.Nothing<TAttribute>();
        }

        public static string GetAssemblyName()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetName().Name;
        }

        public static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetName().Version.ToStringInvariant();
        }

        public static bool IsFSharpOptionType(Type type)
        {
            return type.FullName.StartsWith(
                "Microsoft.FSharp.Core.FSharpOption`1", StringComparison.Ordinal);
        }
    }
}