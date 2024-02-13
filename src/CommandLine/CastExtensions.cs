using System;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Linq;
using System.Reflection;

namespace CommandLine
{
    internal static class CastExtensions
    {
        private const string ImplicitCastMethodName = "op_Implicit";
        private const string ExplicitCastMethodName = "op_Explicit";

        public static bool CanCast<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            this Type baseType)
        {
            return baseType.CanImplicitCast<T>() || baseType.CanExplicitCast<T>();
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on type", "IL2072")]
#endif
        public static bool CanCast<T>(this object obj)
        {
            var objType = obj.GetType();
            return objType.CanCast<T>();
        }

        public static T Cast<T>(this object obj)
        {
            try
            {
                return (T)obj;
            }
            catch (InvalidCastException)
            {
                if (obj.CanImplicitCast<T>())
                    return obj.ImplicitCast<T>();
                if (obj.CanExplicitCast<T>())
                    return obj.ExplicitCast<T>();
                else
                    throw;
            }
        }

        private static bool CanImplicitCast<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            this Type baseType)
        {
            return baseType.CanCast<T>(ImplicitCastMethodName);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on type", "IL2072")]
#endif
        private static bool CanImplicitCast<T>(this object obj)
        {
            var baseType = obj.GetType();
            return baseType.CanImplicitCast<T>();
        }

        private static bool CanExplicitCast<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            this Type baseType)
        {
            return baseType.CanCast<T>(ExplicitCastMethodName);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Missing annotations on type", "IL2072")]
#endif
        private static bool CanExplicitCast<T>(this object obj)
        {
            var baseType = obj.GetType();
            return baseType.CanExplicitCast<T>();
        }

        private static bool CanCast<T>(
#if NET8_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            this Type baseType,
            string castMethodName)
        {
            var targetType = typeof(T);
            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == castMethodName && mi.ReturnType == targetType)
                .Any(mi =>
                {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });
        }

        private static T ImplicitCast<T>(this object obj)
        {
            return obj.Cast<T>(ImplicitCastMethodName);
        }

        private static T ExplicitCast<T>(this object obj)
        {
            return obj.Cast<T>(ExplicitCastMethodName);
        }

#if NET8_0_OR_GREATER
        [UnconditionalSuppressMessage("Reflection on object", "IL2075")]
#endif
        private static T Cast<T>(this object obj, string castMethodName)
        {
            var objType = obj.GetType();
            MethodInfo conversionMethod = objType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == castMethodName && mi.ReturnType == typeof(T))
                .SingleOrDefault(mi =>
                {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == objType;
                });
            return conversionMethod != null
                ? (T)conversionMethod.Invoke(null, new[] { obj })
                : throw new InvalidCastException($"No method to cast {objType.FullName} to {typeof(T).FullName}");
        }
    }
}
