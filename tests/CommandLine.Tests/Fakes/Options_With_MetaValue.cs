// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

namespace CommandLine.Tests.Fakes
{
    class Options_With_MetaValue
    {
        [Option('v', "verbose", HelpText = "Comment extensively every operation.")]
        public bool Verbose { get; set; }

        [Option('i', "input-file", MetaValue = "FILE", Required = true, HelpText = "Specify input FILE to be processed.")]
        public string FileName { get; set; }
    }
}
