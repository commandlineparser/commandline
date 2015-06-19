using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Core
{
    internal static class KeyValuePairHelper
    {
        public static KeyValuePair<string, IEnumerable<string>> Create(string value, params string[] values)
        {
            return new KeyValuePair<string, IEnumerable<string>>(value, values);
        }
    }
}
