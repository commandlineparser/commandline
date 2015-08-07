// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    class Options_With_HelpText_And_MetaValue
    {
        [Option(
            MetaValue = "STR",
            HelpText = "Define a string value here.")]
        public string StringValue { get; set; }

        [Option('i', Min = 3, Max = 4,
            MetaValue = "INTSEQ",
            HelpText = "Define a int sequence here.")]
        public IEnumerable<int> IntSequence { get; set; }

        [Option('x',
            HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }

        [Value(0,
            MetaName = "number",
            MetaValue = "NUM",
            HelpText = "Define a long value here.")]
        public long LongValue { get; set; }

        [Value(1,
            MetaName = "paintcolor",
            MetaValue = "COLOR",
            HelpText = "Define a color value here.")]
        public Colors ColorValue { get; set; }
    }
}
