// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    public class Options_With_FlagCounter_Switches
    {
        [Option('v', FlagCounter=true)]
        public int Verbose { get; set; }

        [Option('s', FlagCounter=true)]
        public int Silent { get; set; }
    }
}
