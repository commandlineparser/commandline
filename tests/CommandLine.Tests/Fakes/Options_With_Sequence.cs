// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Sequence
    {
        [Option("int-seq")]
        public IEnumerable<int> IntSequence { get; set; }

        //[Option("string-seq")]
        //public IEnumerable<string> StringSequence { get; set; } 
    }
}
