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
        private readonly Type fsharpOptionType;

        private FSharpOptionHelper()
        {
            Assembly fsharpCoreAssembly;
            try
            {
                fsharpCoreAssembly =
                    Assembly.Load(Path.Combine(Assembly.GetCallingAssembly().Location, "FSharp.Core.dll"));
            }
            catch (FileNotFoundException) { fsharpCoreAssembly = null; }
            catch (FileLoadException)  { fsharpCoreAssembly = null; }
            if (fsharpCoreAssembly != null)
            {
                fsharpOptionType = fsharpCoreAssembly.GetType("FSharpOption`1", false);
            }
        }

        private bool Available
        {
            get { return fsharpOptionType != null; }
        }

        public bool MatchType(Type type)
        {
            if (!Available)
            {
                throw new InvalidOperationException();
            }
            return type.IsAssignableFrom(fsharpOptionType);
        }
    }
}
