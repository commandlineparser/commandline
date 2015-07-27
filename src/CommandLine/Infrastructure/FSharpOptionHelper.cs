using System;
using System.Reflection;

using CommandLine.Core;

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

            return typedType.StaticMethod(
                "Some", value);
        }

        public static object None(Type type)
        {
            var optionType = typeof(FSharpOption<>);
            var typedType = optionType.MakeGenericType(type);

            return typedType.StaticProperty(
                "None");
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

            return (bool)typedType.StaticMethod(
                "get_IsSome", value);
        }
    }
}
