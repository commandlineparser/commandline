using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceWithoutRange
    {
        [Value(0)]
        public IEnumerable<long> LongSequence { get; set; } 
    }
}
