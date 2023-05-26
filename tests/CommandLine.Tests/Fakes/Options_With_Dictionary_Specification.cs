using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Dictionary_Specification
    {
        [Option('d', "dict")]
        public Dictionary<string, string> KeyValuePairs { get; set; }
    }
}
