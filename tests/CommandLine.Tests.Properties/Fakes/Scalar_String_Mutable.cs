using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLine.Tests.Properties.Fakes
{
    class Scalar_String_Mutable
    {
        [Option]
        public string StringValue { get; set; }
    }
}
