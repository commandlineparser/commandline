using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndOnlyMaxConstraint
    {
        [Option('s', "string-seq", Max=3)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
