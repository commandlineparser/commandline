using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndOnlyMinConstraintAsValue
    {
        [Value(0, Min=1)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
