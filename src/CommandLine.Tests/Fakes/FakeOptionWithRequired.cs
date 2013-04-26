// Copyright 2005-2013 Giacomo Stelluti Scala & Contributors. All rights reserved. See doc/License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class FakeOptionWithRequired
    {
        [Option("str", Required = true)]
        public string StringValue { get; set; }

        [Option("int")]
        public int IntValue { get; set; }
    }
}
