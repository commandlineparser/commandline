using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndSeparator
    {
        [Option("long-seq", Min=0, Max=100, Separator=";")] //todo: problem omitting min/max -> defauly should be no check
        public IEnumerable<long> LongSequence { get; set; }
    }
}
