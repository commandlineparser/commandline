// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    
    [Verb("verb1")]
    class Options_HelpText_Ordering_Verb1
    {
        [Option('a', "alpha", Required = true)]
        public string alphaOption { get; set; }

        [Option('b', "alpha2", Required = true)]
        public string alphaTwoOption { get; set; }

        [Option('d', "charlie", Required = false)]
        public string deltaOption { get; set; }

        [Option('c', "bravo", Required = false)]
        public string charlieOption { get; set; }

        [Option('f', "foxtrot", Required = false)]
        public string foxOption { get; set; }

        [Option('e', "echo", Required = false)]
        public string echoOption { get; set; }

        [Value(0)] public string someExtraOption { get; set; }
    }

    [Verb("verb2")]
    class Options_HelpText_Ordering_Verb2
    {
        [Option('a', "alpha", Required = true)]
        public string alphaOption { get; set; }

        [Option('b', "alpha2", Required = true)]
        public string alphaTwoOption { get; set; }

        [Option('c', "bravo", Required = false)]
        public string charlieOption { get; set; }

        [Option('d', "charlie", Required = false)]
        public string deltaOption { get; set; }
    }
}
