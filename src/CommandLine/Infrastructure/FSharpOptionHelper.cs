#if !SKIP_FSHARP
using System;
#if PLATFORM_DOTNET
using System.Reflection;
#endif
using CommandLine.Core;
using Microsoft.FSharp.Core;

namespace CommandLine.Infrastructure
{
    static class FSharpOptionHelper
    {
        public static Type GetUnderlyingType(Type type)
        {
            return type
#if NETSTANDARD1_5
                .GetTypeInfo()
#endif
                .GetGenericArguments()[0];
        }

        public static object Some(Type type, object value)
        {
            return typeof(FSharpOption<>)
                    .MakeGenericType(type)
                    .StaticMethod(
                        "Some", value);
        }

        public static object None(Type type)
        {
            return typeof(FSharpOption<>)
                    .MakeGenericType(type)
                    .StaticProperty(
                        "None");
        }

        public static object ValueOf(object value)
        {
            return typeof(FSharpOption<>)
                .MakeGenericType(GetUnderlyingType(value.GetType()))
                .InstanceProperty(
                    "Value", value);
        }

        public static bool IsSome(object value)
        {
            return (bool)typeof(FSharpOption<>)
                .MakeGenericType(GetUnderlyingType(value.GetType()))
                .StaticMethod(
                    "get_IsSome", value);
        }
    }
}
#endif
