// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithDefaultSetToSequence
    {
        [Option('z', "strseq", Default = new[] { "a", "b", "c" })]
        public IEnumerable<string> StringSequence { get; set; }

        [Option('y', "intseq", Default = new[] { 1, 2, 3 })]
        public IEnumerable<int> IntSequence { get; set; }

        [Option('q', "dblseq", Default = new[] { 1.1, 2.2, 3.3 })]
        public IEnumerable<int> DoubleSequence { get; set; }
    }
}
