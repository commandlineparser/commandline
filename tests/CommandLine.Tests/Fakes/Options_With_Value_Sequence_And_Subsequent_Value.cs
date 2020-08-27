// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Value_Sequence_And_Subsequent_Value
    {
        [Value(0)]
        public IEnumerable<string> StringSequence { get; set; }

        [Value(1)]
        public string NeverReachedValue { get; set; }
    }
}
