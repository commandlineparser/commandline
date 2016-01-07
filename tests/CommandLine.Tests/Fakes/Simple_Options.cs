// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections.Generic;

namespace CommandLine.Tests.Fakes
{
    public class Simple_Options
    {
        [Option(HelpText = "Define a string value here.")]
        public string StringValue { get; set; }

        [Option('s', "shortandlong", HelpText = "Example with both short and long name.")]
        public string ShortAndLong { get; set; }

        [Option('i', Min = 3, Max = 4, HelpText = "Define a int sequence here.")]
        public IEnumerable<int> IntSequence { get; set; }

        [Option('x', HelpText = "Define a boolean or switch value here.")]
        public bool BoolValue { get; set; }

        [Value(0, HelpText = "Define a long value here.")]
        public long LongValue { get; set; }
    }
}
