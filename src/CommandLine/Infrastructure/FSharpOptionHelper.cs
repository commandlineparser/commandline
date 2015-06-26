using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommandLine.Infrastructure
{
    class FSharpOptionHelper
    {
        public static bool MatchType(Type type)
        {
            return type.FullName.StartsWith(
                "Microsoft.FSharp.Core.FSharpOption`1", StringComparison.Ordinal);
        }


    }
}
