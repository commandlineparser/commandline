using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine
{
#if NET40
 
	internal static class IntrospectionExtensions
	{
		public static Type GetTypeInfo(this Type type)
		{
			return type;
		}
	}
#endif
}

