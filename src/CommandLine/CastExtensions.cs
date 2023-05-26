using System;
using System.Linq;
using System.Reflection;

namespace CommandLine
{
    internal static class CastExtensions
    {
        private const string ImplicitCastMethodName = "op_Implicit";
        private const string ExplicitCastMethodName = "op_Explicit";

        public static bool CanCast<T>(this Type baseType)
        {
            return baseType.CanImplicitCast<T>() || baseType.CanExplicitCast<T>();
        }

        public static bool CanCast<T>(this object obj)
        {
            var objType = obj.GetType();
            return objType.CanCast<T>();
        }

        public static T Cast<T>(this object obj)
        {
            try
            {
                return (T) obj;
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

        private static bool CanImplicitCast<T>(this Type baseType)
        {
            return baseType.CanCast<T>(ImplicitCastMethodName);
        }

        private static bool CanImplicitCast<T>(this object obj)
        {
            var baseType = obj.GetType();
            return baseType.CanImplicitCast<T>();
        }

        private static bool CanExplicitCast<T>(this Type baseType)
        {
            return baseType.CanCast<T>(ExplicitCastMethodName);
        }

        private static bool CanExplicitCast<T>(this object obj)
        {
            var baseType = obj.GetType();
            return baseType.CanExplicitCast<T>();
        }

        private static bool CanCast<T>(this Type baseType, string castMethodName)
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
            if (conversionMethod != null)
                return (T) conversionMethod.Invoke(null, new[] {obj});
            else
                throw new InvalidCastException($"No method to cast {objType.FullName} to {typeof(T).FullName}");
        }
    }
}
