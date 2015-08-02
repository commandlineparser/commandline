// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class FakeOptionsWithTwoRequired
    {
        [Option("str", Required = true)]
        public string StringValue { get; set; }

        [Option("long", Required = true)]
        public long LongValue { get; set; }

        [Option("int")]
        public int IntValue { get; set; }
    }
}
