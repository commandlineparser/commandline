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

        public static object ValueOf(object value)
        {
            var optionType = typeof(FSharpOption<>);
            var typedType = optionType.MakeGenericType(GetUnderlyingType(value.GetType()));

            return typedType.InvokeMember(
                "Value",
                BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance,
                null,
                value,
                new object[] { });
        }

        public static bool IsSome(object value)
        {
            var optionType = typeof(FSharpOption<>);
            var typedType = optionType.MakeGenericType(GetUnderlyingType(value.GetType()));

            return (bool)typedType.InvokeMember(
                "get_IsSome",
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                null,
                null,
                new [] { value });
        }
    }
}
