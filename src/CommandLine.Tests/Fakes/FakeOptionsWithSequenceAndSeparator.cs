using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequenceAndSeparator
    {
        [Option("string-seq", Separator=";")]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
