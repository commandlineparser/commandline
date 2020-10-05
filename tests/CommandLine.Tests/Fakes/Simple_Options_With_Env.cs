// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Simple_Options_With_Env
    {
        [Option('s', Default = "", Env = "StringValue")]
        public string StringValue { get; set; }

        [Option("bvff", Env = "BoolValueFullFalse", HelpText = "Define a boolean or switch value here.")]
        public bool BoolValueFullFalse { get; set; }
        [Option("bvft", Env = "BoolValueFullTrue", HelpText = "Define a boolean or switch value here.")]
        public bool BoolValueFullTrue { get; set; }
        [Option("bvst", Env = "BoolValueShortTrue", HelpText = "Define a boolean or switch value here.")]
        public bool BoolValueShortTrue { get; set; }
        [Option("bvsf", Env = "BoolValueShortFalse", HelpText = "Define a boolean or switch value here.")]
        public bool BoolValueShortFalse { get; set; }


        [Option('l', Default = 1, Env = "LongValue")]
        public long LongValue { get; set; }

        [Option('i', Default = 2, Env = "IntValue")]
        public long IntValue { get; set; }
    }
}
