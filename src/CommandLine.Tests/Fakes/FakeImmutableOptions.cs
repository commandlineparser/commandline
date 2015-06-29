// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class FakeImmutableOptions
    {
        public FakeImmutableOptions(string stringValue, IEnumerable<int> intSequence, bool boolValue, long longValue)
        {
            StringValue = stringValue;
            IntSequence = intSequence;
            BoolValue = boolValue;
            LongValue = longValue;
        }

        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get; private set; }

        [Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")]
        public IEnumerable<int> IntSequence { get; private set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; private set; }

        [Value(0)]
        public long LongValue { get; private set; }
    }
}
