using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndOnlyMaxConstraintAsValue
    {
        [Value(0, Max=3)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
