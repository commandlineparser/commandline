// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Default_Set_To_Sequence_With_Separator
    {
        [Option('z', "strseq", Default = new[] { "a", "b", "c" }, Separator = ',')]
        public IEnumerable<string> StringSequence { get; set; }

        [Option('y', "intseq", Default = new[] { 1, 2, 3 }, Separator = ',')]
        public IEnumerable<int> IntSequence { get; set; }

        [Option('q', "dblseq", Default = new[] { 1.1, 2.2, 3.3 }, Separator = ',')]
        public IEnumerable<int> DoubleSequence { get; set; }
    }
}
