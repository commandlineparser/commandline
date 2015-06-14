using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndOnlyMinConstraint
    {
        [Option('s', "string-seq", Min=1)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
