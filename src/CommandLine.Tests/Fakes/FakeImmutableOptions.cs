// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class FakeImmutableOptions
    {
        private readonly string stringValue;
        private readonly IEnumerable<int> intSequence;
        private readonly bool boolValue;
        private readonly long longValue;

        public FakeImmutableOptions(string stringValue, IEnumerable<int> intSequence, bool boolValue, long longValue)
        {
            this.stringValue = stringValue;
            this.intSequence = intSequence;
            this.boolValue = boolValue;
            this.longValue = longValue;
        }

        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get { return stringValue; } }

        [Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")]
        public IEnumerable<int> IntSequence { get { return intSequence; } }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get { return boolValue; } }

        [Value(0)]
        public long LongValue { get { return longValue; } }
    }
}
