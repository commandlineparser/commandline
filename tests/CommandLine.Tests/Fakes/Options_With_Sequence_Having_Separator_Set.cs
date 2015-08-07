// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Sequence_Having_Separator_Set
    {
        [Option("long-seq", Separator=';')]
        public IEnumerable<long> LongSequence { get; set; }

        [Option('s', Min = 1, Max = 100, Separator = ',')]
        public IEnumerable<string> StringSequence { get; set; } 
    }
}
