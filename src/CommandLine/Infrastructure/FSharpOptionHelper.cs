using System;
using System.Reflection;
using Microsoft.FSharp.Core;

namespace CommandLine.Infrastructure
{
    class FSharpOptionHelper
    {
        public static Type GetUnderlyingType(Type type)
        {
            return type.GetGenericArguments()[0];
        }

        public static object Some(Type type, object value)
        {
            var optionType = typeof(FSharpOption<>);
            var typedType = optionType.MakeGenericType(type);

            return typedType.InvokeMember(
                "Some",
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new [] { value });
        }

        public static object None(Type type)
        {
            var optionType = typeof(FSharpOption<>);
            var typedType = optionType.MakeGenericType(type);

            return typedType.InvokeMember(
                "None",
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new object[] {});
        }
    }
}
