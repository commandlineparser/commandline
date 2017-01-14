// Copyright 2005-2015 Giacomo Stelluti Scala & Contributors. All rights reserved. See License.md in the project root for license information.

using System;
using System.Linq;

namespace CommandLine.Tests.Fakes
{
    class Options_With_Shuffled_Index_Values
    {
        [Value(1)]
        public string Arg1 { get; set; }

        [Value(2)]
        public string Arg2 { get; set; }

        [Value(0)]
        public string Arg0 { get; set; }
    }
}
