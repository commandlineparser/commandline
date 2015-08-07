// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Sequence_And_Only_Min_Constraint
    {
        [Option('s', "string-seq", Min=1)]
        public IEnumerable<string> StringSequence { get; set; }
    }
}
