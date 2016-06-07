#if NET40
using System;

namespace CommandLine.Core
{
    internal static class Compatibility
    {
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
    }
}
#endif
