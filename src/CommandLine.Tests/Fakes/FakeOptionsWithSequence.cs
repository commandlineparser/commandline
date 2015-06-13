using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithSequence
    {
        [Option("int-seq")]
        public IEnumerable<int> IntSequence { get; set; }

        //[Option("string-seq")]
        //public IEnumerable<string> StringSequence { get; set; } 
    }
}
