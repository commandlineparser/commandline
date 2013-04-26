// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithValues
    {
        [Option(DefaultValue = "")]
        public string StringValue { get; set; }

        [Value(0)]
        public long LongValue { get; set; }

        [Value(1, Min = 3, Max = 3)]
        public IEnumerable<string> StringSequence { get; set; }

        [Value(2)]
        public int IntValue { get; set; }
    }
}
