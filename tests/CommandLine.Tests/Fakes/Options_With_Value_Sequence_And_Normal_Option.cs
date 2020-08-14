// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.
using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Options_With_Value_Sequence_And_Normal_Option
    {        
        [Option('c', "compress",
            HelpText = "Compress Match Pattern, Pipe Separated (|) ",
            Separator = '|',               
            Default = new[]
            {
        "*.txt", "*.log", "*.ini"
            })]
        public IEnumerable<string> Compress { get; set; }

        [Value(0,
            HelpText = "Input Directories.",               
            Required = true)]
        public IEnumerable<string> InputDirs { get; set; }


        [Option('n', "name",
            HelpText = "Metadata Name.",
            Default = "WILDCARD")]
        public string Name { get; set; }
    }
}
