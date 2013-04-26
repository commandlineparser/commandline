using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithDouble
    {
        [Value(0)]
        public double DoubleValue { get; set; }
    }
}
